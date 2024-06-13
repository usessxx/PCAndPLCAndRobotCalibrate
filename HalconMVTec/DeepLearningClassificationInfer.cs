using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Windows.Forms;
using System.Threading;

namespace HalconMVTec
{
    public class DeepLearningClassificationInfer
    {
        // Local control variables 
        HTuple hv_PreprocessResultDirectoryPath = new HTuple();//存储预处理结果-sample文件夹 及 hdict文件的上一级文件夹路径(包含文件夹名)
        //HTuple hv_PreprocessResultFileName = new HTuple();//预处理结果文件名(以".hdict"为后缀,例如:dl_preprocess_dataset.hdict)
        HTuple hv_PreprocessResultParameterFileName = new HTuple();//预处理结果-参数的文件名(以".hdict"为后缀,例如:dl_preprocess_param.hdict)
        HTuple hv_BestModelBaseName = new HTuple();//已训练好的Best模型文件的存放路径(包含文件名,不需要后缀)(例如:best_dl_model_classification.hdl;best_dl_model_classification_info.hdict)

        HTuple hv_BatchSize = new HTuple();
        HTuple hv_UseGPU = new HTuple();

        HTuple hv_CudaLoaded = new HTuple();
        HTuple hv_CuDNNLoaded = new HTuple();
        HTuple hv_CuBlasLoaded = new HTuple();
        public HTuple hv_DLModelHandle = new HTuple();//此实验约中,占用800M的GPU内存
        HTuple hv_ClassNames = new HTuple();
        HTuple hv_ClassIDs = new HTuple();
        HTuple hv_DLPreprocessParam = new HTuple();
        HTuple hv_WindowHandleDict = new HTuple();
        HTuple hv_DLDataInfo = new HTuple();
        HTuple hv_GenParam = new HTuple();
        HTuple hv_ImageFiles = new HTuple();
        HTuple hv_Index = new HTuple();
        HTuple hv_DLSampleBatch = new HTuple();
        HTuple hv_DLResultBatch = new HTuple();
        HTuple hv_classification_class_names = new HTuple();
        HTuple hv_Width = new HTuple();
        HTuple hv_Height = new HTuple();
        HTuple hv_WindowHandle = new HTuple();

        //ADD FOR DISPLAY
        public HWindowControl hv_HWindowControl = new HWindowControl();//显示用窗口控件

        public bool displayNextFlag = false;

        #region DeepLearningClassificationInfer(构造函数里对推理参数初始化)
        //构造函数里对评估参数初始化
        public DeepLearningClassificationInfer()
        {
            hv_BatchSize = 1;
            hv_UseGPU = 1;
        }
        #endregion

        #region InferAction(推理开始)
        public string InferAction(string preprocessResultDirectoryPath, string preprocessResultParameterFileName, string bestModelBaseName, int batchSize, bool useGPU, HObject ho_Image_Infer, bool needReadparameter, bool needDispose)
        {
            try
            {
                if (needReadparameter)//读取参数耗费时间
                {
                    hv_PreprocessResultDirectoryPath = preprocessResultDirectoryPath;
                    hv_PreprocessResultParameterFileName = preprocessResultParameterFileName;
                    hv_BestModelBaseName = bestModelBaseName;
                    hv_BatchSize = batchSize;
                    if (useGPU)
                        hv_UseGPU = 1;
                    else
                        hv_UseGPU = 0;

                    //推理的批次大小
                    //Batch Size used during inference.
                    hv_BatchSize = 1;
                    //
                    //The inference can be done on GPU or CPU.
                    //See the respective system requirements in the Installation Guide.
                    //hv_UseGPU = 1;
                    //部署的时候,无论用不用GPU,都要部署cuda与cudnn
                    //********************
                    //**   Inference   ***
                    //********************
                    //
                    //Check availability of GPU mode.
                    if ((int)(hv_UseGPU) != 0)
                    {
                        HOperatorSet.GetSystem("cuda_loaded", out hv_CudaLoaded);
                        HOperatorSet.GetSystem("cudnn_loaded", out hv_CuDNNLoaded);
                        HOperatorSet.GetSystem("cublas_loaded", out hv_CuBlasLoaded);
                        if ((int)((new HTuple((new HTuple((new HTuple(hv_CudaLoaded.TupleEqual("true"))).TupleAnd(
                            new HTuple(hv_CuDNNLoaded.TupleEqual("true"))))).TupleAnd(new HTuple(hv_CuBlasLoaded.TupleEqual(
                            "true"))))).TupleNot()) != 0)
                        {
                            hv_UseGPU = 0;
                        }
                    }
                    //Check if all necessary files exist.
                    check_data_availability(hv_PreprocessResultDirectoryPath, hv_PreprocessResultDirectoryPath + "\\" + hv_PreprocessResultParameterFileName, hv_BestModelBaseName + ".hdl", 0);
                    //Read in the retrained model.
                    HOperatorSet.ReadDlModel(hv_BestModelBaseName + ".hdl", out hv_DLModelHandle);
                    //Get the class names and IDs from the model.
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);
                    //Set the batch size.
                    HOperatorSet.SetDlModelParam(hv_DLModelHandle, "batch_size", hv_BatchSize);
                    //Initialize the model for inference.
                    if ((int)(hv_UseGPU) == 1)
                    {
                        HOperatorSet.SetDlModelParam(hv_DLModelHandle, "runtime", "gpu");
                    }
                    if ((int)(hv_UseGPU) != 1)
                    {
                        HOperatorSet.SetDlModelParam(hv_DLModelHandle, "runtime", "cpu");
                    }

                    HOperatorSet.SetDlModelParam(hv_DLModelHandle, "runtime_init", "immediately");

                    //Get the parameters used for preprocessing.
                    HOperatorSet.ReadDict(hv_PreprocessResultDirectoryPath + "\\" + hv_PreprocessResultParameterFileName, new HTuple(), new HTuple(), out hv_DLPreprocessParam);
                    //Create window dictionary for displaying results.
                    HOperatorSet.CreateDict(out hv_WindowHandleDict);
                    //Create dictionary with dataset parameters necessary for displaying.
                    HOperatorSet.CreateDict(out hv_DLDataInfo);
                    HOperatorSet.SetDictTuple(hv_DLDataInfo, "class_names", hv_ClassNames);
                    HOperatorSet.SetDictTuple(hv_DLDataInfo, "class_ids", hv_ClassIDs);
                    //Set generic parameters for visualization.
                    HOperatorSet.CreateDict(out hv_GenParam);
                    HOperatorSet.SetDictTuple(hv_GenParam, "scale_windows", 1.1);
                }

                //最终部署的推理用的代码
                //Generate the DLSampleBatch.

                gen_dl_samples_from_images(ho_Image_Infer, out hv_DLSampleBatch);
                //Preprocess the DLSampleBatch.
                preprocess_dl_samples(hv_DLSampleBatch, hv_DLPreprocessParam);
                //Apply the DL model on the DLSampleBatch.
                HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleBatch, new HTuple(), out hv_DLResultBatch);


                //解析结果
                HOperatorSet.GetDictTuple(hv_DLResultBatch, "classification_class_names", out hv_classification_class_names);
                HTuple ExpTmpLocalVar_classification_class_names = hv_classification_class_names.TupleSelect(0);
                hv_classification_class_names = ExpTmpLocalVar_classification_class_names;

            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image_Infer.Dispose();
                hv_BatchSize.Dispose();
                hv_UseGPU.Dispose();
                hv_CudaLoaded.Dispose();
                hv_CuDNNLoaded.Dispose();
                hv_CuBlasLoaded.Dispose();
                hv_DLModelHandle.Dispose();
                hv_ClassNames.Dispose();
                hv_ClassIDs.Dispose();
                hv_DLPreprocessParam.Dispose();
                hv_WindowHandleDict.Dispose();
                hv_DLDataInfo.Dispose();
                hv_GenParam.Dispose();
                hv_ImageFiles.Dispose();
                hv_Index.Dispose();
                hv_DLSampleBatch.Dispose();
                hv_DLResultBatch.Dispose();
                hv_classification_class_names.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WindowHandle.Dispose();
                MessageBox.Show(HDevExpDefaultException.ToString());
                return "";
                throw HDevExpDefaultException;
            }

            //如果不释放如下变量,反复调用本函数时会导致GPU不足
            if (needDispose)
            {
                //在手动选择一张图片测试一次后,再次按下测试按钮,会出错:debug检查是主画面里选择的图片ho_selectImage为空.是因为以下的ho_Image_Infer.Dispose()造成的?
                //ho_Image_Infer.Dispose();
                hv_BatchSize.Dispose();
                hv_UseGPU.Dispose();
                hv_CudaLoaded.Dispose();
                hv_CuDNNLoaded.Dispose();
                hv_CuBlasLoaded.Dispose();
                hv_DLModelHandle.Dispose();//此实验约中,占用800M的GPU内存
                hv_ClassNames.Dispose();
                hv_ClassIDs.Dispose();
                hv_DLPreprocessParam.Dispose();//
                hv_WindowHandleDict.Dispose();
                hv_DLDataInfo.Dispose();
                hv_GenParam.Dispose();
                hv_ImageFiles.Dispose();
                hv_Index.Dispose();
                hv_DLSampleBatch.Dispose();//
                hv_DLResultBatch.Dispose();//
                hv_classification_class_names.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WindowHandle.Dispose();
            }
            return hv_classification_class_names;
        }
        #endregion

        #region TestimagesInfer(测试文件夹图片推理)
        public void TestImagesInfer(string preprocessClassificationResultDirectoryPath, string preprocessClassificationResultParameterFileName, string bestModelBaseName, int batchSize, bool useGPU, string testimagesDirectoryPath)
        {
            HObject ho_Image_Infer;
            bool needReadparameter = true;
            bool needDispose = false;
            HOperatorSet.ListFiles(testimagesDirectoryPath, (new HTuple("files")).TupleConcat("follow_links"), out hv_ImageFiles);
            {
                HTuple ExpTmpOutVar_0;
                HOperatorSet.TupleRegexpSelect(hv_ImageFiles, (new HTuple("\\.(tif|tiff|gif|bmp|jpg|jpeg|jp2|png|pcx|pgm|ppm|pbm|xwd|ima|hobj)$")).TupleConcat("ignore_case"), out ExpTmpOutVar_0);
                hv_ImageFiles = ExpTmpOutVar_0;
            }
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ImageFiles.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                HOperatorSet.ReadImage(out ho_Image_Infer, hv_ImageFiles.TupleSelect(hv_Index));
                //DISPLAY
                DispImageAdaptively(ref hv_HWindowControl, ho_Image_Infer);

                if (hv_Index > 0)
                    needReadparameter = false;
                if ((int)hv_Index == (int)((new HTuple(hv_ImageFiles.TupleLength())) - 1))
                    needDispose = true;

                HTuple referResult = InferAction(preprocessClassificationResultDirectoryPath, preprocessClassificationResultParameterFileName, bestModelBaseName, batchSize, useGPU, ho_Image_Infer, needReadparameter, needDispose);
                HOperatorSet.DispText(hv_HWindowControl.HalconWindow, referResult, "window", "top", "left", "red", "box", "true");
                displayNextFlag = false;
                while (!displayNextFlag)
                {
                    Thread.Sleep(100);
                }
                displayNextFlag = false;
            }
        }
        #endregion


        // Procedures 
        // External procedures 
        public void add_colormap_to_image(HObject ho_GrayValueImage, HObject ho_Image,
            out HObject ho_ColoredImage, HTuple hv_HeatmapColorScheme)
        {
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RGBValueImage, ho_Channels, ho_ChannelsScaled;
            HObject ho_Channel = null, ho_ChannelScaled = null, ho_ChannelScaledByte = null;
            HObject ho_ImageByte, ho_ImageByteR = null, ho_ImageByteG = null;
            HObject ho_ImageByteB = null;

            // Local copy input parameter variables 
            HObject ho_GrayValueImage_COPY_INP_TMP;
            ho_GrayValueImage_COPY_INP_TMP = new HObject(ho_GrayValueImage);



            // Local control variables 

            HTuple hv_Type = new HTuple(), hv_NumChannels = new HTuple();
            HTuple hv_ChannelIndex = new HTuple(), hv_ChannelMin = new HTuple();
            HTuple hv_ChannelMax = new HTuple(), hv__ = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ColoredImage);
            HOperatorSet.GenEmptyObj(out ho_RGBValueImage);
            HOperatorSet.GenEmptyObj(out ho_Channels);
            HOperatorSet.GenEmptyObj(out ho_ChannelsScaled);
            HOperatorSet.GenEmptyObj(out ho_Channel);
            HOperatorSet.GenEmptyObj(out ho_ChannelScaled);
            HOperatorSet.GenEmptyObj(out ho_ChannelScaledByte);
            HOperatorSet.GenEmptyObj(out ho_ImageByte);
            HOperatorSet.GenEmptyObj(out ho_ImageByteR);
            HOperatorSet.GenEmptyObj(out ho_ImageByteG);
            HOperatorSet.GenEmptyObj(out ho_ImageByteB);
            try
            {
                //
                //This procedure adds a gray-value image to a RGB image with a chosen colormap.
                //
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_GrayValueImage_COPY_INP_TMP, out hv_Type);
                //The image LUT needs a byte image. Rescale real images.
                if ((int)(new HTuple(hv_Type.TupleEqual("real"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        scale_image_range(ho_GrayValueImage_COPY_INP_TMP, out ExpTmpOutVar_0, 0,
                            1);
                        ho_GrayValueImage_COPY_INP_TMP.Dispose();
                        ho_GrayValueImage_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_GrayValueImage_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "byte");
                        ho_GrayValueImage_COPY_INP_TMP.Dispose();
                        ho_GrayValueImage_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_Type.TupleNotEqual("byte"))) != 0)
                {
                    throw new HalconException(new HTuple("For this transformation, a byte or real image is needed!"));
                }
                //
                //Apply the chosen color scheme on the gray value.
                ho_RGBValueImage.Dispose();
                apply_colorscheme_on_gray_value_image(ho_GrayValueImage_COPY_INP_TMP, out ho_RGBValueImage,
                    hv_HeatmapColorScheme);
                //
                //Convert input image to byte image for visualization.
                ho_Channels.Dispose();
                HOperatorSet.ImageToChannels(ho_Image, out ho_Channels);
                hv_NumChannels.Dispose();
                HOperatorSet.CountChannels(ho_Image, out hv_NumChannels);
                ho_ChannelsScaled.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ChannelsScaled);
                HTuple end_val19 = hv_NumChannels;
                HTuple step_val19 = 1;
                for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val19, step_val19); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val19))
                {
                    ho_Channel.Dispose();
                    HOperatorSet.SelectObj(ho_Channels, out ho_Channel, hv_ChannelIndex);
                    hv_ChannelMin.Dispose(); hv_ChannelMax.Dispose(); hv__.Dispose();
                    HOperatorSet.MinMaxGray(ho_Channel, ho_Channel, 0, out hv_ChannelMin, out hv_ChannelMax,
                        out hv__);
                    ho_ChannelScaled.Dispose();
                    scale_image_range(ho_Channel, out ho_ChannelScaled, hv_ChannelMin, hv_ChannelMax);
                    ho_ChannelScaledByte.Dispose();
                    HOperatorSet.ConvertImageType(ho_ChannelScaled, out ho_ChannelScaledByte,
                        "byte");
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_ChannelsScaled, ho_ChannelScaledByte, out ExpTmpOutVar_0
                            );
                        ho_ChannelsScaled.Dispose();
                        ho_ChannelsScaled = ExpTmpOutVar_0;
                    }
                }
                ho_ImageByte.Dispose();
                HOperatorSet.ChannelsToImage(ho_ChannelsScaled, out ho_ImageByte);
                //
                //Note that ImageByte needs to have the same number of channels as
                //RGBValueImage to display colormap image correctly.
                hv_NumChannels.Dispose();
                HOperatorSet.CountChannels(ho_ImageByte, out hv_NumChannels);
                if ((int)(new HTuple(hv_NumChannels.TupleNotEqual(3))) != 0)
                {
                    //Just take the first channel and use this to generate
                    //an image with 3 channels for visualization.
                    ho_ImageByteR.Dispose();
                    HOperatorSet.AccessChannel(ho_ImageByte, out ho_ImageByteR, 1);
                    ho_ImageByteG.Dispose();
                    HOperatorSet.CopyImage(ho_ImageByteR, out ho_ImageByteG);
                    ho_ImageByteB.Dispose();
                    HOperatorSet.CopyImage(ho_ImageByteR, out ho_ImageByteB);
                    ho_ImageByte.Dispose();
                    HOperatorSet.Compose3(ho_ImageByteR, ho_ImageByteG, ho_ImageByteB, out ho_ImageByte
                        );
                }
                //
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.AddImage(ho_ImageByte, ho_RGBValueImage, out ExpTmpOutVar_0, 0.5,
                        0);
                    ho_RGBValueImage.Dispose();
                    ho_RGBValueImage = ExpTmpOutVar_0;
                }
                ho_ColoredImage.Dispose();
                ho_ColoredImage = new HObject(ho_RGBValueImage);
                //
                ho_GrayValueImage_COPY_INP_TMP.Dispose();
                ho_RGBValueImage.Dispose();
                ho_Channels.Dispose();
                ho_ChannelsScaled.Dispose();
                ho_Channel.Dispose();
                ho_ChannelScaled.Dispose();
                ho_ChannelScaledByte.Dispose();
                ho_ImageByte.Dispose();
                ho_ImageByteR.Dispose();
                ho_ImageByteG.Dispose();
                ho_ImageByteB.Dispose();

                hv_Type.Dispose();
                hv_NumChannels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_ChannelMin.Dispose();
                hv_ChannelMax.Dispose();
                hv__.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_GrayValueImage_COPY_INP_TMP.Dispose();
                ho_RGBValueImage.Dispose();
                ho_Channels.Dispose();
                ho_ChannelsScaled.Dispose();
                ho_Channel.Dispose();
                ho_ChannelScaled.Dispose();
                ho_ChannelScaledByte.Dispose();
                ho_ImageByte.Dispose();
                ho_ImageByteR.Dispose();
                ho_ImageByteG.Dispose();
                ho_ImageByteB.Dispose();

                hv_Type.Dispose();
                hv_NumChannels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_ChannelMin.Dispose();
                hv_ChannelMax.Dispose();
                hv__.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Short Description: Create a lookup table and convert a grey scale image. 
        public void apply_colorscheme_on_gray_value_image(HObject ho_InputImage, out HObject ho_ResultImage,
            HTuple hv_Schema)
        {




            // Local iconic variables 

            HObject ho_ImageR, ho_ImageG, ho_ImageB;

            // Local control variables 

            HTuple hv_X = new HTuple(), hv_Low = new HTuple();
            HTuple hv_High = new HTuple(), hv_OffR = new HTuple();
            HTuple hv_OffG = new HTuple(), hv_OffB = new HTuple();
            HTuple hv_A1 = new HTuple(), hv_A0 = new HTuple(), hv_R = new HTuple();
            HTuple hv_G = new HTuple(), hv_B = new HTuple(), hv_A0R = new HTuple();
            HTuple hv_A0G = new HTuple(), hv_A0B = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ResultImage);
            HOperatorSet.GenEmptyObj(out ho_ImageR);
            HOperatorSet.GenEmptyObj(out ho_ImageG);
            HOperatorSet.GenEmptyObj(out ho_ImageB);
            try
            {
                //
                //This procedure generates an RGB ResultImage for a grey-value InputImage.
                //In order to do so, create a color distribution as look up table
                //according to the Schema.
                //
                hv_X.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_X = HTuple.TupleGenSequence(
                        0, 255, 1);
                }
                hv_Low.Dispose();
                HOperatorSet.TupleGenConst(256, 0, out hv_Low);
                hv_High.Dispose();
                HOperatorSet.TupleGenConst(256, 255, out hv_High);
                //
                if ((int)(new HTuple(hv_Schema.TupleEqual("jet"))) != 0)
                {
                    //Scheme Jet: from blue to red
                    hv_OffR.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_OffR = 3.0 * 64.0;
                    }
                    hv_OffG.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_OffG = 2.0 * 64.0;
                    }
                    hv_OffB.Dispose();
                    hv_OffB = 64.0;
                    hv_A1.Dispose();
                    hv_A1 = -4.0;
                    hv_A0.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_A0 = 255.0 + 128.0;
                    }
                    hv_R.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_R = ((((((((hv_X - hv_OffR)).TupleAbs()
                            ) * hv_A1) + hv_A0)).TupleMax2(hv_Low))).TupleMin2(hv_High);
                    }
                    hv_G.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_G = ((((((((hv_X - hv_OffG)).TupleAbs()
                            ) * hv_A1) + hv_A0)).TupleMax2(hv_Low))).TupleMin2(hv_High);
                    }
                    hv_B.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_B = ((((((((hv_X - hv_OffB)).TupleAbs()
                            ) * hv_A1) + hv_A0)).TupleMax2(hv_Low))).TupleMin2(hv_High);
                    }
                    //
                }
                else if ((int)(new HTuple(hv_Schema.TupleEqual("inverse_jet"))) != 0)
                {
                    //Scheme InvJet: from red to blue.
                    hv_OffR.Dispose();
                    hv_OffR = 64;
                    hv_OffG.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_OffG = 2 * 64;
                    }
                    hv_OffB.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_OffB = 3 * 64;
                    }
                    hv_A1.Dispose();
                    hv_A1 = -4.0;
                    hv_A0.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_A0 = 255.0 + 128.0;
                    }
                    hv_R.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_R = ((((((((hv_X - hv_OffR)).TupleAbs()
                            ) * hv_A1) + hv_A0)).TupleMax2(hv_Low))).TupleMin2(hv_High);
                    }
                    hv_G.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_G = ((((((((hv_X - hv_OffG)).TupleAbs()
                            ) * hv_A1) + hv_A0)).TupleMax2(hv_Low))).TupleMin2(hv_High);
                    }
                    hv_B.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_B = ((((((((hv_X - hv_OffB)).TupleAbs()
                            ) * hv_A1) + hv_A0)).TupleMax2(hv_Low))).TupleMin2(hv_High);
                    }
                    //
                }
                else if ((int)(new HTuple(hv_Schema.TupleEqual("hot"))) != 0)
                {
                    //Scheme Hot.
                    hv_A1.Dispose();
                    hv_A1 = 3.0;
                    hv_A0R.Dispose();
                    hv_A0R = 0.0;
                    hv_A0G.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_A0G = ((1.0 / 3.0) * hv_A1) * 255.0;
                    }
                    hv_A0B.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_A0B = ((2.0 / 3.0) * hv_A1) * 255.0;
                    }
                    hv_R.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_R = (((((hv_X * hv_A1) - hv_A0R)).TupleMax2(
                            hv_Low))).TupleMin2(hv_High);
                    }
                    hv_G.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_G = (((((hv_X * hv_A1) - hv_A0G)).TupleMax2(
                            hv_Low))).TupleMin2(hv_High);
                    }
                    hv_B.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_B = (((((hv_X * hv_A1) - hv_A0B)).TupleMax2(
                            hv_Low))).TupleMin2(hv_High);
                    }
                    //
                }
                else if ((int)(new HTuple(hv_Schema.TupleEqual("inverse_hot"))) != 0)
                {
                    //Scheme Inverse Hot.
                    hv_A1.Dispose();
                    hv_A1 = -3.0;
                    hv_A0R.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_A0R = hv_A1 * 255.0;
                    }
                    hv_A0G.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_A0G = ((2.0 / 3.0) * hv_A1) * 255.0;
                    }
                    hv_A0B.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_A0B = ((1.0 / 3.0) * hv_A1) * 255.0;
                    }
                    hv_R.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_R = (((((hv_X * hv_A1) - hv_A0R)).TupleMax2(
                            hv_Low))).TupleMin2(hv_High);
                    }
                    hv_G.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_G = (((((hv_X * hv_A1) - hv_A0G)).TupleMax2(
                            hv_Low))).TupleMin2(hv_High);
                    }
                    hv_B.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_B = (((((hv_X * hv_A1) - hv_A0B)).TupleMax2(
                            hv_Low))).TupleMin2(hv_High);
                    }
                    //
                }
                else
                {
                    //
                    throw new HalconException(("Unknown color schema: " + hv_Schema) + ".");
                    //
                }
                //
                ho_ImageR.Dispose();
                HOperatorSet.LutTrans(ho_InputImage, out ho_ImageR, hv_R);
                ho_ImageG.Dispose();
                HOperatorSet.LutTrans(ho_InputImage, out ho_ImageG, hv_G);
                ho_ImageB.Dispose();
                HOperatorSet.LutTrans(ho_InputImage, out ho_ImageB, hv_B);
                ho_ResultImage.Dispose();
                HOperatorSet.Compose3(ho_ImageR, ho_ImageG, ho_ImageB, out ho_ResultImage);
                //
                ho_ImageR.Dispose();
                ho_ImageG.Dispose();
                ho_ImageB.Dispose();

                hv_X.Dispose();
                hv_Low.Dispose();
                hv_High.Dispose();
                hv_OffR.Dispose();
                hv_OffG.Dispose();
                hv_OffB.Dispose();
                hv_A1.Dispose();
                hv_A0.Dispose();
                hv_R.Dispose();
                hv_G.Dispose();
                hv_B.Dispose();
                hv_A0R.Dispose();
                hv_A0G.Dispose();
                hv_A0B.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageR.Dispose();
                ho_ImageG.Dispose();
                ho_ImageB.Dispose();

                hv_X.Dispose();
                hv_Low.Dispose();
                hv_High.Dispose();
                hv_OffR.Dispose();
                hv_OffG.Dispose();
                hv_OffB.Dispose();
                hv_A1.Dispose();
                hv_A0.Dispose();
                hv_R.Dispose();
                hv_G.Dispose();
                hv_B.Dispose();
                hv_A0R.Dispose();
                hv_A0G.Dispose();
                hv_A0B.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Checks the content of the parameter dictionary DLPreprocessParam. 
        public void check_dl_preprocess_param(HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_DLModelType = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_SupportedModelTypes = new HTuple(), hv_Index = new HTuple();
            HTuple hv_ParamNamesGeneral = new HTuple(), hv_ParamNamesSegmentation = new HTuple();
            HTuple hv_ParamNamesDetectionOptional = new HTuple(), hv_ParamNamesPreprocessingOptional = new HTuple();
            HTuple hv_ParamNamesAll = new HTuple(), hv_ParamNames = new HTuple();
            HTuple hv_KeysExists = new HTuple(), hv_I = new HTuple();
            HTuple hv_Exists = new HTuple(), hv_InputKeys = new HTuple();
            HTuple hv_Key = new HTuple(), hv_Value = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_ValidValues = new HTuple();
            HTuple hv_ValidTypes = new HTuple(), hv_V = new HTuple();
            HTuple hv_T = new HTuple(), hv_IsInt = new HTuple(), hv_ValidTypesListing = new HTuple();
            HTuple hv_ValidValueListing = new HTuple(), hv_EmptyStrings = new HTuple();
            HTuple hv_ImageRangeMinExists = new HTuple(), hv_ImageRangeMaxExists = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_IndexParam = new HTuple(), hv_SetBackgroundID = new HTuple();
            HTuple hv_ClassIDsBackground = new HTuple(), hv_Intersection = new HTuple();
            HTuple hv_IgnoreClassIDs = new HTuple(), hv_KnownClasses = new HTuple();
            HTuple hv_IgnoreClassID = new HTuple(), hv_OptionalKeysExist = new HTuple();
            HTuple hv_InstanceType = new HTuple(), hv_IgnoreDirection = new HTuple();
            HTuple hv_ClassIDsNoOrientation = new HTuple(), hv_SemTypes = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //
                //This procedure checks a dictionary with parameters for DL preprocessing.
                //
                try
                {
                    hv_DLModelType.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_DLModelType);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    throw new HalconException(new HTuple(new HTuple("DLPreprocessParam needs the parameter: '") + "model_type") + "'");
                }
                //
                //Check for correct model type.
                hv_SupportedModelTypes.Dispose();
                hv_SupportedModelTypes = new HTuple();
                hv_SupportedModelTypes[0] = "anomaly_detection";
                hv_SupportedModelTypes[1] = "classification";
                hv_SupportedModelTypes[2] = "detection";
                hv_SupportedModelTypes[3] = "segmentation";
                hv_Index.Dispose();
                HOperatorSet.TupleFind(hv_SupportedModelTypes, hv_DLModelType, out hv_Index);
                if ((int)((new HTuple(hv_Index.TupleEqual(-1))).TupleOr(new HTuple(hv_Index.TupleEqual(
                    new HTuple())))) != 0)
                {
                    throw new HalconException(new HTuple("Only models of type 'anomaly_detection', 'classification', 'detection', or 'segmentation' are supported"));

                    hv_DLModelType.Dispose();
                    hv_Exception.Dispose();
                    hv_SupportedModelTypes.Dispose();
                    hv_Index.Dispose();
                    hv_ParamNamesGeneral.Dispose();
                    hv_ParamNamesSegmentation.Dispose();
                    hv_ParamNamesDetectionOptional.Dispose();
                    hv_ParamNamesPreprocessingOptional.Dispose();
                    hv_ParamNamesAll.Dispose();
                    hv_ParamNames.Dispose();
                    hv_KeysExists.Dispose();
                    hv_I.Dispose();
                    hv_Exists.Dispose();
                    hv_InputKeys.Dispose();
                    hv_Key.Dispose();
                    hv_Value.Dispose();
                    hv_Indices.Dispose();
                    hv_ValidValues.Dispose();
                    hv_ValidTypes.Dispose();
                    hv_V.Dispose();
                    hv_T.Dispose();
                    hv_IsInt.Dispose();
                    hv_ValidTypesListing.Dispose();
                    hv_ValidValueListing.Dispose();
                    hv_EmptyStrings.Dispose();
                    hv_ImageRangeMinExists.Dispose();
                    hv_ImageRangeMaxExists.Dispose();
                    hv_ImageRangeMin.Dispose();
                    hv_ImageRangeMax.Dispose();
                    hv_IndexParam.Dispose();
                    hv_SetBackgroundID.Dispose();
                    hv_ClassIDsBackground.Dispose();
                    hv_Intersection.Dispose();
                    hv_IgnoreClassIDs.Dispose();
                    hv_KnownClasses.Dispose();
                    hv_IgnoreClassID.Dispose();
                    hv_OptionalKeysExist.Dispose();
                    hv_InstanceType.Dispose();
                    hv_IgnoreDirection.Dispose();
                    hv_ClassIDsNoOrientation.Dispose();
                    hv_SemTypes.Dispose();

                    return;
                }
                //
                //Parameter names that are required.
                //General parameters.
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesGeneral = new HTuple();
                hv_ParamNamesGeneral[0] = "model_type";
                hv_ParamNamesGeneral[1] = "image_width";
                hv_ParamNamesGeneral[2] = "image_height";
                hv_ParamNamesGeneral[3] = "image_num_channels";
                hv_ParamNamesGeneral[4] = "image_range_min";
                hv_ParamNamesGeneral[5] = "image_range_max";
                hv_ParamNamesGeneral[6] = "normalization_type";
                hv_ParamNamesGeneral[7] = "domain_handling";
                //Segmentation specific parameters.
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesSegmentation = new HTuple();
                hv_ParamNamesSegmentation[0] = "ignore_class_ids";
                hv_ParamNamesSegmentation[1] = "set_background_id";
                hv_ParamNamesSegmentation[2] = "class_ids_background";
                //Detection specific parameters.
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesDetectionOptional = new HTuple();
                hv_ParamNamesDetectionOptional[0] = "instance_type";
                hv_ParamNamesDetectionOptional[1] = "ignore_direction";
                hv_ParamNamesDetectionOptional[2] = "class_ids_no_orientation";
                //Normalization specific parameters.
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNamesPreprocessingOptional = new HTuple();
                hv_ParamNamesPreprocessingOptional[0] = "mean_values_normalization";
                hv_ParamNamesPreprocessingOptional[1] = "deviation_values_normalization";
                //All parameters
                hv_ParamNamesAll.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ParamNamesAll = new HTuple();
                    hv_ParamNamesAll = hv_ParamNamesAll.TupleConcat(hv_ParamNamesGeneral, hv_ParamNamesSegmentation, hv_ParamNamesDetectionOptional, hv_ParamNamesPreprocessingOptional);
                }
                hv_ParamNames.Dispose();
                hv_ParamNames = new HTuple(hv_ParamNamesGeneral);
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                {
                    //Extend ParamNames for models of type segmentation.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ParamNames = hv_ParamNames.TupleConcat(
                                hv_ParamNamesSegmentation);
                            hv_ParamNames.Dispose();
                            hv_ParamNames = ExpTmpLocalVar_ParamNames;
                        }
                    }
                }
                //
                //Check if legacy parameter exist.
                //Otherwise map it to the legal parameter.
                replace_legacy_preprocessing_parameters(hv_DLPreprocessParam);
                //
                //Check that all necessary parameters are included.
                //
                hv_KeysExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNames,
                    out hv_KeysExists);
                if ((int)(new HTuple(((((hv_KeysExists.TupleEqualElem(0))).TupleSum())).TupleGreater(
                    0))) != 0)
                {
                    for (hv_I = 0; (int)hv_I <= (int)(new HTuple(hv_KeysExists.TupleLength())); hv_I = (int)hv_I + 1)
                    {
                        hv_Exists.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Exists = hv_KeysExists.TupleSelect(
                                hv_I);
                        }
                        if ((int)(hv_Exists.TupleNot()) != 0)
                        {
                            throw new HalconException(("DLPreprocessParam needs the parameter: '" + (hv_ParamNames.TupleSelect(
                                hv_I))) + "'");
                        }
                    }
                }
                //
                //Check the keys provided.
                hv_InputKeys.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "keys", new HTuple(), out hv_InputKeys);
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_InputKeys.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    hv_Key.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Key = hv_InputKeys.TupleSelect(
                            hv_I);
                    }
                    hv_Value.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_Key, out hv_Value);
                    //Check that the key is known.
                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_ParamNamesAll, hv_Key, out hv_Indices);
                    if ((int)(new HTuple(hv_Indices.TupleEqual(-1))) != 0)
                    {
                        throw new HalconException(("Unknown key for DLPreprocessParam: '" + (hv_InputKeys.TupleSelect(
                            hv_I))) + "'");

                        hv_DLModelType.Dispose();
                        hv_Exception.Dispose();
                        hv_SupportedModelTypes.Dispose();
                        hv_Index.Dispose();
                        hv_ParamNamesGeneral.Dispose();
                        hv_ParamNamesSegmentation.Dispose();
                        hv_ParamNamesDetectionOptional.Dispose();
                        hv_ParamNamesPreprocessingOptional.Dispose();
                        hv_ParamNamesAll.Dispose();
                        hv_ParamNames.Dispose();
                        hv_KeysExists.Dispose();
                        hv_I.Dispose();
                        hv_Exists.Dispose();
                        hv_InputKeys.Dispose();
                        hv_Key.Dispose();
                        hv_Value.Dispose();
                        hv_Indices.Dispose();
                        hv_ValidValues.Dispose();
                        hv_ValidTypes.Dispose();
                        hv_V.Dispose();
                        hv_T.Dispose();
                        hv_IsInt.Dispose();
                        hv_ValidTypesListing.Dispose();
                        hv_ValidValueListing.Dispose();
                        hv_EmptyStrings.Dispose();
                        hv_ImageRangeMinExists.Dispose();
                        hv_ImageRangeMaxExists.Dispose();
                        hv_ImageRangeMin.Dispose();
                        hv_ImageRangeMax.Dispose();
                        hv_IndexParam.Dispose();
                        hv_SetBackgroundID.Dispose();
                        hv_ClassIDsBackground.Dispose();
                        hv_Intersection.Dispose();
                        hv_IgnoreClassIDs.Dispose();
                        hv_KnownClasses.Dispose();
                        hv_IgnoreClassID.Dispose();
                        hv_OptionalKeysExist.Dispose();
                        hv_InstanceType.Dispose();
                        hv_IgnoreDirection.Dispose();
                        hv_ClassIDsNoOrientation.Dispose();
                        hv_SemTypes.Dispose();

                        return;
                    }
                    //Set expected values and types.
                    hv_ValidValues.Dispose();
                    hv_ValidValues = new HTuple();
                    hv_ValidTypes.Dispose();
                    hv_ValidTypes = new HTuple();
                    if ((int)(new HTuple(hv_Key.TupleEqual("normalization_type"))) != 0)
                    {
                        hv_ValidValues.Dispose();
                        hv_ValidValues = new HTuple();
                        hv_ValidValues[0] = "all_channels";
                        hv_ValidValues[1] = "first_channel";
                        hv_ValidValues[2] = "constant_values";
                        hv_ValidValues[3] = "none";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("domain_handling"))) != 0)
                    {
                        if ((int)(new HTuple(hv_DLModelType.TupleEqual("anomaly_detection"))) != 0)
                        {
                            hv_ValidValues.Dispose();
                            hv_ValidValues = new HTuple();
                            hv_ValidValues[0] = "full_domain";
                            hv_ValidValues[1] = "crop_domain";
                            hv_ValidValues[2] = "keep_domain";
                        }
                        else
                        {
                            hv_ValidValues.Dispose();
                            hv_ValidValues = new HTuple();
                            hv_ValidValues[0] = "full_domain";
                            hv_ValidValues[1] = "crop_domain";
                        }
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("model_type"))) != 0)
                    {
                        hv_ValidValues.Dispose();
                        hv_ValidValues = new HTuple();
                        hv_ValidValues[0] = "anomaly_detection";
                        hv_ValidValues[1] = "classification";
                        hv_ValidValues[2] = "detection";
                        hv_ValidValues[3] = "segmentation";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("set_background_id"))) != 0)
                    {
                        hv_ValidTypes.Dispose();
                        hv_ValidTypes = "int";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("class_ids_background"))) != 0)
                    {
                        hv_ValidTypes.Dispose();
                        hv_ValidTypes = "int";
                    }
                    //Check that type is valid.
                    if ((int)(new HTuple((new HTuple(hv_ValidTypes.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        for (hv_V = 0; (int)hv_V <= (int)((new HTuple(hv_ValidTypes.TupleLength())) - 1); hv_V = (int)hv_V + 1)
                        {
                            hv_T.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_T = hv_ValidTypes.TupleSelect(
                                    hv_V);
                            }
                            if ((int)(new HTuple(hv_T.TupleEqual("int"))) != 0)
                            {
                                hv_IsInt.Dispose();
                                HOperatorSet.TupleIsInt(hv_Value, out hv_IsInt);
                                if ((int)(hv_IsInt.TupleNot()) != 0)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_ValidTypes = ("'" + hv_ValidTypes) + "'";
                                            hv_ValidTypes.Dispose();
                                            hv_ValidTypes = ExpTmpLocalVar_ValidTypes;
                                        }
                                    }
                                    if ((int)(new HTuple((new HTuple(hv_ValidTypes.TupleLength())).TupleLess(
                                        2))) != 0)
                                    {
                                        hv_ValidTypesListing.Dispose();
                                        hv_ValidTypesListing = new HTuple(hv_ValidTypes);
                                    }
                                    else
                                    {
                                        hv_ValidTypesListing.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_ValidTypesListing = ((((hv_ValidTypes.TupleSelectRange(
                                                0, (new HTuple(0)).TupleMax2((new HTuple(hv_ValidTypes.TupleLength()
                                                )) - 2))) + new HTuple(", ")) + (hv_ValidTypes.TupleSelect((new HTuple(hv_ValidTypes.TupleLength()
                                                )) - 1)))).TupleSum();
                                        }
                                    }
                                    throw new HalconException(((((("The value given in the key '" + hv_Key) + "' of DLPreprocessParam is invalid. Valid types are: ") + hv_ValidTypesListing) + ". The given value was '") + hv_Value) + "'.");

                                    hv_DLModelType.Dispose();
                                    hv_Exception.Dispose();
                                    hv_SupportedModelTypes.Dispose();
                                    hv_Index.Dispose();
                                    hv_ParamNamesGeneral.Dispose();
                                    hv_ParamNamesSegmentation.Dispose();
                                    hv_ParamNamesDetectionOptional.Dispose();
                                    hv_ParamNamesPreprocessingOptional.Dispose();
                                    hv_ParamNamesAll.Dispose();
                                    hv_ParamNames.Dispose();
                                    hv_KeysExists.Dispose();
                                    hv_I.Dispose();
                                    hv_Exists.Dispose();
                                    hv_InputKeys.Dispose();
                                    hv_Key.Dispose();
                                    hv_Value.Dispose();
                                    hv_Indices.Dispose();
                                    hv_ValidValues.Dispose();
                                    hv_ValidTypes.Dispose();
                                    hv_V.Dispose();
                                    hv_T.Dispose();
                                    hv_IsInt.Dispose();
                                    hv_ValidTypesListing.Dispose();
                                    hv_ValidValueListing.Dispose();
                                    hv_EmptyStrings.Dispose();
                                    hv_ImageRangeMinExists.Dispose();
                                    hv_ImageRangeMaxExists.Dispose();
                                    hv_ImageRangeMin.Dispose();
                                    hv_ImageRangeMax.Dispose();
                                    hv_IndexParam.Dispose();
                                    hv_SetBackgroundID.Dispose();
                                    hv_ClassIDsBackground.Dispose();
                                    hv_Intersection.Dispose();
                                    hv_IgnoreClassIDs.Dispose();
                                    hv_KnownClasses.Dispose();
                                    hv_IgnoreClassID.Dispose();
                                    hv_OptionalKeysExist.Dispose();
                                    hv_InstanceType.Dispose();
                                    hv_IgnoreDirection.Dispose();
                                    hv_ClassIDsNoOrientation.Dispose();
                                    hv_SemTypes.Dispose();

                                    return;
                                }
                            }
                            else
                            {
                                throw new HalconException("Internal error. Unknown valid type.");
                            }
                        }
                    }
                    //Check that value is valid.
                    if ((int)(new HTuple((new HTuple(hv_ValidValues.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_Index.Dispose();
                        HOperatorSet.TupleFindFirst(hv_ValidValues, hv_Value, out hv_Index);
                        if ((int)(new HTuple(hv_Index.TupleEqual(-1))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_ValidValues = ("'" + hv_ValidValues) + "'";
                                    hv_ValidValues.Dispose();
                                    hv_ValidValues = ExpTmpLocalVar_ValidValues;
                                }
                            }
                            if ((int)(new HTuple((new HTuple(hv_ValidValues.TupleLength())).TupleLess(
                                2))) != 0)
                            {
                                hv_ValidValueListing.Dispose();
                                hv_ValidValueListing = new HTuple(hv_ValidValues);
                            }
                            else
                            {
                                hv_EmptyStrings.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_EmptyStrings = HTuple.TupleGenConst(
                                        (new HTuple(hv_ValidValues.TupleLength())) - 2, "");
                                }
                                hv_ValidValueListing.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_ValidValueListing = ((((hv_ValidValues.TupleSelectRange(
                                        0, (new HTuple(0)).TupleMax2((new HTuple(hv_ValidValues.TupleLength()
                                        )) - 2))) + new HTuple(", ")) + (hv_EmptyStrings.TupleConcat(hv_ValidValues.TupleSelect(
                                        (new HTuple(hv_ValidValues.TupleLength())) - 1))))).TupleSum();
                                }
                            }
                            throw new HalconException(((((("The value given in the key '" + hv_Key) + "' of DLPreprocessParam is invalid. Valid values are: ") + hv_ValidValueListing) + ". The given value was '") + hv_Value) + "'.");
                        }
                    }
                }
                //
                //Check the correct setting of ImageRangeMin and ImageRangeMax.
                if ((int)((new HTuple(hv_DLModelType.TupleEqual("classification"))).TupleOr(
                    new HTuple(hv_DLModelType.TupleEqual("detection")))) != 0)
                {
                    //Check ImageRangeMin and ImageRangeMax.
                    hv_ImageRangeMinExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "image_range_min",
                        out hv_ImageRangeMinExists);
                    hv_ImageRangeMaxExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "image_range_max",
                        out hv_ImageRangeMaxExists);
                    //If they are present, check that they are set correctly.
                    if ((int)(hv_ImageRangeMinExists) != 0)
                    {
                        hv_ImageRangeMin.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                        if ((int)(new HTuple(hv_ImageRangeMin.TupleNotEqual(-127))) != 0)
                        {
                            throw new HalconException(("For model type " + hv_DLModelType) + " ImageRangeMin has to be -127.");
                        }
                    }
                    if ((int)(hv_ImageRangeMaxExists) != 0)
                    {
                        hv_ImageRangeMax.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                        if ((int)(new HTuple(hv_ImageRangeMax.TupleNotEqual(128))) != 0)
                        {
                            throw new HalconException(("For model type " + hv_DLModelType) + " ImageRangeMax has to be 128.");
                        }
                    }
                }
                //
                //Check segmentation specific parameters.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                {
                    //Check if detection specific parameters are set.
                    hv_KeysExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesDetectionOptional,
                        out hv_KeysExists);
                    //If they are present, check that they are [].
                    for (hv_IndexParam = 0; (int)hv_IndexParam <= (int)((new HTuple(hv_ParamNamesDetectionOptional.TupleLength()
                        )) - 1); hv_IndexParam = (int)hv_IndexParam + 1)
                    {
                        if ((int)(hv_KeysExists.TupleSelect(hv_IndexParam)) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Value.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                    hv_IndexParam), out hv_Value);
                            }
                            if ((int)(new HTuple(hv_Value.TupleNotEqual(new HTuple()))) != 0)
                            {
                                throw new HalconException(((("The preprocessing parameter '" + (hv_ParamNamesDetectionOptional.TupleSelect(
                                    hv_IndexParam))) + "' was set to ") + hv_Value) + new HTuple(" but for segmentation it should be set to [], as it is not used for this method."));
                            }
                        }
                    }
                    //Check 'set_background_id'.
                    hv_SetBackgroundID.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "set_background_id", out hv_SetBackgroundID);
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleGreater(
                        1))) != 0)
                    {
                        throw new HalconException("Only one class_id as 'set_background_id' allowed.");
                    }
                    //Check 'class_ids_background'.
                    hv_ClassIDsBackground.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "class_ids_background", out hv_ClassIDsBackground);
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_SetBackgroundID.TupleLength()
                        )).TupleGreater(0))).TupleAnd((new HTuple((new HTuple(hv_ClassIDsBackground.TupleLength()
                        )).TupleGreater(0))).TupleNot()))).TupleOr((new HTuple((new HTuple(hv_ClassIDsBackground.TupleLength()
                        )).TupleGreater(0))).TupleAnd((new HTuple((new HTuple(hv_SetBackgroundID.TupleLength()
                        )).TupleGreater(0))).TupleNot()))) != 0)
                    {
                        throw new HalconException("Both keys 'set_background_id' and 'class_ids_background' are required.");
                    }
                    //Check that 'class_ids_background' and 'set_background_id' are disjoint.
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_Intersection.Dispose();
                        HOperatorSet.TupleIntersection(hv_SetBackgroundID, hv_ClassIDsBackground,
                            out hv_Intersection);
                        if ((int)(new HTuple(hv_Intersection.TupleLength())) != 0)
                        {
                            throw new HalconException("Class IDs in 'set_background_id' and 'class_ids_background' need to be disjoint.");
                        }
                    }
                    //Check 'ignore_class_ids'.
                    hv_IgnoreClassIDs.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", out hv_IgnoreClassIDs);
                    hv_KnownClasses.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_KnownClasses = new HTuple();
                        hv_KnownClasses = hv_KnownClasses.TupleConcat(hv_SetBackgroundID, hv_ClassIDsBackground);
                    }
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_IgnoreClassIDs.TupleLength()
                        )) - 1); hv_I = (int)hv_I + 1)
                    {
                        hv_IgnoreClassID.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IgnoreClassID = hv_IgnoreClassIDs.TupleSelect(
                                hv_I);
                        }
                        hv_Index.Dispose();
                        HOperatorSet.TupleFindFirst(hv_KnownClasses, hv_IgnoreClassID, out hv_Index);
                        if ((int)((new HTuple((new HTuple(hv_Index.TupleLength())).TupleGreater(
                            0))).TupleAnd(new HTuple(hv_Index.TupleNotEqual(-1)))) != 0)
                        {
                            throw new HalconException("The given 'ignore_class_ids' must not be included in the 'class_ids_background' or 'set_background_id'.");
                        }
                    }
                }
                else if ((int)(new HTuple(hv_DLModelType.TupleEqual("detection"))) != 0)
                {
                    //Check if segmentation specific parameters are set.
                    hv_KeysExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesSegmentation,
                        out hv_KeysExists);
                    //If they are present, check that they are [].
                    for (hv_IndexParam = 0; (int)hv_IndexParam <= (int)((new HTuple(hv_ParamNamesSegmentation.TupleLength()
                        )) - 1); hv_IndexParam = (int)hv_IndexParam + 1)
                    {
                        if ((int)(hv_KeysExists.TupleSelect(hv_IndexParam)) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Value.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesSegmentation.TupleSelect(
                                    hv_IndexParam), out hv_Value);
                            }
                            if ((int)(new HTuple(hv_Value.TupleNotEqual(new HTuple()))) != 0)
                            {
                                throw new HalconException(((("The preprocessing parameter '" + (hv_ParamNamesSegmentation.TupleSelect(
                                    hv_IndexParam))) + "' was set to ") + hv_Value) + new HTuple(" but for detection it should be set to [], as it is not used for this method."));
                            }
                        }
                    }
                    //Check optional parameters.
                    hv_OptionalKeysExist.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesDetectionOptional,
                        out hv_OptionalKeysExist);
                    if ((int)(hv_OptionalKeysExist.TupleSelect(0)) != 0)
                    {
                        //Check 'instance_type'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_InstanceType.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                0), out hv_InstanceType);
                        }
                        if ((int)(new HTuple((new HTuple(((new HTuple("rectangle1")).TupleConcat(
                            "rectangle2")).TupleFind(hv_InstanceType))).TupleEqual(-1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'instance_type': " + hv_InstanceType) + new HTuple(", only 'rectangle1' and 'rectangle2' are allowed"));
                        }
                    }
                    hv_OptionalKeysExist.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesDetectionOptional,
                        out hv_OptionalKeysExist);
                    if ((int)(hv_OptionalKeysExist.TupleSelect(1)) != 0)
                    {
                        //Check 'ignore_direction'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IgnoreDirection.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                1), out hv_IgnoreDirection);
                        }
                        if ((int)(new HTuple((new HTuple(((new HTuple(1)).TupleConcat(0)).TupleFind(
                            hv_IgnoreDirection))).TupleEqual(-1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'ignore_direction': " + hv_IgnoreDirection) + new HTuple(", only true and false are allowed"));
                        }
                    }
                    if ((int)(hv_OptionalKeysExist.TupleSelect(2)) != 0)
                    {
                        //Check 'class_ids_no_orientation'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ClassIDsNoOrientation.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                2), out hv_ClassIDsNoOrientation);
                        }
                        hv_SemTypes.Dispose();
                        HOperatorSet.TupleSemTypeElem(hv_ClassIDsNoOrientation, out hv_SemTypes);
                        if ((int)((new HTuple(hv_ClassIDsNoOrientation.TupleNotEqual(new HTuple()))).TupleAnd(
                            new HTuple(((((hv_SemTypes.TupleEqualElem("integer"))).TupleSum())).TupleNotEqual(
                            new HTuple(hv_ClassIDsNoOrientation.TupleLength()))))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'class_ids_no_orientation': " + hv_ClassIDsNoOrientation) + new HTuple(", only integers are allowed"));
                        }
                        else
                        {
                            if ((int)((new HTuple(hv_ClassIDsNoOrientation.TupleNotEqual(new HTuple()))).TupleAnd(
                                new HTuple(((((hv_ClassIDsNoOrientation.TupleGreaterEqualElem(0))).TupleSum()
                                )).TupleNotEqual(new HTuple(hv_ClassIDsNoOrientation.TupleLength()
                                ))))) != 0)
                            {
                                throw new HalconException(("Invalid generic parameter for 'class_ids_no_orientation': " + hv_ClassIDsNoOrientation) + new HTuple(", only non-negative integers are allowed"));
                            }
                        }
                    }
                }
                //

                hv_DLModelType.Dispose();
                hv_Exception.Dispose();
                hv_SupportedModelTypes.Dispose();
                hv_Index.Dispose();
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNamesAll.Dispose();
                hv_ParamNames.Dispose();
                hv_KeysExists.Dispose();
                hv_I.Dispose();
                hv_Exists.Dispose();
                hv_InputKeys.Dispose();
                hv_Key.Dispose();
                hv_Value.Dispose();
                hv_Indices.Dispose();
                hv_ValidValues.Dispose();
                hv_ValidTypes.Dispose();
                hv_V.Dispose();
                hv_T.Dispose();
                hv_IsInt.Dispose();
                hv_ValidTypesListing.Dispose();
                hv_ValidValueListing.Dispose();
                hv_EmptyStrings.Dispose();
                hv_ImageRangeMinExists.Dispose();
                hv_ImageRangeMaxExists.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IndexParam.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassIDsBackground.Dispose();
                hv_Intersection.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_KnownClasses.Dispose();
                hv_IgnoreClassID.Dispose();
                hv_OptionalKeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_SemTypes.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_DLModelType.Dispose();
                hv_Exception.Dispose();
                hv_SupportedModelTypes.Dispose();
                hv_Index.Dispose();
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNamesAll.Dispose();
                hv_ParamNames.Dispose();
                hv_KeysExists.Dispose();
                hv_I.Dispose();
                hv_Exists.Dispose();
                hv_InputKeys.Dispose();
                hv_Key.Dispose();
                hv_Value.Dispose();
                hv_Indices.Dispose();
                hv_ValidValues.Dispose();
                hv_ValidTypes.Dispose();
                hv_V.Dispose();
                hv_T.Dispose();
                hv_IsInt.Dispose();
                hv_ValidTypesListing.Dispose();
                hv_ValidValueListing.Dispose();
                hv_EmptyStrings.Dispose();
                hv_ImageRangeMinExists.Dispose();
                hv_ImageRangeMaxExists.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IndexParam.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassIDsBackground.Dispose();
                hv_Intersection.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_KnownClasses.Dispose();
                hv_IgnoreClassID.Dispose();
                hv_OptionalKeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_SemTypes.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Tools / Geometry
        // Short Description: Convert the parameters of rectangles with format rectangle2 to the coordinates of its 4 corner-points. 
        public void convert_rect2_5to8param(HTuple hv_Row, HTuple hv_Col, HTuple hv_Length1,
            HTuple hv_Length2, HTuple hv_Phi, out HTuple hv_Row1, out HTuple hv_Col1, out HTuple hv_Row2,
            out HTuple hv_Col2, out HTuple hv_Row3, out HTuple hv_Col3, out HTuple hv_Row4,
            out HTuple hv_Col4)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Co1 = new HTuple(), hv_Co2 = new HTuple();
            HTuple hv_Si1 = new HTuple(), hv_Si2 = new HTuple();
            // Initialize local and output iconic variables 
            hv_Row1 = new HTuple();
            hv_Col1 = new HTuple();
            hv_Row2 = new HTuple();
            hv_Col2 = new HTuple();
            hv_Row3 = new HTuple();
            hv_Col3 = new HTuple();
            hv_Row4 = new HTuple();
            hv_Col4 = new HTuple();
            try
            {
                //This procedure takes the parameters for a rectangle of type 'rectangle2'
                //and returns the coordinates of the four corners.
                //
                hv_Co1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Co1 = (hv_Phi.TupleCos()
                        ) * hv_Length1;
                }
                hv_Co2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Co2 = (hv_Phi.TupleCos()
                        ) * hv_Length2;
                }
                hv_Si1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Si1 = (hv_Phi.TupleSin()
                        ) * hv_Length1;
                }
                hv_Si2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Si2 = (hv_Phi.TupleSin()
                        ) * hv_Length2;
                }

                hv_Col1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col1 = (hv_Co1 - hv_Si2) + hv_Col;
                }
                hv_Row1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row1 = ((-hv_Si1) - hv_Co2) + hv_Row;
                }
                hv_Col2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col2 = ((-hv_Co1) - hv_Si2) + hv_Col;
                }
                hv_Row2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row2 = (hv_Si1 - hv_Co2) + hv_Row;
                }
                hv_Col3.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col3 = ((-hv_Co1) + hv_Si2) + hv_Col;
                }
                hv_Row3.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row3 = (hv_Si1 + hv_Co2) + hv_Row;
                }
                hv_Col4.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col4 = (hv_Co1 + hv_Si2) + hv_Col;
                }
                hv_Row4.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row4 = ((-hv_Si1) + hv_Co2) + hv_Row;
                }


                hv_Co1.Dispose();
                hv_Co2.Dispose();
                hv_Si1.Dispose();
                hv_Si2.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Co1.Dispose();
                hv_Co2.Dispose();
                hv_Si1.Dispose();
                hv_Si2.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Tools / Geometry
        // Short Description: Convert for four-sided figures the coordinates of the 4 corner-points to the parameters of format rectangle2. 
        public void convert_rect2_8to5param(HTuple hv_Row1, HTuple hv_Col1, HTuple hv_Row2,
            HTuple hv_Col2, HTuple hv_Row3, HTuple hv_Col3, HTuple hv_Row4, HTuple hv_Col4,
            HTuple hv_ForceL1LargerL2, out HTuple hv_Row, out HTuple hv_Col, out HTuple hv_Length1,
            out HTuple hv_Length2, out HTuple hv_Phi)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Hor = new HTuple(), hv_Vert = new HTuple();
            HTuple hv_IdxSwap = new HTuple(), hv_Tmp = new HTuple();
            // Initialize local and output iconic variables 
            hv_Row = new HTuple();
            hv_Col = new HTuple();
            hv_Length1 = new HTuple();
            hv_Length2 = new HTuple();
            hv_Phi = new HTuple();
            try
            {
                //This procedure takes the corners of four-sided figures
                //and returns the parameters of type 'rectangle2'.
                //
                //Calculate center row and column.
                hv_Row.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row = (((hv_Row1 + hv_Row2) + hv_Row3) + hv_Row4) / 4.0;
                }
                hv_Col.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col = (((hv_Col1 + hv_Col2) + hv_Col3) + hv_Col4) / 4.0;
                }
                //Length1 and Length2.
                hv_Length1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Length1 = (((((hv_Row1 - hv_Row2) * (hv_Row1 - hv_Row2)) + ((hv_Col1 - hv_Col2) * (hv_Col1 - hv_Col2)))).TupleSqrt()
                        ) / 2.0;
                }
                hv_Length2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Length2 = (((((hv_Row2 - hv_Row3) * (hv_Row2 - hv_Row3)) + ((hv_Col2 - hv_Col3) * (hv_Col2 - hv_Col3)))).TupleSqrt()
                        ) / 2.0;
                }
                //Calculate the angle phi.
                hv_Hor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Hor = hv_Col1 - hv_Col2;
                }
                hv_Vert.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Vert = hv_Row2 - hv_Row1;
                }
                if ((int)(hv_ForceL1LargerL2) != 0)
                {
                    //Swap length1 and length2 if necessary.
                    hv_IdxSwap.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_IdxSwap = ((((hv_Length2 - hv_Length1)).TupleGreaterElem(
                            1e-9))).TupleFind(1);
                    }
                    if ((int)(new HTuple(hv_IdxSwap.TupleNotEqual(-1))) != 0)
                    {
                        hv_Tmp.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Tmp = hv_Length1.TupleSelect(
                                hv_IdxSwap);
                        }
                        if (hv_Length1 == null)
                            hv_Length1 = new HTuple();
                        hv_Length1[hv_IdxSwap] = hv_Length2.TupleSelect(hv_IdxSwap);
                        if (hv_Length2 == null)
                            hv_Length2 = new HTuple();
                        hv_Length2[hv_IdxSwap] = hv_Tmp;
                        if (hv_Hor == null)
                            hv_Hor = new HTuple();
                        hv_Hor[hv_IdxSwap] = (hv_Col2.TupleSelect(hv_IdxSwap)) - (hv_Col3.TupleSelect(
                            hv_IdxSwap));
                        if (hv_Vert == null)
                            hv_Vert = new HTuple();
                        hv_Vert[hv_IdxSwap] = (hv_Row3.TupleSelect(hv_IdxSwap)) - (hv_Row2.TupleSelect(
                            hv_IdxSwap));
                    }
                }
                hv_Phi.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Phi = hv_Vert.TupleAtan2(
                        hv_Hor);
                }
                //

                hv_Hor.Dispose();
                hv_Vert.Dispose();
                hv_IdxSwap.Dispose();
                hv_Tmp.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Hor.Dispose();
                hv_Vert.Dispose();
                hv_IdxSwap.Dispose();
                hv_Tmp.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Window
        // Short Description: Close all window handles contained in a dictionary. 
        public void dev_close_window_dict(HTuple hv_WindowHandleDict)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_WindowHandleKeys = new HTuple();
            HTuple hv_Index = new HTuple(), hv_WindowHandles = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_RemovedWindowIndices = new HTuple();
            HTuple hv_WindowHandleIndex = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //
                //This procedure closes all window handles
                //that are contained in the dictionary WindowHandleDict.
                //
                hv_WindowHandleKeys.Dispose();
                HOperatorSet.GetDictParam(hv_WindowHandleDict, "keys", new HTuple(), out hv_WindowHandleKeys);
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_WindowHandleKeys.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    try
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_WindowHandles.Dispose();
                            HOperatorSet.GetDictTuple(hv_WindowHandleDict, hv_WindowHandleKeys.TupleSelect(
                                hv_Index), out hv_WindowHandles);
                        }
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        continue;
                    }
                    hv_RemovedWindowIndices.Dispose();
                    hv_RemovedWindowIndices = new HTuple();
                    for (hv_WindowHandleIndex = 0; (int)hv_WindowHandleIndex <= (int)((new HTuple(hv_WindowHandles.TupleLength()
                        )) - 1); hv_WindowHandleIndex = (int)hv_WindowHandleIndex + 1)
                    {
                        //Not every entry has to be a window handle, therefore use try-catch.
                        try
                        {
                            //Call set_window_param to check if the handle is a window handle.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetWindowParam(hv_WindowHandles.TupleSelect(hv_WindowHandleIndex),
                                    "flush", "false");
                            }
                            HDevWindowStack.SetActive(hv_WindowHandles.TupleSelect(
                                hv_WindowHandleIndex));
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.CloseWindow(HDevWindowStack.Pop());
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_RemovedWindowIndices = hv_RemovedWindowIndices.TupleConcat(
                                        hv_WindowHandleIndex);
                                    hv_RemovedWindowIndices.Dispose();
                                    hv_RemovedWindowIndices = ExpTmpLocalVar_RemovedWindowIndices;
                                }
                            }
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException1)
                        {
                            HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        }
                    }
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleRemove(hv_WindowHandles, hv_RemovedWindowIndices, out ExpTmpOutVar_0);
                        hv_WindowHandles.Dispose();
                        hv_WindowHandles = ExpTmpOutVar_0;
                    }
                    //If some entries remained, set reduced tuple. Otherwise, remove whole key entry.
                    if ((int)(new HTuple((new HTuple(hv_WindowHandles.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_WindowHandleDict, hv_WindowHandleKeys.TupleSelect(
                                hv_Index), hv_WindowHandles);
                        }
                    }
                    else
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.RemoveDictKey(hv_WindowHandleDict, hv_WindowHandleKeys.TupleSelect(
                                hv_Index));
                        }
                    }
                }
                //

                hv_WindowHandleKeys.Dispose();
                hv_Index.Dispose();
                hv_WindowHandles.Dispose();
                hv_Exception.Dispose();
                hv_RemovedWindowIndices.Dispose();
                hv_WindowHandleIndex.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_WindowHandleKeys.Dispose();
                hv_Index.Dispose();
                hv_WindowHandles.Dispose();
                hv_Exception.Dispose();
                hv_RemovedWindowIndices.Dispose();
                hv_WindowHandleIndex.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Output
        // Short Description: Display a map of the confidences. 
        public void dev_display_confidence_regions(HObject ho_ImageConfidence, HTuple hv_DrawTransparency,
            out HTuple hv_Colors)
        {




            // Local iconic variables 

            HObject ho_Region = null;

            // Local control variables 

            HTuple hv_NumColors = new HTuple(), hv_WeightsColorsAlpha = new HTuple();
            HTuple hv_ColorIndex = new HTuple(), hv_Threshold = new HTuple();
            HTuple hv_MinGray = new HTuple(), hv_MaxGray = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Region);
            hv_Colors = new HTuple();
            try
            {
                //
                //This procedure displays a map of the confidences
                //given in ImageConfidence as regions.
                //DrawTransparency determines the alpha value of the colors.
                //The used colors are returned.
                //
                //Define colors.
                hv_NumColors.Dispose();
                hv_NumColors = 20;
                hv_Colors.Dispose();
                get_distinct_colors(hv_NumColors, 0, 0, 100, out hv_Colors);
                hv_WeightsColorsAlpha.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WeightsColorsAlpha = hv_Colors + hv_DrawTransparency;
                }
                hv_ColorIndex.Dispose();
                hv_ColorIndex = 0;
                //
                //Threshold the image according to
                //the number of colors and
                //display resulting regions.
                HTuple end_val15 = hv_NumColors - 1;
                HTuple step_val15 = 1;
                for (hv_ColorIndex = 0; hv_ColorIndex.Continue(end_val15, step_val15); hv_ColorIndex = hv_ColorIndex.TupleAdd(step_val15))
                {
                    hv_Threshold.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Threshold = hv_ColorIndex * (1.0 / hv_NumColors);
                    }
                    hv_MinGray.Dispose();
                    hv_MinGray = new HTuple(hv_Threshold);
                    hv_MaxGray.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaxGray = hv_Threshold + (1 / hv_NumColors);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Region.Dispose();
                        HOperatorSet.Threshold(ho_ImageConfidence, out ho_Region, hv_Threshold, hv_Threshold + (1.0 / hv_NumColors));
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_WeightsColorsAlpha.TupleSelect(
                                hv_ColorIndex));
                        }
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_Region, HDevWindowStack.GetActive());
                    }
                }
                ho_Region.Dispose();

                hv_NumColors.Dispose();
                hv_WeightsColorsAlpha.Dispose();
                hv_ColorIndex.Dispose();
                hv_Threshold.Dispose();
                hv_MinGray.Dispose();
                hv_MaxGray.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Region.Dispose();

                hv_NumColors.Dispose();
                hv_WeightsColorsAlpha.Dispose();
                hv_ColorIndex.Dispose();
                hv_Threshold.Dispose();
                hv_MinGray.Dispose();
                hv_MaxGray.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Visualize different images, annotations and inference results for a sample. 
        public void dev_display_dl_data(HTuple hv_DLSample, HTuple hv_DLResult, HTuple hv_DLDatasetInfo,
            HTuple hv_KeysForDisplay, HTuple hv_GenParam, HTuple hv_WindowHandleDict)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Image = null, ho_AnomalyImage = null;
            HObject ho_AnomalyRegion = null, ho_PredictionColorFrame = null;
            HObject ho_ImageHeatmap = null, ho_ImageWeight = null, ho_ImageConfidence = null;
            HObject ho_SegmentationImagGroundTruth = null, ho_SegmentationImageResult = null;
            HObject ho_ImageAbsDiff = null, ho_DiffRegion = null;

            // Local control variables 

            HTuple hv_ThresholdWidth = new HTuple(), hv_ScaleWindows = new HTuple();
            HTuple hv_Font = new HTuple(), hv_FontSize = new HTuple();
            HTuple hv_LineWidth = new HTuple(), hv_MapTransparency = new HTuple();
            HTuple hv_MapColorBarWidth = new HTuple(), hv_AnomalyRegionThreshold = new HTuple();
            HTuple hv_AnomalyClassificationThreshold = new HTuple();
            HTuple hv_AnomalyRegionLabelColor = new HTuple(), hv_AnomalyColorTransparency = new HTuple();
            HTuple hv_AnomalyRegionResultColor = new HTuple(), hv_SegMaxWeight = new HTuple();
            HTuple hv_SegDraw = new HTuple(), hv_SegTransparency = new HTuple();
            HTuple hv_SegExcludeClassIDs = new HTuple(), hv_BboxLabelColor = new HTuple();
            HTuple hv_BboxDisplayConfidence = new HTuple(), hv_BboxTextColor = new HTuple();
            HTuple hv_ShowBottomDesc = new HTuple(), hv_ShowLegend = new HTuple();
            HTuple hv_ShowGroundTruthAnomalyRegions = new HTuple();
            HTuple hv_ShowClassificationIDs = new HTuple(), hv_ShowClassificationColorFrame = new HTuple();
            HTuple hv_ShowLabels = new HTuple(), hv_ShowDirection = new HTuple();
            HTuple hv_HeatmapColorScheme = new HTuple(), hv_GenParamNames = new HTuple();
            HTuple hv_ParamIndex = new HTuple(), hv_GenParamName = new HTuple();
            HTuple hv_GenParamValue = new HTuple(), hv_SampleKeys = new HTuple();
            HTuple hv_ResultKeys = new HTuple(), hv_ImageIDExists = new HTuple();
            HTuple hv_ImageID = new HTuple(), hv_ImageIDString = new HTuple();
            HTuple hv_ImageIDStringBraces = new HTuple(), hv_ImageIDStringCapital = new HTuple();
            HTuple hv_NeededKeys = new HTuple(), hv_Index = new HTuple();
            HTuple hv_DLDatasetInfoKeys = new HTuple(), hv_ClassNames = new HTuple();
            HTuple hv_ClassIDs = new HTuple(), hv_Colors = new HTuple();
            HTuple hv_ClassesLegend = new HTuple(), hv_PrevWindowCoordinates = new HTuple();
            HTuple hv_Keys = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_MetaInfoIndex = new HTuple(), hv_MetaInfo = new HTuple();
            HTuple hv_FlushValues = new HTuple(), hv_WindowHandleKeys = new HTuple();
            HTuple hv_KeyIndex = new HTuple(), hv_WindowHandles = new HTuple();
            HTuple hv_WindowIndex = new HTuple(), hv_FlushValue = new HTuple();
            HTuple hv_WidthImage = new HTuple(), hv_HeightImage = new HTuple();
            HTuple hv_CurrentWindowHandle = new HTuple(), hv_WindowImageRatio = new HTuple();
            HTuple hv_AnomalyLabelGroundTruth = new HTuple(), hv_AnomalyLabelIDGroundTruth = new HTuple();
            HTuple hv_AnomalyRegionExists = new HTuple(), hv_Text = new HTuple();
            HTuple hv_AnomalyScore = new HTuple(), hv_AnomalyClassID = new HTuple();
            HTuple hv_AnomalyRegionGroundTruthExists = new HTuple();
            HTuple hv_PredictionColor = new HTuple(), hv_LineColors = new HTuple();
            HTuple hv_ClassificationLabelIDGroundTruth = new HTuple();
            HTuple hv_ClassificationLabelIDResult = new HTuple(), hv_MarginBottom = new HTuple();
            HTuple hv_WindowCoordinates = new HTuple(), hv_CurrentWindowHeight = new HTuple();
            HTuple hv__ = new HTuple(), hv_MaxHeight = new HTuple();
            HTuple hv_PredictionText = new HTuple(), hv_BoarderOffset = new HTuple();
            HTuple hv_WindowImageRatioHeight = new HTuple(), hv_WindowImageRatioWidth = new HTuple();
            HTuple hv_BoarderOffsetRow = new HTuple(), hv_BoarderOffsetCol = new HTuple();
            HTuple hv_SelectedHeatmapMethod = new HTuple(), hv_DictHeatmap = new HTuple();
            HTuple hv_MethodName = new HTuple(), hv_HeatmapKeys = new HTuple();
            HTuple hv_HeatmapImageName = new HTuple(), hv_TargetClassID = new HTuple();
            HTuple hv_Confidences = new HTuple(), hv_MaxDeviation = new HTuple();
            HTuple hv_ClassificationLabelNameResult = new HTuple();
            HTuple hv_TargetClassConfidence = new HTuple(), hv_ClassificationLabelNamesGroundTruth = new HTuple();
            HTuple hv_BboxIDs = new HTuple(), hv_BboxColors = new HTuple();
            HTuple hv_BboxIDsUniq = new HTuple(), hv_BboxConfidences = new HTuple();
            HTuple hv_TextConf = new HTuple(), hv_BboxClassIndex = new HTuple();
            HTuple hv_BboxColorsResults = new HTuple(), hv_BboxClassIndexUniq = new HTuple();
            HTuple hv_BboxLabelIndex = new HTuple(), hv_BboxColorsBoth = new HTuple();
            HTuple hv_BboxClassLabelIndexUniq = new HTuple(), hv_ColorsSegmentation = new HTuple();
            HTuple hv_DrawMode = new HTuple(), hv_Width = new HTuple();
            HTuple hv_ImageClassIDs = new HTuple(), hv_ImageClassIDsUniq = new HTuple();
            HTuple hv_ColorsResults = new HTuple(), hv_GroundTruthIDs = new HTuple();
            HTuple hv_ResultIDs = new HTuple(), hv_StringSegExcludeClassIDs = new HTuple();
            HTuple hv_StringIndex = new HTuple(), hv_Min = new HTuple();
            HTuple hv_Max = new HTuple(), hv_Range = new HTuple();
            HTuple hv_MinWeight = new HTuple(), hv_WeightsColors = new HTuple();
            HTuple hv_ConfidenceColors = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_WindowHandleKeysNew = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_AnomalyImage);
            HOperatorSet.GenEmptyObj(out ho_AnomalyRegion);
            HOperatorSet.GenEmptyObj(out ho_PredictionColorFrame);
            HOperatorSet.GenEmptyObj(out ho_ImageHeatmap);
            HOperatorSet.GenEmptyObj(out ho_ImageWeight);
            HOperatorSet.GenEmptyObj(out ho_ImageConfidence);
            HOperatorSet.GenEmptyObj(out ho_SegmentationImagGroundTruth);
            HOperatorSet.GenEmptyObj(out ho_SegmentationImageResult);
            HOperatorSet.GenEmptyObj(out ho_ImageAbsDiff);
            HOperatorSet.GenEmptyObj(out ho_DiffRegion);
            try
            {
                //
                //This procedure displays the content of the provided DLSample and/or DLResult
                //depending on the input string KeysForDisplay.
                //DLDatasetInfo is a dictionary containing the information about the dataset.
                //The visualization can be adapted with GenParam.
                //
                //** Set the default values: ***
                //
                //Define the screen width when a new window row is started.
                hv_ThresholdWidth.Dispose();
                hv_ThresholdWidth = 1024;
                //Since potentially a lot of windows are opened,
                //scale the windows consistently.
                hv_ScaleWindows.Dispose();
                hv_ScaleWindows = 0.8;
                //Set a font and a font size.
                hv_Font.Dispose();
                hv_Font = "mono";
                hv_FontSize.Dispose();
                hv_FontSize = 14;
                //
                hv_LineWidth.Dispose();
                hv_LineWidth = 2;
                hv_MapTransparency.Dispose();
                hv_MapTransparency = "cc";
                hv_MapColorBarWidth.Dispose();
                hv_MapColorBarWidth = 140;
                //
                //Define anomaly detection-specific parameter values.
                hv_AnomalyRegionThreshold.Dispose();
                hv_AnomalyRegionThreshold = -1;
                hv_AnomalyClassificationThreshold.Dispose();
                hv_AnomalyClassificationThreshold = -1;
                hv_AnomalyRegionLabelColor.Dispose();
                hv_AnomalyRegionLabelColor = "#40e0d0";
                hv_AnomalyColorTransparency.Dispose();
                hv_AnomalyColorTransparency = "40";
                hv_AnomalyRegionResultColor.Dispose();
                hv_AnomalyRegionResultColor = "#ff0000c0";
                //
                //Define segmentation-specific parameter values.
                hv_SegMaxWeight.Dispose();
                hv_SegMaxWeight = 0;
                hv_SegDraw.Dispose();
                hv_SegDraw = "fill";
                hv_SegTransparency.Dispose();
                hv_SegTransparency = "aa";
                hv_SegExcludeClassIDs.Dispose();
                hv_SegExcludeClassIDs = new HTuple();
                //
                //Define bounding box-specific parameter values.
                hv_BboxLabelColor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_BboxLabelColor = new HTuple("#000000") + "99";
                }
                hv_BboxDisplayConfidence.Dispose();
                hv_BboxDisplayConfidence = 1;
                hv_BboxTextColor.Dispose();
                hv_BboxTextColor = "#eeeeee";
                //
                //By default, display a description on the bottom.
                hv_ShowBottomDesc.Dispose();
                hv_ShowBottomDesc = 1;
                //
                //By default, show a legend with class IDs.
                hv_ShowLegend.Dispose();
                hv_ShowLegend = 1;
                //
                //By default, show the anomaly ground truth regions.
                hv_ShowGroundTruthAnomalyRegions.Dispose();
                hv_ShowGroundTruthAnomalyRegions = 1;
                //
                //By default, show class IDs and color frames for classification ground truth/results.
                hv_ShowClassificationIDs.Dispose();
                hv_ShowClassificationIDs = 1;
                hv_ShowClassificationColorFrame.Dispose();
                hv_ShowClassificationColorFrame = 1;
                //
                //By default, show class labels for detection ground truth/results.
                hv_ShowLabels.Dispose();
                hv_ShowLabels = 1;
                //
                //By default, show direction of the ground truth/results instances for detection with instance_type 'rectangle2'.
                hv_ShowDirection.Dispose();
                hv_ShowDirection = 1;
                //
                //By default, use color scheme 'Jet' for the heatmap display.
                hv_HeatmapColorScheme.Dispose();
                hv_HeatmapColorScheme = "jet";
                //** Set user defined values: ***
                //
                //Overwrite default values by given generic parameters.
                if ((int)(new HTuple(hv_GenParam.TupleNotEqual(new HTuple()))) != 0)
                {
                    hv_GenParamNames.Dispose();
                    HOperatorSet.GetDictParam(hv_GenParam, "keys", new HTuple(), out hv_GenParamNames);
                    for (hv_ParamIndex = 0; (int)hv_ParamIndex <= (int)((new HTuple(hv_GenParamNames.TupleLength()
                        )) - 1); hv_ParamIndex = (int)hv_ParamIndex + 1)
                    {
                        hv_GenParamName.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_GenParamName = hv_GenParamNames.TupleSelect(
                                hv_ParamIndex);
                        }
                        hv_GenParamValue.Dispose();
                        HOperatorSet.GetDictTuple(hv_GenParam, hv_GenParamName, out hv_GenParamValue);
                        if ((int)(new HTuple(hv_GenParamName.TupleEqual("threshold_width"))) != 0)
                        {
                            hv_ThresholdWidth.Dispose();
                            hv_ThresholdWidth = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("scale_windows"))) != 0)
                        {
                            hv_ScaleWindows.Dispose();
                            hv_ScaleWindows = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("font"))) != 0)
                        {
                            hv_Font.Dispose();
                            hv_Font = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("font_size"))) != 0)
                        {
                            hv_FontSize.Dispose();
                            hv_FontSize = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("line_width"))) != 0)
                        {
                            hv_LineWidth.Dispose();
                            hv_LineWidth = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("map_transparency"))) != 0)
                        {
                            hv_MapTransparency.Dispose();
                            hv_MapTransparency = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("map_color_bar_width"))) != 0)
                        {
                            hv_MapColorBarWidth.Dispose();
                            hv_MapColorBarWidth = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("segmentation_max_weight"))) != 0)
                        {
                            hv_SegMaxWeight.Dispose();
                            hv_SegMaxWeight = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("segmentation_draw"))) != 0)
                        {
                            hv_SegDraw.Dispose();
                            hv_SegDraw = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("segmentation_transparency"))) != 0)
                        {
                            hv_SegTransparency.Dispose();
                            hv_SegTransparency = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("segmentation_exclude_class_ids"))) != 0)
                        {
                            hv_SegExcludeClassIDs.Dispose();
                            hv_SegExcludeClassIDs = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("bbox_label_color"))) != 0)
                        {
                            hv_BboxLabelColor.Dispose();
                            hv_BboxLabelColor = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("bbox_display_confidence"))) != 0)
                        {
                            hv_BboxDisplayConfidence.Dispose();
                            hv_BboxDisplayConfidence = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("bbox_text_color"))) != 0)
                        {
                            hv_BboxTextColor.Dispose();
                            hv_BboxTextColor = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("display_bottom_desc"))) != 0)
                        {
                            hv_ShowBottomDesc.Dispose();
                            hv_ShowBottomDesc = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("display_legend"))) != 0)
                        {
                            hv_ShowLegend.Dispose();
                            hv_ShowLegend = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("display_classification_ids"))) != 0)
                        {
                            hv_ShowClassificationIDs.Dispose();
                            hv_ShowClassificationIDs = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("display_classification_color_frame"))) != 0)
                        {
                            hv_ShowClassificationColorFrame.Dispose();
                            hv_ShowClassificationColorFrame = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("display_labels"))) != 0)
                        {
                            hv_ShowLabels.Dispose();
                            hv_ShowLabels = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("display_direction"))) != 0)
                        {
                            hv_ShowDirection.Dispose();
                            hv_ShowDirection = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("heatmap_color_scheme"))) != 0)
                        {
                            hv_HeatmapColorScheme.Dispose();
                            hv_HeatmapColorScheme = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("display_ground_truth_anomaly_regions"))) != 0)
                        {
                            hv_ShowGroundTruthAnomalyRegions.Dispose();
                            hv_ShowGroundTruthAnomalyRegions = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("anomaly_region_threshold"))) != 0)
                        {
                            hv_AnomalyRegionThreshold.Dispose();
                            hv_AnomalyRegionThreshold = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("anomaly_classification_threshold"))) != 0)
                        {
                            hv_AnomalyClassificationThreshold.Dispose();
                            hv_AnomalyClassificationThreshold = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("anomaly_region_label_color"))) != 0)
                        {
                            hv_AnomalyRegionLabelColor.Dispose();
                            hv_AnomalyRegionLabelColor = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("anomaly_region_result_color"))) != 0)
                        {
                            hv_AnomalyRegionResultColor.Dispose();
                            hv_AnomalyRegionResultColor = new HTuple(hv_GenParamValue);
                        }
                        else if ((int)(new HTuple(hv_GenParamName.TupleEqual("anomaly_color_transparency"))) != 0)
                        {
                            hv_AnomalyColorTransparency.Dispose();
                            hv_AnomalyColorTransparency = new HTuple(hv_GenParamValue);
                        }
                        else
                        {
                            throw new HalconException(("Unknown generic parameter: " + hv_GenParamName) + ".");
                        }
                    }
                }
                //
                //Get the dictionary keys.
                hv_SampleKeys.Dispose();
                HOperatorSet.GetDictParam(hv_DLSample, "keys", new HTuple(), out hv_SampleKeys);
                if ((int)(new HTuple(hv_DLResult.TupleNotEqual(new HTuple()))) != 0)
                {
                    hv_ResultKeys.Dispose();
                    HOperatorSet.GetDictParam(hv_DLResult, "keys", new HTuple(), out hv_ResultKeys);
                }
                //
                //Get image ID if it is available.
                hv_ImageIDExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "image_id", out hv_ImageIDExists);
                if ((int)(hv_ImageIDExists) != 0)
                {
                    hv_ImageID.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageID);
                    hv_ImageIDString.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageIDString = "image ID " + hv_ImageID;
                    }
                    hv_ImageIDStringBraces.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageIDStringBraces = ("(image ID " + hv_ImageID) + ")";
                    }
                    hv_ImageIDStringCapital.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageIDStringCapital = "Image ID " + hv_ImageID;
                    }
                }
                else
                {
                    hv_ImageIDString.Dispose();
                    hv_ImageIDString = "";
                    hv_ImageIDStringBraces.Dispose();
                    hv_ImageIDStringBraces = new HTuple(hv_ImageIDString);
                    hv_ImageIDStringCapital.Dispose();
                    hv_ImageIDStringCapital = new HTuple(hv_ImageIDString);
                }
                //
                //Check if DLDatasetInfo is valid.
                if ((int)(new HTuple(hv_DLDatasetInfo.TupleEqual(new HTuple()))) != 0)
                {
                    //If DLDatasetInfo is empty, 'image' is the only key allowed in KeysForDisplay.
                    if ((int)((new HTuple((new HTuple(hv_KeysForDisplay.TupleLength())).TupleNotEqual(
                        1))).TupleOr(new HTuple(((hv_KeysForDisplay.TupleSelect(0))).TupleNotEqual(
                        "image")))) != 0)
                    {
                        throw new HalconException("DLDatasetInfo is needed for requested keys in KeysForDisplay.");
                    }
                }
                else
                {
                    //Check if DLDatasetInfo contains necessary keys.
                    hv_NeededKeys.Dispose();
                    hv_NeededKeys = new HTuple();
                    hv_NeededKeys[0] = "class_names";
                    hv_NeededKeys[1] = "class_ids";
                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_NeededKeys.TupleLength()
                        )) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        hv_DLDatasetInfoKeys.Dispose();
                        HOperatorSet.GetDictParam(hv_DLDatasetInfo, "keys", new HTuple(), out hv_DLDatasetInfoKeys);
                        if ((int)(new HTuple(((hv_DLDatasetInfoKeys.TupleFindFirst(hv_NeededKeys.TupleSelect(
                            hv_Index)))).TupleEqual(-1))) != 0)
                        {
                            throw new HalconException(("Key " + (hv_NeededKeys.TupleSelect(
                                hv_Index))) + " is missing in DLDatasetInfo.");
                        }
                    }
                    //
                    //Get the general dataset information, if available.
                    hv_ClassNames.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLDatasetInfo, "class_names", out hv_ClassNames);
                    hv_ClassIDs.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLDatasetInfo, "class_ids", out hv_ClassIDs);
                    //
                    //Define distinct colors for the classes.
                    hv_Colors.Dispose();
                    get_dl_class_colors(hv_ClassNames, out hv_Colors);
                    //
                    hv_ClassesLegend.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ClassesLegend = (hv_ClassIDs + " : ") + hv_ClassNames;
                    }
                }
                //
                //** Set window parameters: ***
                //
                //Set previous window coordinates.
                hv_PrevWindowCoordinates.Dispose();
                hv_PrevWindowCoordinates = new HTuple();
                hv_PrevWindowCoordinates[0] = 0;
                hv_PrevWindowCoordinates[1] = 0;
                hv_PrevWindowCoordinates[2] = 0;
                hv_PrevWindowCoordinates[3] = 0;
                hv_PrevWindowCoordinates[4] = 1;
                //
                //Check that the WindowHandleDict is of type dictionary.
                try
                {
                    hv_Keys.Dispose();
                    HOperatorSet.GetDictParam(hv_WindowHandleDict, "keys", new HTuple(), out hv_Keys);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    if ((int)(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1401))) != 0)
                    {
                        throw new HalconException("WindowHandleDict has to be of type dictionary. Use create_dict to create an empty dictionary.");
                    }
                    else
                    {
                        throw new HalconException(hv_Exception);
                    }
                }
                //For better usage, add meta information about the window handles in WindowHandleDict.
                hv_MetaInfoIndex.Dispose();
                HOperatorSet.TupleFind(hv_Keys, "meta_information", out hv_MetaInfoIndex);
                if ((int)((new HTuple(hv_MetaInfoIndex.TupleEqual(-1))).TupleOr(new HTuple(hv_MetaInfoIndex.TupleEqual(
                    new HTuple())))) != 0)
                {
                    hv_MetaInfo.Dispose();
                    HOperatorSet.CreateDict(out hv_MetaInfo);
                    HOperatorSet.SetDictTuple(hv_WindowHandleDict, "meta_information", hv_MetaInfo);
                }
                //
                //For each window, set 'flush' to 'false' to avoid flickering.
                hv_FlushValues.Dispose();
                hv_FlushValues = new HTuple();
                hv_WindowHandleKeys.Dispose();
                HOperatorSet.GetDictParam(hv_WindowHandleDict, "keys", new HTuple(), out hv_WindowHandleKeys);
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_WindowHandleKeys.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    //Only consider the WindowHandleKeys that are needed for the current visualization.
                    hv_KeyIndex.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_KeyIndex = hv_KeysForDisplay.TupleFind(
                            hv_WindowHandleKeys.TupleSelect(hv_Index));
                    }
                    if ((int)((new HTuple(hv_KeyIndex.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_KeyIndex.TupleNotEqual(
                        new HTuple())))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_WindowHandles.Dispose();
                            HOperatorSet.GetDictTuple(hv_WindowHandleDict, hv_WindowHandleKeys.TupleSelect(
                                hv_Index), out hv_WindowHandles);
                        }
                        for (hv_WindowIndex = 0; (int)hv_WindowIndex <= (int)((new HTuple(hv_WindowHandles.TupleLength()
                            )) - 1); hv_WindowIndex = (int)hv_WindowIndex + 1)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_FlushValue.Dispose();
                                HOperatorSet.GetWindowParam(hv_WindowHandles.TupleSelect(hv_WindowIndex),
                                    "flush", out hv_FlushValue);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_FlushValues = hv_FlushValues.TupleConcat(
                                        hv_FlushValue);
                                    hv_FlushValues.Dispose();
                                    hv_FlushValues = ExpTmpLocalVar_FlushValues;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetWindowParam(hv_WindowHandles.TupleSelect(hv_WindowIndex),
                                    "flush", "false");
                            }
                        }
                    }
                }
                //
                //** Display the data: ***
                //
                //Display data dictionaries.
                for (hv_KeyIndex = 0; (int)hv_KeyIndex <= (int)((new HTuple(hv_KeysForDisplay.TupleLength()
                    )) - 1); hv_KeyIndex = (int)hv_KeyIndex + 1)
                {
                    if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "image"))) != 0)
                    {
                        //
                        //Image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_ImageIDStringCapital,
                                    "window", "bottom", "left", "white", "box", "false");
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "anomaly_ground_truth"))) != 0)
                    {
                        //Image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        hv_AnomalyLabelGroundTruth.Dispose(); hv_AnomalyLabelIDGroundTruth.Dispose();
                        get_anomaly_ground_truth_label(hv_SampleKeys, hv_DLSample, out hv_AnomalyLabelGroundTruth,
                            out hv_AnomalyLabelIDGroundTruth);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualize image.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        //
                        hv_AnomalyRegionExists.Dispose();
                        hv_AnomalyRegionExists = "false";
                        if ((int)(hv_ShowGroundTruthAnomalyRegions) != 0)
                        {
                            //Show the ground truth region.
                            hv_AnomalyRegionExists.Dispose();
                            dev_display_ground_truth_anomaly_regions(hv_SampleKeys, hv_DLSample,
                                hv_CurrentWindowHandle, hv_LineWidth, hv_AnomalyRegionLabelColor,
                                hv_AnomalyColorTransparency, out hv_AnomalyRegionExists);
                        }
                        //
                        hv_Text.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Text = "Ground truth anomalies " + hv_ImageIDStringBraces;
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = ((hv_AnomalyLabelIDGroundTruth + " : '") + hv_AnomalyLabelGroundTruth) + "'";
                            if ((int)((new HTuple((new HTuple(hv_AnomalyRegionExists.TupleEqual("false"))).TupleAnd(
                                new HTuple(hv_AnomalyLabelIDGroundTruth.TupleEqual(1))))).TupleAnd(
                                hv_ShowGroundTruthAnomalyRegions)) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "No 'anomaly_ground_truth' exists!";
                            }
                            //
                            //Get or open next child window
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "top", "left", "white", "box", "false");
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "anomaly_result"))) != 0)
                    {
                        //
                        //Get image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        //
                        //Get the anomaly image out of DLResult.
                        ho_AnomalyImage.Dispose(); hv_AnomalyScore.Dispose(); hv_AnomalyClassID.Dispose();
                        get_anomaly_result(out ho_AnomalyImage, hv_ResultKeys, hv_DLResult, out hv_AnomalyScore,
                            out hv_AnomalyClassID);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualize image.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        //
                        //Display predicted anomaly regions.
                        if ((int)(new HTuple(hv_AnomalyRegionThreshold.TupleNotEqual(-1))) != 0)
                        {
                            ho_AnomalyRegion.Dispose();
                            dev_display_result_anomaly_regions(ho_AnomalyImage, out ho_AnomalyRegion,
                                hv_CurrentWindowHandle, hv_AnomalyRegionThreshold, hv_LineWidth,
                                hv_AnomalyRegionResultColor);
                        }
                        //
                        hv_Text.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Text = "Detected anomalies " + hv_ImageIDStringBraces;
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "---------------";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Results ";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "---------------";
                            if ((int)(new HTuple(hv_AnomalyClassID.TupleEqual(1))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_AnomalyClassID + " : 'nok'";
                            }
                            else
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_AnomalyClassID + " : 'ok'";
                            }
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Anomaly score: " + (hv_AnomalyScore.TupleString(
                                ".2f"));
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if ((int)((new HTuple(hv_AnomalyClassificationThreshold.TupleNotEqual(
                                -1))).TupleOr(new HTuple(hv_AnomalyRegionThreshold.TupleNotEqual(
                                -1)))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "---------------";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "Thresholds ";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "---------------";
                            }
                            //
                            if ((int)(new HTuple(hv_AnomalyClassificationThreshold.TupleNotEqual(
                                -1))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "Classification: " + (hv_AnomalyClassificationThreshold.TupleString(
                                    ".2f"));
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            }
                            if ((int)(new HTuple(hv_AnomalyRegionThreshold.TupleNotEqual(-1))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "Segmentation: " + (hv_AnomalyRegionThreshold.TupleString(
                                    ".2f"));
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            }
                            //
                            //Get or open next child window
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "top", "left", "white", "box", "false");
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "anomaly_both"))) != 0)
                    {
                        //
                        //Get image, ground truth and results.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        hv_AnomalyLabelGroundTruth.Dispose(); hv_AnomalyLabelIDGroundTruth.Dispose();
                        get_anomaly_ground_truth_label(hv_SampleKeys, hv_DLSample, out hv_AnomalyLabelGroundTruth,
                            out hv_AnomalyLabelIDGroundTruth);
                        ho_AnomalyImage.Dispose(); hv_AnomalyScore.Dispose(); hv_AnomalyClassID.Dispose();
                        get_anomaly_result(out ho_AnomalyImage, hv_ResultKeys, hv_DLResult, out hv_AnomalyScore,
                            out hv_AnomalyClassID);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualize image, ground truth (if available), and result regions.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        hv_AnomalyRegionGroundTruthExists.Dispose();
                        hv_AnomalyRegionGroundTruthExists = "false";
                        if ((int)(hv_ShowGroundTruthAnomalyRegions) != 0)
                        {
                            hv_AnomalyRegionGroundTruthExists.Dispose();
                            dev_display_ground_truth_anomaly_regions(hv_SampleKeys, hv_DLSample,
                                hv_CurrentWindowHandle, hv_LineWidth, hv_AnomalyRegionLabelColor,
                                hv_AnomalyColorTransparency, out hv_AnomalyRegionGroundTruthExists);
                        }
                        //
                        //Display result anomaly regions.
                        ho_AnomalyRegion.Dispose();
                        dev_display_result_anomaly_regions(ho_AnomalyImage, out ho_AnomalyRegion,
                            hv_CurrentWindowHandle, hv_AnomalyRegionThreshold, hv_LineWidth, hv_AnomalyRegionResultColor);
                        //
                        hv_Text.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Text = "GT and detected anomalies " + hv_ImageIDStringBraces;
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Get prediction color.
                        hv_PredictionColor.Dispose();
                        hv_PredictionColor = "white";
                        if ((int)(new HTuple(hv_AnomalyLabelIDGroundTruth.TupleEqual(hv_AnomalyClassID))) != 0)
                        {
                            hv_PredictionColor.Dispose();
                            hv_PredictionColor = "green";
                        }
                        else
                        {
                            hv_PredictionColor.Dispose();
                            hv_PredictionColor = "red";
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "---------------";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Ground truth ";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "---------------";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = ((hv_AnomalyLabelIDGroundTruth + " : '") + hv_AnomalyLabelGroundTruth) + "'";
                            if ((int)((new HTuple((new HTuple(hv_AnomalyRegionGroundTruthExists.TupleEqual(
                                "false"))).TupleAnd(new HTuple(hv_AnomalyLabelIDGroundTruth.TupleEqual(
                                1))))).TupleAnd(hv_ShowGroundTruthAnomalyRegions)) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = " No 'anomaly_ground_truth' exists!";
                            }
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "---------------";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Results ";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "---------------";
                            if ((int)(new HTuple(hv_AnomalyClassID.TupleEqual(1))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_AnomalyClassID + " : 'nok'";
                            }
                            else
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_AnomalyClassID + " : 'ok'";
                            }
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Anomaly score: " + (hv_AnomalyScore.TupleString(
                                ".2f"));
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if ((int)((new HTuple(hv_AnomalyClassificationThreshold.TupleNotEqual(
                                -1))).TupleOr(new HTuple(hv_AnomalyRegionThreshold.TupleNotEqual(
                                -1)))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "---------------";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "Thresholds ";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "---------------";
                            }
                            //
                            if ((int)(new HTuple(hv_AnomalyClassificationThreshold.TupleNotEqual(
                                -1))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "Classification: " + (hv_AnomalyClassificationThreshold.TupleString(
                                    ".2f"));
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            }
                            if ((int)(new HTuple(hv_AnomalyRegionThreshold.TupleNotEqual(-1))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "Segmentation: " + (hv_AnomalyRegionThreshold.TupleString(
                                    ".2f"));
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            }
                            //Get or open next child window
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColors.Dispose();
                                HOperatorSet.TupleGenConst(new HTuple(hv_Text.TupleLength()), "white",
                                    out hv_LineColors);
                            }
                            if (hv_LineColors == null)
                                hv_LineColors = new HTuple();
                            hv_LineColors[(new HTuple(hv_LineColors.TupleLength())) - 8] = hv_PredictionColor;
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "top", "left", hv_LineColors, "box", "false");
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "anomaly_image"))) != 0)
                    {
                        //
                        //Image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        //
                        ho_AnomalyImage.Dispose(); hv_AnomalyScore.Dispose(); hv_AnomalyClassID.Dispose();
                        get_anomaly_result(out ho_AnomalyImage, hv_ResultKeys, hv_DLResult, out hv_AnomalyScore,
                            out hv_AnomalyClassID);
                        //
                        //Read in input image.
                        ho_Image.Dispose();
                        HOperatorSet.GetDictObject(out ho_Image, hv_DLSample, "image");
                        //Add the anomaly image to the input image.
                        {
                            HObject ExpTmpOutVar_0;
                            add_colormap_to_image(ho_AnomalyImage, ho_Image, out ExpTmpOutVar_0, hv_HeatmapColorScheme);
                            ho_AnomalyImage.Dispose();
                            ho_AnomalyImage = ExpTmpOutVar_0;
                        }
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_AnomalyImage, HDevWindowStack.GetActive());
                        }
                        hv_Text.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Text = "Anomaly image " + hv_ImageIDStringBraces;
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if ((int)(new HTuple(hv_AnomalyClassID.TupleEqual(1))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_AnomalyClassID + " : 'nok'";
                            }
                            else
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_AnomalyClassID + " : 'ok'";
                            }
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Anomaly score: " + (hv_AnomalyScore.TupleString(
                                ".2f"));
                            //Get or open next child window
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "top", "left", "white", "box", "false");
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "classification_ground_truth"))) != 0)
                    {
                        //
                        //Ground truth classification image and class label.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        //
                        hv_ClassificationLabelIDGroundTruth.Dispose();
                        get_classification_ground_truth(hv_SampleKeys, hv_DLSample, out hv_ClassificationLabelIDGroundTruth);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualization.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        //
                        if ((int)(hv_ShowClassificationIDs) != 0)
                        {
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "GT label ID: " + hv_ClassificationLabelIDGroundTruth;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "top", "left", "white", "box", "false");
                            }
                        }
                        //
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "Ground truth classification " + hv_ImageIDStringBraces;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "Ground truth class ID " + hv_ImageIDStringBraces;
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Text = hv_Text.TupleConcat(
                                        hv_ClassesLegend.TupleSelect(hv_ClassificationLabelIDGroundTruth));
                                    hv_Text.Dispose();
                                    hv_Text = ExpTmpLocalVar_Text;
                                }
                            }
                            //
                            //Get or open next child window
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                        "top", "left", (new HTuple("white")).TupleConcat(hv_Colors.TupleSelect(
                                        hv_ClassificationLabelIDGroundTruth)), "box", "false");
                                }
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "classification_result"))) != 0)
                    {
                        //
                        //Ground truth classification image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        //
                        hv_ClassificationLabelIDResult.Dispose();
                        get_classification_result(hv_ResultKeys, hv_DLResult, out hv_ClassificationLabelIDResult);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualization.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        //
                        //Display the class IDs.
                        if ((int)(hv_ShowClassificationIDs) != 0)
                        {
                            hv_MetaInfo.Dispose();
                            HOperatorSet.GetDictTuple(hv_WindowHandleDict, "meta_information", out hv_MetaInfo);
                            hv_MarginBottom.Dispose();
                            HOperatorSet.GetDictTuple(hv_MetaInfo, "classification_result_margin_bottom",
                                out hv_MarginBottom);
                            hv_WindowCoordinates.Dispose();
                            HOperatorSet.GetDictTuple(hv_MetaInfo, "classification_result_window_coordinates",
                                out hv_WindowCoordinates);
                            hv_CurrentWindowHeight.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_CurrentWindowHeight = (hv_WindowCoordinates.TupleSelect(
                                    3)) - (hv_WindowCoordinates.TupleSelect(0));
                            }
                            hv__.Dispose(); hv__.Dispose(); hv__.Dispose(); hv_MaxHeight.Dispose();
                            HOperatorSet.GetFontExtents(hv_CurrentWindowHandle, out hv__, out hv__,
                                out hv__, out hv_MaxHeight);
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "Result class ID: " + hv_ClassificationLabelIDResult;
                            }
                            if ((int)(hv_ShowBottomDesc) != 0)
                            {
                                if (HDevWindowStack.IsOpen())
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                            hv_CurrentWindowHeight - ((hv_MarginBottom + hv_MaxHeight) + 10), "left",
                                            "white", "box", "false");
                                    }
                                }
                            }
                            else
                            {
                                if (HDevWindowStack.IsOpen())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                        "bottom", "left", "white", "box", "false");
                                }
                            }
                        }
                        //
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "Result classification " + hv_ImageIDStringBraces;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "Result class ID " + hv_ImageIDStringBraces;
                            }
                            if ((int)(new HTuple(hv_ClassificationLabelIDResult.TupleEqual(new HTuple()))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "No classification result is given!";
                            }
                            else
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Text = hv_Text.TupleConcat(
                                            hv_ClassesLegend.TupleSelect(hv_ClassificationLabelIDResult));
                                        hv_Text.Dispose();
                                        hv_Text = ExpTmpLocalVar_Text;
                                    }
                                }
                            }
                            //
                            //Get or open next child window
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                        "top", "left", (new HTuple("white")).TupleConcat(hv_Colors.TupleSelect(
                                        hv_ClassificationLabelIDResult)), "box", "false");
                                }
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "classification_both"))) != 0)
                    {
                        //
                        //Ground truth and result classification image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        //
                        hv_ClassificationLabelIDGroundTruth.Dispose();
                        get_classification_ground_truth(hv_SampleKeys, hv_DLSample, out hv_ClassificationLabelIDGroundTruth);
                        hv_ClassificationLabelIDResult.Dispose();
                        get_classification_result(hv_ResultKeys, hv_DLResult, out hv_ClassificationLabelIDResult);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualization.
                        //
                        //Get prediction color.
                        hv_PredictionColor.Dispose();
                        hv_PredictionColor = "white";
                        if ((int)(new HTuple(hv_ClassificationLabelIDGroundTruth.TupleEqual(hv_ClassificationLabelIDResult))) != 0)
                        {
                            hv_PredictionText.Dispose();
                            hv_PredictionText = "Correct";
                            hv_PredictionColor.Dispose();
                            hv_PredictionColor = "green";
                        }
                        else
                        {
                            hv_PredictionText.Dispose();
                            hv_PredictionText = "Wrong";
                            hv_PredictionColor.Dispose();
                            hv_PredictionColor = "red";
                        }
                        //
                        //Generate prediction color frame and show image.
                        if ((int)(hv_ShowClassificationColorFrame) != 0)
                        {
                            //Create a frame with line width 7 that is completely displayed in the window.
                            hv_BoarderOffset.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_BoarderOffset = 7 / 2.0;
                            }
                            hv_MetaInfo.Dispose();
                            HOperatorSet.GetDictTuple(hv_WindowHandleDict, "meta_information", out hv_MetaInfo);
                            hv_WindowImageRatioHeight.Dispose();
                            HOperatorSet.GetDictTuple(hv_MetaInfo, "classification_both_window_image_ratio_height",
                                out hv_WindowImageRatioHeight);
                            hv_WindowImageRatioWidth.Dispose();
                            HOperatorSet.GetDictTuple(hv_MetaInfo, "classification_both_window_image_ratio_width",
                                out hv_WindowImageRatioWidth);
                            hv_BoarderOffsetRow.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_BoarderOffsetRow = hv_BoarderOffset / hv_WindowImageRatioHeight;
                            }
                            hv_BoarderOffsetCol.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_BoarderOffsetCol = hv_BoarderOffset / hv_WindowImageRatioWidth;
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_PredictionColorFrame.Dispose();
                                HOperatorSet.GenContourPolygonXld(out ho_PredictionColorFrame, ((((((((hv_BoarderOffsetRow - 0.5)).TupleConcat(
                                    hv_BoarderOffsetRow - 0.5))).TupleConcat((hv_HeightImage + 0.5) - hv_BoarderOffsetRow))).TupleConcat(
                                    (hv_HeightImage + 0.5) - hv_BoarderOffsetRow))).TupleConcat(hv_BoarderOffsetRow - 0.5),
                                    ((((((((hv_BoarderOffsetCol - 0.5)).TupleConcat((hv_WidthImage + 0.5) - hv_BoarderOffsetCol))).TupleConcat(
                                    (hv_WidthImage + 0.5) - hv_BoarderOffsetCol))).TupleConcat(hv_BoarderOffsetCol - 0.5))).TupleConcat(
                                    hv_BoarderOffsetCol - 0.5));
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), 7);
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_PredictionColor);
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_PredictionColorFrame, HDevWindowStack.GetActive()
                                    );
                            }
                        }
                        else
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                            }
                        }
                        //
                        if ((int)(hv_ShowClassificationIDs) != 0)
                        {
                            hv_MetaInfo.Dispose();
                            HOperatorSet.GetDictTuple(hv_WindowHandleDict, "meta_information", out hv_MetaInfo);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_MarginBottom.Dispose();
                                HOperatorSet.GetDictTuple(hv_MetaInfo, (hv_KeysForDisplay.TupleSelect(
                                    hv_KeyIndex)) + "_margin_bottom", out hv_MarginBottom);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowCoordinates.Dispose();
                                HOperatorSet.GetDictTuple(hv_MetaInfo, (hv_KeysForDisplay.TupleSelect(
                                    hv_KeyIndex)) + "_window_coordinates", out hv_WindowCoordinates);
                            }
                            hv_CurrentWindowHeight.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_CurrentWindowHeight = (hv_WindowCoordinates.TupleSelect(
                                    3)) - (hv_WindowCoordinates.TupleSelect(0));
                            }
                            hv__.Dispose(); hv__.Dispose(); hv__.Dispose(); hv_MaxHeight.Dispose();
                            HOperatorSet.GetFontExtents(hv_CurrentWindowHandle, out hv__, out hv__,
                                out hv__, out hv_MaxHeight);
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "GT label ID: " + hv_ClassificationLabelIDGroundTruth;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "top", "left", "white", "box", "false");
                            }
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "Result class ID: " + hv_ClassificationLabelIDResult;
                            }
                            if ((int)(hv_ShowBottomDesc) != 0)
                            {
                                if (HDevWindowStack.IsOpen())
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                            hv_CurrentWindowHeight - ((hv_MarginBottom + hv_MaxHeight) + 10), "left",
                                            "white", "box", "false");
                                    }
                                }
                            }
                            else
                            {
                                if (HDevWindowStack.IsOpen())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                        "bottom", "left", "white", "box", "false");
                                }
                            }
                        }
                        //
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "Result/Ground truth classification " + hv_ImageIDStringBraces;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "Ground truth class ID " + hv_ImageIDStringBraces;
                            }
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = hv_ClassesLegend.TupleSelect(
                                hv_ClassificationLabelIDGroundTruth);
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Result class ID";
                            if ((int)(new HTuple(hv_ClassificationLabelIDResult.TupleEqual(new HTuple()))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "No classification result is given!";
                            }
                            else
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_ClassesLegend.TupleSelect(
                                    hv_ClassificationLabelIDResult);
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "Prediction ";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_PredictionText;
                            }
                            //
                            //Get or open next child window.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColors.Dispose();
                                HOperatorSet.TupleGenConst(new HTuple(hv_Text.TupleLength()), "white",
                                    out hv_LineColors);
                            }
                            if (hv_LineColors == null)
                                hv_LineColors = new HTuple();
                            hv_LineColors[1] = hv_Colors.TupleSelect(hv_ClassificationLabelIDGroundTruth);
                            if ((int)(new HTuple(hv_ClassificationLabelIDResult.TupleNotEqual(new HTuple()))) != 0)
                            {
                                if (hv_LineColors == null)
                                    hv_LineColors = new HTuple();
                                hv_LineColors[5] = hv_Colors.TupleSelect(hv_ClassificationLabelIDResult);
                                if (hv_LineColors == null)
                                    hv_LineColors = new HTuple();
                                hv_LineColors[9] = hv_PredictionColor;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "top", "left", hv_LineColors, "box", "false");
                            }
                        }
                        //
                    }
                    else if ((int)((new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "heatmap_grad_cam"))).TupleOr(new HTuple(((hv_KeysForDisplay.TupleSelect(
                        hv_KeyIndex))).TupleEqual("heatmap_confidence_based")))) != 0)
                    {
                        //
                        //Display the heatmap image (method 'heatmap_grad_cam' or 'heatmap_confidence_based')
                        //in the selected color scheme.
                        //Retrieve heatmap image, inferred image, and inference results.
                        hv_SelectedHeatmapMethod.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_SelectedHeatmapMethod = hv_KeysForDisplay.TupleSelect(
                                hv_KeyIndex);
                        }
                        if ((int)((new HTuple(((hv_ResultKeys.TupleFind("heatmap_grad_cam"))).TupleNotEqual(
                            -1))).TupleOr(new HTuple(((hv_ResultKeys.TupleFind("heatmap_confidence_based"))).TupleNotEqual(
                            -1)))) != 0)
                        {
                            if ((int)(new HTuple(hv_SelectedHeatmapMethod.TupleEqual("heatmap_grad_cam"))) != 0)
                            {
                                hv_DictHeatmap.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResult, "heatmap_grad_cam", out hv_DictHeatmap);
                                hv_MethodName.Dispose();
                                hv_MethodName = "Grad-CAM";
                            }
                            else
                            {
                                hv_DictHeatmap.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResult, "heatmap_confidence_based",
                                    out hv_DictHeatmap);
                                hv_MethodName.Dispose();
                                hv_MethodName = "Confidence based";
                            }
                            hv_HeatmapKeys.Dispose();
                            HOperatorSet.GetDictParam(hv_DictHeatmap, "keys", new HTuple(), out hv_HeatmapKeys);
                            hv_HeatmapImageName.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_HeatmapImageName = hv_HeatmapKeys.TupleRegexpSelect(
                                    "heatmap_image_class_[0-9]*");
                            }
                            hv_TargetClassID.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_TargetClassID = hv_HeatmapImageName.TupleRegexpMatch(
                                    "heatmap_image_class_([0-9]+)$");
                            }
                            ho_ImageHeatmap.Dispose();
                            HOperatorSet.GetDictObject(out ho_ImageHeatmap, hv_DictHeatmap, hv_HeatmapImageName);
                        }
                        else
                        {
                            throw new HalconException("Heatmap image could not be found in DLResult.");
                        }
                        //
                        if ((int)(new HTuple(hv_SelectedHeatmapMethod.TupleEqual("heatmap_grad_cam"))) != 0)
                        {
                            //Read in input image.
                            ho_Image.Dispose();
                            HOperatorSet.GetDictObject(out ho_Image, hv_DLSample, "image");
                            //Add the heatmap to the input image.
                            {
                                HObject ExpTmpOutVar_0;
                                add_colormap_to_image(ho_ImageHeatmap, ho_Image, out ExpTmpOutVar_0,
                                    hv_HeatmapColorScheme);
                                ho_ImageHeatmap.Dispose();
                                ho_ImageHeatmap = ExpTmpOutVar_0;
                            }
                        }
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_ImageHeatmap, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_ImageHeatmap, HDevWindowStack.GetActive());
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "Classification heatmap " + hv_ImageIDStringBraces;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            hv_ClassNames.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLResult, "classification_class_names",
                                out hv_ClassNames);
                            hv_ClassIDs.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLResult, "classification_class_ids", out hv_ClassIDs);
                            hv_Confidences.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLResult, "classification_confidences",
                                out hv_Confidences);
                            if ((int)(new HTuple(hv_SelectedHeatmapMethod.TupleEqual("heatmap_confidence_based"))) != 0)
                            {
                                hv_MaxDeviation.Dispose();
                                HOperatorSet.GetDictTuple(hv_DictHeatmap, "classification_heatmap_maxdeviation",
                                    out hv_MaxDeviation);
                            }
                            hv_ClassificationLabelNameResult.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ClassificationLabelNameResult = hv_ClassNames.TupleSelect(
                                    0);
                            }
                            hv_ClassificationLabelIDResult.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ClassificationLabelIDResult = hv_ClassIDs.TupleSelect(
                                    0);
                            }
                            hv_TargetClassConfidence.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_TargetClassConfidence = hv_Confidences.TupleSelect(
                                    hv_ClassIDs.TupleFind(hv_TargetClassID.TupleNumber()));
                            }
                            hv_Text.Dispose();
                            hv_Text = "--------- ";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Image ";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "--------- ";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if ((int)(new HTuple(((hv_SampleKeys.TupleFind("image_label_id"))).TupleNotEqual(
                                -1))) != 0)
                            {
                                hv_ClassificationLabelIDGroundTruth.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLSample, "image_label_id", out hv_ClassificationLabelIDGroundTruth);
                                hv_ClassificationLabelNamesGroundTruth.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLDatasetInfo, "class_names", out hv_ClassificationLabelNamesGroundTruth);
                                //Get prediction color.
                                if ((int)(new HTuple(hv_ClassificationLabelIDGroundTruth.TupleEqual(
                                    hv_ClassificationLabelIDResult))) != 0)
                                {
                                    hv_PredictionColor.Dispose();
                                    hv_PredictionColor = "green";
                                }
                                else
                                {
                                    hv_PredictionColor.Dispose();
                                    hv_PredictionColor = "red";
                                }
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "Ground truth class: ";
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_ClassificationLabelNamesGroundTruth.TupleSelect(
                                    hv_ClassificationLabelIDGroundTruth);
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            }
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Predicted class: ";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = hv_ClassificationLabelNameResult;
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Confidence: " + (((hv_Confidences.TupleSelect(
                                0))).TupleString(".2f"));
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "--------- ";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Heatmap ";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "--------- ";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Method: " + hv_MethodName;
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Target class: " + hv_TargetClassID;
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Target class confidence: " + (hv_TargetClassConfidence.TupleString(
                                ".2f"));
                            if ((int)(new HTuple(hv_SelectedHeatmapMethod.TupleEqual("heatmap_confidence_based"))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = "Maximum deviation: " + (hv_MaxDeviation.TupleString(
                                    ".2f"));
                            }
                            //
                            //Get or open next child window
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColors.Dispose();
                                HOperatorSet.TupleGenConst(new HTuple(hv_Text.TupleLength()), "white",
                                    out hv_LineColors);
                            }
                            if ((int)(new HTuple(((hv_SampleKeys.TupleFind("image_label_id"))).TupleNotEqual(
                                -1))) != 0)
                            {
                                if (hv_LineColors == null)
                                    hv_LineColors = new HTuple();
                                hv_LineColors[8] = hv_PredictionColor;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "top", "left", hv_LineColors, "box", "false");
                            }
                        }
                        //
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "segmentation_weight"))) != 0)
                    {
                        //
                        //Weight image.
                        ho_ImageWeight.Dispose();
                        get_weight_image(out ho_ImageWeight, hv_SampleKeys, hv_DLSample);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_ImageWeight, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_ImageWeight, HDevWindowStack.GetActive());
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Weight image " + hv_ImageIDStringBraces,
                                        "window", "bottom", "left", "white", "box", "false");
                                }
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "segmentation_confidence"))) != 0)
                    {
                        //
                        //Segmentation confidences.
                        ho_ImageConfidence.Dispose();
                        get_confidence_image(out ho_ImageConfidence, hv_ResultKeys, hv_DLResult);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_ImageConfidence, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_ImageConfidence, HDevWindowStack.GetActive());
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Confidence image " + hv_ImageIDStringBraces,
                                        "window", "bottom", "left", "white", "box", "false");
                                }
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "bbox_ground_truth"))) != 0)
                    {
                        //
                        //Sample bounding boxes on image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualization.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        //
                        hv_BboxIDs.Dispose();
                        dev_display_ground_truth_detection(hv_DLSample, hv_SampleKeys, hv_LineWidth,
                            hv_ClassIDs, hv_Colors, hv_BboxLabelColor, hv_BboxTextColor, hv_ShowLabels,
                            hv_ShowDirection, hv_CurrentWindowHandle, out hv_BboxIDs);
                        hv_Text.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Text = "Ground truth " + hv_ImageIDStringBraces;
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            hv_BboxColors.Dispose();
                            hv_BboxColors = "white";
                            if ((int)(new HTuple(hv_BboxIDs.TupleLength())) != 0)
                            {
                                hv_BboxIDsUniq.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_BboxIDsUniq = ((hv_BboxIDs.TupleSort()
                                        )).TupleUniq();
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Text = hv_Text.TupleConcat(
                                            hv_ClassesLegend.TupleSelect(hv_BboxIDsUniq));
                                        hv_Text.Dispose();
                                        hv_Text = ExpTmpLocalVar_Text;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_BboxColors = hv_BboxColors.TupleConcat(
                                            hv_Colors.TupleSelect(hv_BboxIDsUniq));
                                        hv_BboxColors.Dispose();
                                        hv_BboxColors = ExpTmpLocalVar_BboxColors;
                                    }
                                }
                            }
                            else
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Text = hv_Text.TupleConcat(
                                            "No ground truth bounding boxes present.");
                                        hv_Text.Dispose();
                                        hv_Text = ExpTmpLocalVar_Text;
                                    }
                                }
                            }
                            //
                            //Get or open next child window.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "top", "left", hv_BboxColors, "box", "false");
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "bbox_result"))) != 0)
                    {
                        //
                        //Result bounding boxes on image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualization.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        //
                        if ((int)(new HTuple(((hv_ResultKeys.TupleFind("bbox_confidence"))).TupleNotEqual(
                            -1))) != 0)
                        {
                            hv_BboxConfidences.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLResult, "bbox_confidence", out hv_BboxConfidences);
                        }
                        else
                        {
                            throw new HalconException("Result bounding box data could not be found in DLResult.");
                        }
                        if ((int)(hv_BboxDisplayConfidence) != 0)
                        {
                            hv_TextConf.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_TextConf = (" (" + (hv_BboxConfidences.TupleString(
                                    ".2f"))) + ")";
                            }
                        }
                        else
                        {
                            hv_TextConf.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_TextConf = HTuple.TupleGenConst(
                                    new HTuple(hv_BboxConfidences.TupleLength()), "");
                            }
                        }
                        hv_BboxClassIndex.Dispose();
                        dev_display_result_detection(hv_DLResult, hv_ResultKeys, hv_LineWidth,
                            hv_ClassIDs, hv_TextConf, hv_Colors, hv_BboxLabelColor, hv_WindowImageRatio,
                            "top", hv_BboxTextColor, hv_ShowLabels, hv_ShowDirection, hv_CurrentWindowHandle,
                            out hv_BboxClassIndex);
                        hv_Text.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Text = "Result bounding boxes " + hv_ImageIDStringBraces;
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            hv_BboxColorsResults.Dispose();
                            hv_BboxColorsResults = "white";
                            if ((int)(new HTuple((new HTuple(hv_BboxClassIndex.TupleLength())).TupleGreater(
                                0))) != 0)
                            {
                                hv_BboxClassIndexUniq.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_BboxClassIndexUniq = ((hv_BboxClassIndex.TupleSort()
                                        )).TupleUniq();
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Text = hv_Text.TupleConcat(
                                            hv_ClassesLegend.TupleSelect(hv_BboxClassIndexUniq));
                                        hv_Text.Dispose();
                                        hv_Text = ExpTmpLocalVar_Text;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_BboxColorsResults = hv_BboxColorsResults.TupleConcat(
                                            hv_Colors.TupleSelect(hv_BboxClassIndexUniq));
                                        hv_BboxColorsResults.Dispose();
                                        hv_BboxColorsResults = ExpTmpLocalVar_BboxColorsResults;
                                    }
                                }
                            }
                            else
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Text = hv_Text.TupleConcat(
                                            "No result bounding boxes present.");
                                        hv_Text.Dispose();
                                        hv_Text = ExpTmpLocalVar_Text;
                                    }
                                }
                            }
                            //
                            //Get or open next child window.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "top", "left", hv_BboxColorsResults, "box", "false");
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "bbox_both"))) != 0)
                    {
                        //
                        //Ground truth and result bounding boxes on image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        //
                        //Visualization.
                        hv_BboxLabelIndex.Dispose();
                        dev_display_ground_truth_detection(hv_DLSample, hv_SampleKeys, hv_LineWidth,
                            hv_ClassIDs, hv_Colors, hv_BboxLabelColor, hv_BboxTextColor, hv_ShowLabels,
                            hv_ShowDirection, hv_CurrentWindowHandle, out hv_BboxLabelIndex);
                        if ((int)(new HTuple(((hv_ResultKeys.TupleFind("bbox_confidence"))).TupleNotEqual(
                            -1))) != 0)
                        {
                            hv_BboxConfidences.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLResult, "bbox_confidence", out hv_BboxConfidences);
                        }
                        else
                        {
                            throw new HalconException("Result bounding box data could not be found in DLResult.");
                        }
                        if ((int)(hv_BboxDisplayConfidence) != 0)
                        {
                            hv_TextConf.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_TextConf = (" (" + (hv_BboxConfidences.TupleString(
                                    ".2f"))) + ")";
                            }
                        }
                        else
                        {
                            hv_TextConf.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_TextConf = HTuple.TupleGenConst(
                                    new HTuple(hv_BboxConfidences.TupleLength()), "");
                            }
                        }
                        hv_BboxClassIndex.Dispose();
                        dev_display_result_detection(hv_DLResult, hv_ResultKeys, hv_LineWidth,
                            hv_ClassIDs, hv_TextConf, hv_Colors, hv_BboxLabelColor, hv_WindowImageRatio,
                            "bottom", hv_BboxTextColor, hv_ShowLabels, hv_ShowDirection, hv_CurrentWindowHandle,
                            out hv_BboxClassIndex);
                        hv_Text.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Text = "Ground truth and result bounding boxes " + hv_ImageIDStringBraces;
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        hv_Text.Dispose();
                        hv_Text = "Ground truth and";
                        if (hv_Text == null)
                            hv_Text = new HTuple();
                        hv_Text[new HTuple(hv_Text.TupleLength())] = "result bounding boxes " + hv_ImageIDStringBraces;
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            hv_BboxColorsBoth.Dispose();
                            hv_BboxColorsBoth = new HTuple();
                            hv_BboxColorsBoth[0] = "white";
                            hv_BboxColorsBoth[1] = "white";
                            if ((int)(new HTuple((new HTuple((new HTuple(hv_BboxClassIndex.TupleLength()
                                )) + (new HTuple(hv_BboxLabelIndex.TupleLength())))).TupleGreater(0))) != 0)
                            {
                                hv_BboxClassLabelIndexUniq.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_BboxClassLabelIndexUniq = ((((hv_BboxClassIndex.TupleConcat(
                                        hv_BboxLabelIndex))).TupleSort())).TupleUniq();
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Text = hv_Text.TupleConcat(
                                            hv_ClassesLegend.TupleSelect(hv_BboxClassLabelIndexUniq));
                                        hv_Text.Dispose();
                                        hv_Text = ExpTmpLocalVar_Text;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_BboxColorsBoth = hv_BboxColorsBoth.TupleConcat(
                                            hv_Colors.TupleSelect(hv_BboxClassLabelIndexUniq));
                                        hv_BboxColorsBoth.Dispose();
                                        hv_BboxColorsBoth = ExpTmpLocalVar_BboxColorsBoth;
                                    }
                                }
                            }
                            else
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Text = hv_Text.TupleConcat(
                                            "No ground truth nor result bounding boxes present.");
                                        hv_Text.Dispose();
                                        hv_Text = ExpTmpLocalVar_Text;
                                    }
                                }
                            }
                            //
                            //Get or open next child window.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "top", "left", hv_BboxColorsBoth, "box", "false");
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "segmentation_image_ground_truth"))) != 0)
                    {
                        //
                        //Ground truth segmentation image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        ho_SegmentationImagGroundTruth.Dispose();
                        get_segmentation_image_ground_truth(out ho_SegmentationImagGroundTruth,
                            hv_SampleKeys, hv_DLSample);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualization.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        //
                        //Display segmentation regions.
                        hv_ColorsSegmentation.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ColorsSegmentation = hv_Colors + hv_SegTransparency;
                        }
                        hv_DrawMode.Dispose();
                        HOperatorSet.GetDraw(hv_CurrentWindowHandle, out hv_DrawMode);
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetDraw(HDevWindowStack.GetActive(), hv_SegDraw);
                        }
                        hv_Width.Dispose();
                        HOperatorSet.GetLineWidth(hv_CurrentWindowHandle, out hv_Width);
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), hv_LineWidth);
                        }
                        hv_ImageClassIDs.Dispose();
                        dev_display_segmentation_regions(ho_SegmentationImagGroundTruth, hv_ClassIDs,
                            hv_ColorsSegmentation, hv_SegExcludeClassIDs, out hv_ImageClassIDs);
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetDraw(HDevWindowStack.GetActive(), hv_DrawMode);
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), hv_Width.TupleInt()
                                    );
                            }
                        }
                        hv_Text.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Text = "Ground truth segmentation " + hv_ImageIDStringBraces;
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            hv_ImageClassIDsUniq.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ImageClassIDsUniq = ((hv_ImageClassIDs.TupleSort()
                                    )).TupleUniq();
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Text = hv_Text.TupleConcat(
                                        hv_ClassesLegend.TupleSelect(hv_ImageClassIDsUniq));
                                    hv_Text.Dispose();
                                    hv_Text = ExpTmpLocalVar_Text;
                                }
                            }
                            //
                            //Get or open next child window
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                        "top", "left", (new HTuple("white")).TupleConcat(hv_Colors.TupleSelect(
                                        hv_ImageClassIDsUniq)), "box", "false");
                                }
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "segmentation_image_result"))) != 0)
                    {
                        //
                        //Result segmentation on image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        ho_SegmentationImageResult.Dispose();
                        get_segmentation_image_result(out ho_SegmentationImageResult, hv_ResultKeys,
                            hv_DLResult);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualization.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        //
                        //Display result segmentation regions.
                        hv_ColorsResults.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ColorsResults = hv_Colors + hv_SegTransparency;
                        }
                        hv_DrawMode.Dispose();
                        HOperatorSet.GetDraw(hv_CurrentWindowHandle, out hv_DrawMode);
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetDraw(HDevWindowStack.GetActive(), hv_SegDraw);
                        }
                        hv_Width.Dispose();
                        HOperatorSet.GetLineWidth(hv_CurrentWindowHandle, out hv_Width);
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), hv_LineWidth);
                        }
                        hv_ImageClassIDs.Dispose();
                        dev_display_segmentation_regions(ho_SegmentationImageResult, hv_ClassIDs,
                            hv_ColorsResults, hv_SegExcludeClassIDs, out hv_ImageClassIDs);
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetDraw(HDevWindowStack.GetActive(), hv_DrawMode);
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), hv_Width.TupleInt()
                                    );
                            }
                        }
                        hv_Text.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Text = "Result segmentation " + hv_ImageIDStringBraces;
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            hv_ImageClassIDsUniq.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ImageClassIDsUniq = ((hv_ImageClassIDs.TupleSort()
                                    )).TupleUniq();
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Text = hv_Text.TupleConcat(
                                        hv_ClassesLegend.TupleSelect(hv_ImageClassIDsUniq));
                                    hv_Text.Dispose();
                                    hv_Text = ExpTmpLocalVar_Text;
                                }
                            }
                            //
                            //Get or open next child window.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                        "top", "left", (new HTuple("white")).TupleConcat(hv_Colors.TupleSelect(
                                        hv_ImageClassIDsUniq)), "box", "false");
                                }
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "segmentation_image_both"))) != 0)
                    {
                        //
                        //Ground truth and result segmentation on image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        ho_SegmentationImagGroundTruth.Dispose();
                        get_segmentation_image_ground_truth(out ho_SegmentationImagGroundTruth,
                            hv_SampleKeys, hv_DLSample);
                        ho_SegmentationImageResult.Dispose();
                        get_segmentation_image_result(out ho_SegmentationImageResult, hv_ResultKeys,
                            hv_DLResult);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualization.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        //
                        //Display regions.
                        hv_ColorsResults.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ColorsResults = hv_Colors + hv_SegTransparency;
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetDraw(HDevWindowStack.GetActive(), "margin");
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), 2);
                        }
                        hv_GroundTruthIDs.Dispose();
                        dev_display_segmentation_regions(ho_SegmentationImagGroundTruth, hv_ClassIDs,
                            hv_ColorsResults, hv_SegExcludeClassIDs, out hv_GroundTruthIDs);
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), 6);
                        }
                        hv_ResultIDs.Dispose();
                        dev_display_segmentation_regions(ho_SegmentationImageResult, hv_ClassIDs,
                            hv_ColorsResults, hv_SegExcludeClassIDs, out hv_ResultIDs);
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetDraw(HDevWindowStack.GetActive(), "fill");
                        }
                        hv_Text.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Text = "Ground truth and result segmentation " + hv_ImageIDStringBraces;
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                        //
                        //Display the legend.
                        if ((int)(hv_ShowLegend) != 0)
                        {
                            hv_ImageClassIDsUniq.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ImageClassIDsUniq = ((((hv_GroundTruthIDs.TupleConcat(
                                    hv_ResultIDs))).TupleSort())).TupleUniq();
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Text = hv_Text.TupleConcat(
                                        hv_ClassesLegend.TupleSelect(hv_ImageClassIDsUniq));
                                    hv_Text.Dispose();
                                    hv_Text = ExpTmpLocalVar_Text;
                                }
                            }
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[(new HTuple(hv_Text.TupleLength())) + 1] = new HTuple("- thicker line: result, thinner lines: ground truth");
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "  (you may have to zoom in for a more detailed view)";
                            hv_StringSegExcludeClassIDs.Dispose();
                            hv_StringSegExcludeClassIDs = "";
                            for (hv_StringIndex = 0; (int)hv_StringIndex <= (int)((new HTuple(hv_SegExcludeClassIDs.TupleLength()
                                )) - 1); hv_StringIndex = (int)hv_StringIndex + 1)
                            {
                                if ((int)(new HTuple(hv_StringIndex.TupleEqual((new HTuple(hv_SegExcludeClassIDs.TupleLength()
                                    )) - 1))) != 0)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_StringSegExcludeClassIDs = hv_StringSegExcludeClassIDs + (hv_SegExcludeClassIDs.TupleSelect(
                                                hv_StringIndex));
                                            hv_StringSegExcludeClassIDs.Dispose();
                                            hv_StringSegExcludeClassIDs = ExpTmpLocalVar_StringSegExcludeClassIDs;
                                        }
                                    }
                                }
                                else
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_StringSegExcludeClassIDs = (hv_StringSegExcludeClassIDs + (hv_SegExcludeClassIDs.TupleSelect(
                                                hv_StringIndex))) + new HTuple(", ");
                                            hv_StringSegExcludeClassIDs.Dispose();
                                            hv_StringSegExcludeClassIDs = ExpTmpLocalVar_StringSegExcludeClassIDs;
                                        }
                                    }
                                }
                            }
                            if ((int)(new HTuple(hv_SegExcludeClassIDs.TupleNotEqual(new HTuple()))) != 0)
                            {
                                if (hv_Text == null)
                                    hv_Text = new HTuple();
                                hv_Text[new HTuple(hv_Text.TupleLength())] = ("- (excluded classID(s) " + hv_StringSegExcludeClassIDs) + " from visualization)";
                            }
                            //
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                                get_child_window(hv_HeightImage, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                                    hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex),
                                    out hv_WindowImageRatio, out ExpTmpOutVar_0);
                                hv_PrevWindowCoordinates.Dispose();
                                hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                        "top", "left", (((new HTuple("white")).TupleConcat(hv_Colors.TupleSelect(
                                        hv_ImageClassIDsUniq)))).TupleConcat(((new HTuple("white")).TupleConcat(
                                        "white")).TupleConcat("white")), "box", "false");
                                }
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "segmentation_image_diff"))) != 0)
                    {
                        //
                        //Difference of ground truth and result segmentation on image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        ho_SegmentationImagGroundTruth.Dispose();
                        get_segmentation_image_ground_truth(out ho_SegmentationImagGroundTruth,
                            hv_SampleKeys, hv_DLSample);
                        ho_SegmentationImageResult.Dispose();
                        get_segmentation_image_result(out ho_SegmentationImageResult, hv_ResultKeys,
                            hv_DLResult);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, 0, hv_ScaleWindows, hv_ThresholdWidth, hv_PrevWindowCoordinates,
                                hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(hv_KeyIndex), out hv_CurrentWindowHandle,
                                out hv_WindowImageRatio, out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualization.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        ho_ImageAbsDiff.Dispose();
                        HOperatorSet.AbsDiffImage(ho_SegmentationImagGroundTruth, ho_SegmentationImageResult,
                            out ho_ImageAbsDiff, 1);
                        hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                        HOperatorSet.MinMaxGray(ho_SegmentationImageResult, ho_ImageAbsDiff, 0,
                            out hv_Min, out hv_Max, out hv_Range);
                        if ((int)(new HTuple(hv_Min.TupleNotEqual(hv_Max))) != 0)
                        {
                            ho_DiffRegion.Dispose();
                            HOperatorSet.Threshold(ho_ImageAbsDiff, out ho_DiffRegion, 0.00001, hv_Max);
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.SetColor(HDevWindowStack.GetActive(), "#ff0000" + hv_SegTransparency);
                                }
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_DiffRegion, HDevWindowStack.GetActive());
                            }
                        }
                        else
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), "No difference found.",
                                    "window", "top", "left", "black", new HTuple(), new HTuple());
                            }
                        }
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = "Difference of ground truth and result segmentation " + hv_ImageIDStringBraces;
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window",
                                    "bottom", "left", "white", "box", "false");
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "segmentation_weight_map"))) != 0)
                    {
                        //
                        //Weight map on image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        ho_ImageWeight.Dispose();
                        get_weight_image(out ho_ImageWeight, hv_SampleKeys, hv_DLSample);
                        //
                        if ((int)(new HTuple(hv_SegMaxWeight.TupleEqual(0))) != 0)
                        {
                            //Calculate SegMaxWeight if not given in GenParam.
                            hv_MinWeight.Dispose(); hv_SegMaxWeight.Dispose(); hv_Range.Dispose();
                            HOperatorSet.MinMaxGray(ho_ImageWeight, ho_ImageWeight, 0, out hv_MinWeight,
                                out hv_SegMaxWeight, out hv_Range);
                        }
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, hv_MapColorBarWidth, hv_ScaleWindows, hv_ThresholdWidth,
                                hv_PrevWindowCoordinates, hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(
                                hv_KeyIndex), out hv_CurrentWindowHandle, out hv_WindowImageRatio,
                                out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualization.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        hv_WeightsColors.Dispose();
                        dev_display_weight_regions(ho_ImageWeight, hv_MapTransparency, hv_SegMaxWeight,
                            out hv_WeightsColors);
                        dev_display_map_color_bar(hv_WidthImage, hv_HeightImage, hv_MapColorBarWidth,
                            hv_WeightsColors, hv_SegMaxWeight, hv_WindowImageRatio, hv_CurrentWindowHandle);
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Weight map " + hv_ImageIDStringBraces,
                                        "window", "bottom", "left", "white", "box", "false");
                                }
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_KeysForDisplay.TupleSelect(hv_KeyIndex))).TupleEqual(
                        "segmentation_confidence_map"))) != 0)
                    {
                        //
                        //Segmentation confidence map on image.
                        ho_Image.Dispose();
                        get_image(out ho_Image, hv_SampleKeys, hv_DLSample);
                        ho_ImageConfidence.Dispose();
                        get_confidence_image(out ho_ImageConfidence, hv_ResultKeys, hv_DLResult);
                        //
                        //Get or open next window.
                        hv_WidthImage.Dispose(); hv_HeightImage.Dispose();
                        HOperatorSet.GetImageSize(ho_Image, out hv_WidthImage, out hv_HeightImage);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatio.Dispose(); HTuple ExpTmpOutVar_0;
                            get_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                                hv_HeightImage, hv_MapColorBarWidth, hv_ScaleWindows, hv_ThresholdWidth,
                                hv_PrevWindowCoordinates, hv_WindowHandleDict, hv_KeysForDisplay.TupleSelect(
                                hv_KeyIndex), out hv_CurrentWindowHandle, out hv_WindowImageRatio,
                                out ExpTmpOutVar_0);
                            hv_PrevWindowCoordinates.Dispose();
                            hv_PrevWindowCoordinates = ExpTmpOutVar_0;
                        }
                        //
                        //Visualization.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                        }
                        hv_ConfidenceColors.Dispose();
                        dev_display_confidence_regions(ho_ImageConfidence, hv_MapTransparency,
                            out hv_ConfidenceColors);
                        dev_display_map_color_bar(hv_WidthImage, hv_HeightImage, hv_MapColorBarWidth,
                            hv_ConfidenceColors, 1.0, hv_WindowImageRatio, hv_CurrentWindowHandle);
                        if ((int)(hv_ShowBottomDesc) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Confidence map " + hv_ImageIDStringBraces,
                                        "window", "bottom", "left", "white", "box", "false");
                                }
                            }
                        }
                    }
                    else
                    {
                        //Reset flush buffer of existing windows before throwing an exception.
                        hv_WindowHandleKeys.Dispose();
                        HOperatorSet.GetDictParam(hv_WindowHandleDict, "keys", new HTuple(), out hv_WindowHandleKeys);
                        for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_WindowHandleKeys.TupleLength()
                            )) - 1); hv_Index = (int)hv_Index + 1)
                        {
                            //Only consider the WindowHandleKeys that are needed for the current visualization.
                            hv_Indices.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Indices = hv_KeysForDisplay.TupleFind(
                                    hv_WindowHandleKeys.TupleSelect(hv_Index));
                            }
                            if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                                new HTuple())))) != 0)
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_WindowHandles.Dispose();
                                    HOperatorSet.GetDictTuple(hv_WindowHandleDict, hv_WindowHandleKeys.TupleSelect(
                                        hv_Index), out hv_WindowHandles);
                                }
                                for (hv_WindowIndex = 0; (int)hv_WindowIndex <= (int)((new HTuple(hv_WindowHandles.TupleLength()
                                    )) - 1); hv_WindowIndex = (int)hv_WindowIndex + 1)
                                {
                                    //Reset values of windows that have been changed temporarily.
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.SetWindowParam(hv_WindowHandles.TupleSelect(hv_WindowIndex),
                                            "flush", hv_FlushValues.TupleSelect(hv_Index));
                                    }
                                }
                            }
                        }
                        throw new HalconException("Key for display unknown: " + (hv_KeysForDisplay.TupleSelect(
                            hv_KeyIndex)));
                    }
                }
                //
                //Display results.
                hv_WindowHandleKeysNew.Dispose();
                HOperatorSet.GetDictParam(hv_WindowHandleDict, "keys", new HTuple(), out hv_WindowHandleKeysNew);
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_WindowHandleKeysNew.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    //Only consider the WindowHandleKeys that are needed for the current visualization.
                    hv_KeyIndex.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_KeyIndex = hv_KeysForDisplay.TupleFind(
                            hv_WindowHandleKeysNew.TupleSelect(hv_Index));
                    }
                    if ((int)((new HTuple(hv_KeyIndex.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_KeyIndex.TupleNotEqual(
                        new HTuple())))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_WindowHandles.Dispose();
                            HOperatorSet.GetDictTuple(hv_WindowHandleDict, hv_WindowHandleKeysNew.TupleSelect(
                                hv_Index), out hv_WindowHandles);
                        }
                        for (hv_WindowIndex = 0; (int)hv_WindowIndex <= (int)((new HTuple(hv_WindowHandles.TupleLength()
                            )) - 1); hv_WindowIndex = (int)hv_WindowIndex + 1)
                        {
                            //Display content of window handle.
                            if ((int)(new HTuple((new HTuple(hv_WindowHandleKeys.TupleLength())).TupleEqual(
                                new HTuple(hv_WindowHandleKeysNew.TupleLength())))) != 0)
                            {
                                //Reset values of windows that have been changed temporarily.
                                if ((int)(new HTuple(((hv_FlushValues.TupleSelect(hv_WindowIndex))).TupleEqual(
                                    "true"))) != 0)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.FlushBuffer(hv_WindowHandles.TupleSelect(hv_WindowIndex));
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.SetWindowParam(hv_WindowHandles.TupleSelect(hv_WindowIndex),
                                        "flush", hv_FlushValues.TupleSelect(hv_WindowIndex));
                                }
                            }
                            else
                            {
                                //Per default, 'flush' of new windows should be set to 'true'.
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.FlushBuffer(hv_WindowHandles.TupleSelect(hv_WindowIndex));
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.SetWindowParam(hv_WindowHandles.TupleSelect(hv_WindowIndex),
                                        "flush", "true");
                                }
                            }
                        }
                    }
                }
                //
                ho_Image.Dispose();
                ho_AnomalyImage.Dispose();
                ho_AnomalyRegion.Dispose();
                ho_PredictionColorFrame.Dispose();
                ho_ImageHeatmap.Dispose();
                ho_ImageWeight.Dispose();
                ho_ImageConfidence.Dispose();
                ho_SegmentationImagGroundTruth.Dispose();
                ho_SegmentationImageResult.Dispose();
                ho_ImageAbsDiff.Dispose();
                ho_DiffRegion.Dispose();

                hv_ThresholdWidth.Dispose();
                hv_ScaleWindows.Dispose();
                hv_Font.Dispose();
                hv_FontSize.Dispose();
                hv_LineWidth.Dispose();
                hv_MapTransparency.Dispose();
                hv_MapColorBarWidth.Dispose();
                hv_AnomalyRegionThreshold.Dispose();
                hv_AnomalyClassificationThreshold.Dispose();
                hv_AnomalyRegionLabelColor.Dispose();
                hv_AnomalyColorTransparency.Dispose();
                hv_AnomalyRegionResultColor.Dispose();
                hv_SegMaxWeight.Dispose();
                hv_SegDraw.Dispose();
                hv_SegTransparency.Dispose();
                hv_SegExcludeClassIDs.Dispose();
                hv_BboxLabelColor.Dispose();
                hv_BboxDisplayConfidence.Dispose();
                hv_BboxTextColor.Dispose();
                hv_ShowBottomDesc.Dispose();
                hv_ShowLegend.Dispose();
                hv_ShowGroundTruthAnomalyRegions.Dispose();
                hv_ShowClassificationIDs.Dispose();
                hv_ShowClassificationColorFrame.Dispose();
                hv_ShowLabels.Dispose();
                hv_ShowDirection.Dispose();
                hv_HeatmapColorScheme.Dispose();
                hv_GenParamNames.Dispose();
                hv_ParamIndex.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();
                hv_SampleKeys.Dispose();
                hv_ResultKeys.Dispose();
                hv_ImageIDExists.Dispose();
                hv_ImageID.Dispose();
                hv_ImageIDString.Dispose();
                hv_ImageIDStringBraces.Dispose();
                hv_ImageIDStringCapital.Dispose();
                hv_NeededKeys.Dispose();
                hv_Index.Dispose();
                hv_DLDatasetInfoKeys.Dispose();
                hv_ClassNames.Dispose();
                hv_ClassIDs.Dispose();
                hv_Colors.Dispose();
                hv_ClassesLegend.Dispose();
                hv_PrevWindowCoordinates.Dispose();
                hv_Keys.Dispose();
                hv_Exception.Dispose();
                hv_MetaInfoIndex.Dispose();
                hv_MetaInfo.Dispose();
                hv_FlushValues.Dispose();
                hv_WindowHandleKeys.Dispose();
                hv_KeyIndex.Dispose();
                hv_WindowHandles.Dispose();
                hv_WindowIndex.Dispose();
                hv_FlushValue.Dispose();
                hv_WidthImage.Dispose();
                hv_HeightImage.Dispose();
                hv_CurrentWindowHandle.Dispose();
                hv_WindowImageRatio.Dispose();
                hv_AnomalyLabelGroundTruth.Dispose();
                hv_AnomalyLabelIDGroundTruth.Dispose();
                hv_AnomalyRegionExists.Dispose();
                hv_Text.Dispose();
                hv_AnomalyScore.Dispose();
                hv_AnomalyClassID.Dispose();
                hv_AnomalyRegionGroundTruthExists.Dispose();
                hv_PredictionColor.Dispose();
                hv_LineColors.Dispose();
                hv_ClassificationLabelIDGroundTruth.Dispose();
                hv_ClassificationLabelIDResult.Dispose();
                hv_MarginBottom.Dispose();
                hv_WindowCoordinates.Dispose();
                hv_CurrentWindowHeight.Dispose();
                hv__.Dispose();
                hv_MaxHeight.Dispose();
                hv_PredictionText.Dispose();
                hv_BoarderOffset.Dispose();
                hv_WindowImageRatioHeight.Dispose();
                hv_WindowImageRatioWidth.Dispose();
                hv_BoarderOffsetRow.Dispose();
                hv_BoarderOffsetCol.Dispose();
                hv_SelectedHeatmapMethod.Dispose();
                hv_DictHeatmap.Dispose();
                hv_MethodName.Dispose();
                hv_HeatmapKeys.Dispose();
                hv_HeatmapImageName.Dispose();
                hv_TargetClassID.Dispose();
                hv_Confidences.Dispose();
                hv_MaxDeviation.Dispose();
                hv_ClassificationLabelNameResult.Dispose();
                hv_TargetClassConfidence.Dispose();
                hv_ClassificationLabelNamesGroundTruth.Dispose();
                hv_BboxIDs.Dispose();
                hv_BboxColors.Dispose();
                hv_BboxIDsUniq.Dispose();
                hv_BboxConfidences.Dispose();
                hv_TextConf.Dispose();
                hv_BboxClassIndex.Dispose();
                hv_BboxColorsResults.Dispose();
                hv_BboxClassIndexUniq.Dispose();
                hv_BboxLabelIndex.Dispose();
                hv_BboxColorsBoth.Dispose();
                hv_BboxClassLabelIndexUniq.Dispose();
                hv_ColorsSegmentation.Dispose();
                hv_DrawMode.Dispose();
                hv_Width.Dispose();
                hv_ImageClassIDs.Dispose();
                hv_ImageClassIDsUniq.Dispose();
                hv_ColorsResults.Dispose();
                hv_GroundTruthIDs.Dispose();
                hv_ResultIDs.Dispose();
                hv_StringSegExcludeClassIDs.Dispose();
                hv_StringIndex.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_MinWeight.Dispose();
                hv_WeightsColors.Dispose();
                hv_ConfidenceColors.Dispose();
                hv_Indices.Dispose();
                hv_WindowHandleKeysNew.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();
                ho_AnomalyImage.Dispose();
                ho_AnomalyRegion.Dispose();
                ho_PredictionColorFrame.Dispose();
                ho_ImageHeatmap.Dispose();
                ho_ImageWeight.Dispose();
                ho_ImageConfidence.Dispose();
                ho_SegmentationImagGroundTruth.Dispose();
                ho_SegmentationImageResult.Dispose();
                ho_ImageAbsDiff.Dispose();
                ho_DiffRegion.Dispose();

                hv_ThresholdWidth.Dispose();
                hv_ScaleWindows.Dispose();
                hv_Font.Dispose();
                hv_FontSize.Dispose();
                hv_LineWidth.Dispose();
                hv_MapTransparency.Dispose();
                hv_MapColorBarWidth.Dispose();
                hv_AnomalyRegionThreshold.Dispose();
                hv_AnomalyClassificationThreshold.Dispose();
                hv_AnomalyRegionLabelColor.Dispose();
                hv_AnomalyColorTransparency.Dispose();
                hv_AnomalyRegionResultColor.Dispose();
                hv_SegMaxWeight.Dispose();
                hv_SegDraw.Dispose();
                hv_SegTransparency.Dispose();
                hv_SegExcludeClassIDs.Dispose();
                hv_BboxLabelColor.Dispose();
                hv_BboxDisplayConfidence.Dispose();
                hv_BboxTextColor.Dispose();
                hv_ShowBottomDesc.Dispose();
                hv_ShowLegend.Dispose();
                hv_ShowGroundTruthAnomalyRegions.Dispose();
                hv_ShowClassificationIDs.Dispose();
                hv_ShowClassificationColorFrame.Dispose();
                hv_ShowLabels.Dispose();
                hv_ShowDirection.Dispose();
                hv_HeatmapColorScheme.Dispose();
                hv_GenParamNames.Dispose();
                hv_ParamIndex.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();
                hv_SampleKeys.Dispose();
                hv_ResultKeys.Dispose();
                hv_ImageIDExists.Dispose();
                hv_ImageID.Dispose();
                hv_ImageIDString.Dispose();
                hv_ImageIDStringBraces.Dispose();
                hv_ImageIDStringCapital.Dispose();
                hv_NeededKeys.Dispose();
                hv_Index.Dispose();
                hv_DLDatasetInfoKeys.Dispose();
                hv_ClassNames.Dispose();
                hv_ClassIDs.Dispose();
                hv_Colors.Dispose();
                hv_ClassesLegend.Dispose();
                hv_PrevWindowCoordinates.Dispose();
                hv_Keys.Dispose();
                hv_Exception.Dispose();
                hv_MetaInfoIndex.Dispose();
                hv_MetaInfo.Dispose();
                hv_FlushValues.Dispose();
                hv_WindowHandleKeys.Dispose();
                hv_KeyIndex.Dispose();
                hv_WindowHandles.Dispose();
                hv_WindowIndex.Dispose();
                hv_FlushValue.Dispose();
                hv_WidthImage.Dispose();
                hv_HeightImage.Dispose();
                hv_CurrentWindowHandle.Dispose();
                hv_WindowImageRatio.Dispose();
                hv_AnomalyLabelGroundTruth.Dispose();
                hv_AnomalyLabelIDGroundTruth.Dispose();
                hv_AnomalyRegionExists.Dispose();
                hv_Text.Dispose();
                hv_AnomalyScore.Dispose();
                hv_AnomalyClassID.Dispose();
                hv_AnomalyRegionGroundTruthExists.Dispose();
                hv_PredictionColor.Dispose();
                hv_LineColors.Dispose();
                hv_ClassificationLabelIDGroundTruth.Dispose();
                hv_ClassificationLabelIDResult.Dispose();
                hv_MarginBottom.Dispose();
                hv_WindowCoordinates.Dispose();
                hv_CurrentWindowHeight.Dispose();
                hv__.Dispose();
                hv_MaxHeight.Dispose();
                hv_PredictionText.Dispose();
                hv_BoarderOffset.Dispose();
                hv_WindowImageRatioHeight.Dispose();
                hv_WindowImageRatioWidth.Dispose();
                hv_BoarderOffsetRow.Dispose();
                hv_BoarderOffsetCol.Dispose();
                hv_SelectedHeatmapMethod.Dispose();
                hv_DictHeatmap.Dispose();
                hv_MethodName.Dispose();
                hv_HeatmapKeys.Dispose();
                hv_HeatmapImageName.Dispose();
                hv_TargetClassID.Dispose();
                hv_Confidences.Dispose();
                hv_MaxDeviation.Dispose();
                hv_ClassificationLabelNameResult.Dispose();
                hv_TargetClassConfidence.Dispose();
                hv_ClassificationLabelNamesGroundTruth.Dispose();
                hv_BboxIDs.Dispose();
                hv_BboxColors.Dispose();
                hv_BboxIDsUniq.Dispose();
                hv_BboxConfidences.Dispose();
                hv_TextConf.Dispose();
                hv_BboxClassIndex.Dispose();
                hv_BboxColorsResults.Dispose();
                hv_BboxClassIndexUniq.Dispose();
                hv_BboxLabelIndex.Dispose();
                hv_BboxColorsBoth.Dispose();
                hv_BboxClassLabelIndexUniq.Dispose();
                hv_ColorsSegmentation.Dispose();
                hv_DrawMode.Dispose();
                hv_Width.Dispose();
                hv_ImageClassIDs.Dispose();
                hv_ImageClassIDsUniq.Dispose();
                hv_ColorsResults.Dispose();
                hv_GroundTruthIDs.Dispose();
                hv_ResultIDs.Dispose();
                hv_StringSegExcludeClassIDs.Dispose();
                hv_StringIndex.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_MinWeight.Dispose();
                hv_WeightsColors.Dispose();
                hv_ConfidenceColors.Dispose();
                hv_Indices.Dispose();
                hv_WindowHandleKeysNew.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Anomaly Detection
        // Short Description: Display the ground truth anomaly regions of the given DLSample. 
        public void dev_display_ground_truth_anomaly_regions(HTuple hv_SampleKeys, HTuple hv_DLSample,
            HTuple hv_CurrentWindowHandle, HTuple hv_LineWidth, HTuple hv_AnomalyRegionLabelColor,
            HTuple hv_AnomalyColorTransparency, out HTuple hv_AnomalyRegionExists)
        {



            // Local iconic variables 

            HObject ho_AnomalyImage = null, ho_AnomalyRegion = null;

            // Local control variables 

            HTuple hv_Red = new HTuple(), hv_Green = new HTuple();
            HTuple hv_Blue = new HTuple(), hv_Alpha = new HTuple();
            HTuple hv_InitialColor = new HTuple(), hv_IndexColor = new HTuple();
            HTuple hv_Color_RGBA = new HTuple(), hv_Area = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_AnomalyImage);
            HOperatorSet.GenEmptyObj(out ho_AnomalyRegion);
            hv_AnomalyRegionExists = new HTuple();
            try
            {
                //
                //This procedure visualizes the ground truth anomalies
                //if there is an anomaly_ground_truth in DLSample.
                //
                //Get current set color.
                hv_Red.Dispose(); hv_Green.Dispose(); hv_Blue.Dispose(); hv_Alpha.Dispose();
                HOperatorSet.GetRgba(hv_CurrentWindowHandle, out hv_Red, out hv_Green, out hv_Blue,
                    out hv_Alpha);
                hv_InitialColor.Dispose();
                hv_InitialColor = new HTuple();
                for (hv_IndexColor = 0; (int)hv_IndexColor <= (int)((new HTuple(hv_Red.TupleLength()
                    )) - 1); hv_IndexColor = (int)hv_IndexColor + 1)
                {
                    hv_Color_RGBA.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Color_RGBA = ((("#" + (((hv_Red.TupleSelect(
                            hv_IndexColor))).TupleString("2x"))) + (((hv_Green.TupleSelect(hv_IndexColor))).TupleString(
                            "2x"))) + (((hv_Blue.TupleSelect(hv_IndexColor))).TupleString("2x"))) + (((hv_Alpha.TupleSelect(
                            hv_IndexColor))).TupleString("2x"));
                    }
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleRegexpReplace(hv_Color_RGBA, (new HTuple(" ")).TupleConcat(
                            "replace_all"), "0", out ExpTmpOutVar_0);
                        hv_Color_RGBA.Dispose();
                        hv_Color_RGBA = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_InitialColor = hv_InitialColor.TupleConcat(
                                hv_Color_RGBA);
                            hv_InitialColor.Dispose();
                            hv_InitialColor = ExpTmpLocalVar_InitialColor;
                        }
                    }
                }
                //
                if ((int)(new HTuple(((hv_SampleKeys.TupleFind("anomaly_ground_truth"))).TupleNotEqual(
                    -1))) != 0)
                {
                    ho_AnomalyImage.Dispose();
                    HOperatorSet.GetDictObject(out ho_AnomalyImage, hv_DLSample, "anomaly_ground_truth");
                    ho_AnomalyRegion.Dispose();
                    HOperatorSet.Threshold(ho_AnomalyImage, out ho_AnomalyRegion, 1, 255);
                    //Get non-empty regions.
                    hv_Area.Dispose();
                    HOperatorSet.RegionFeatures(ho_AnomalyRegion, "area", out hv_Area);
                    if ((int)(new HTuple(hv_Area.TupleGreater(0))) != 0)
                    {
                        if (HDevWindowStack.IsOpen())
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_AnomalyRegionLabelColor + hv_AnomalyColorTransparency);
                            }
                        }
                        //Display the anomaly region.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetDraw(HDevWindowStack.GetActive(), "fill");
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_AnomalyRegion, HDevWindowStack.GetActive());
                        }
                    }
                    hv_AnomalyRegionExists.Dispose();
                    hv_AnomalyRegionExists = "true";
                }
                else
                {
                    hv_AnomalyRegionExists.Dispose();
                    hv_AnomalyRegionExists = "false";
                }
                //
                //Reset colors.
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_InitialColor);
                }
                //
                ho_AnomalyImage.Dispose();
                ho_AnomalyRegion.Dispose();

                hv_Red.Dispose();
                hv_Green.Dispose();
                hv_Blue.Dispose();
                hv_Alpha.Dispose();
                hv_InitialColor.Dispose();
                hv_IndexColor.Dispose();
                hv_Color_RGBA.Dispose();
                hv_Area.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_AnomalyImage.Dispose();
                ho_AnomalyRegion.Dispose();

                hv_Red.Dispose();
                hv_Green.Dispose();
                hv_Blue.Dispose();
                hv_Alpha.Dispose();
                hv_InitialColor.Dispose();
                hv_IndexColor.Dispose();
                hv_Color_RGBA.Dispose();
                hv_Area.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Output
        // Short Description: Display the ground truth bounding boxes of DLSample. 
        public void dev_display_ground_truth_detection(HTuple hv_DLSample, HTuple hv_SampleKeys,
            HTuple hv_LineWidthBbox, HTuple hv_ClassIDs, HTuple hv_BboxColors, HTuple hv_BboxLabelColor,
            HTuple hv_TextColor, HTuple hv_ShowLabels, HTuple hv_ShowDirection, HTuple hv_WindowHandle,
            out HTuple hv_BboxIDs)
        {



            // Local iconic variables 

            HObject ho_BboxRectangle = null, ho_OrientationArrows = null;
            HObject ho_RectangleSelected = null, ho_ArrowSelected = null;

            // Local control variables 

            HTuple hv_InstanceType = new HTuple(), hv_BboxRow1 = new HTuple();
            HTuple hv_BboxCol1 = new HTuple(), hv_BboxRow2 = new HTuple();
            HTuple hv_BboxCol2 = new HTuple(), hv_BboxLabels = new HTuple();
            HTuple hv_BboxRow = new HTuple(), hv_BboxCol = new HTuple();
            HTuple hv_BboxLength1 = new HTuple(), hv_BboxLength2 = new HTuple();
            HTuple hv_BboxPhi = new HTuple(), hv_LabelRow = new HTuple();
            HTuple hv_LabelCol = new HTuple(), hv_HeadSize = new HTuple();
            HTuple hv_BboxClassIDs = new HTuple(), hv_ContourStyle = new HTuple();
            HTuple hv_IndexBbox = new HTuple(), hv_ClassID = new HTuple();
            HTuple hv_TxtColor = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_BboxRectangle);
            HOperatorSet.GenEmptyObj(out ho_OrientationArrows);
            HOperatorSet.GenEmptyObj(out ho_RectangleSelected);
            HOperatorSet.GenEmptyObj(out ho_ArrowSelected);
            hv_BboxIDs = new HTuple();
            try
            {
                //
                //This procedure displays the ground truth bounding boxes of DLSample.
                //
                hv_InstanceType.Dispose();
                hv_InstanceType = "rectangle1";
                if ((int)(new HTuple(((hv_SampleKeys.TupleFind("bbox_row1"))).TupleNotEqual(
                    -1))) != 0)
                {
                    hv_BboxRow1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row1", out hv_BboxRow1);
                    hv_BboxCol1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col1", out hv_BboxCol1);
                    hv_BboxRow2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row2", out hv_BboxRow2);
                    hv_BboxCol2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col2", out hv_BboxCol2);
                    hv_BboxLabels.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_label_id", out hv_BboxLabels);
                }
                else if ((int)(new HTuple(((hv_SampleKeys.TupleFind("bbox_phi"))).TupleNotEqual(
                    -1))) != 0)
                {
                    hv_BboxRow.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row", out hv_BboxRow);
                    hv_BboxCol.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col", out hv_BboxCol);
                    hv_BboxLength1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_length1", out hv_BboxLength1);
                    hv_BboxLength2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_length2", out hv_BboxLength2);
                    hv_BboxPhi.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_phi", out hv_BboxPhi);
                    hv_BboxLabels.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_label_id", out hv_BboxLabels);
                    hv_InstanceType.Dispose();
                    hv_InstanceType = "rectangle2";
                }
                else
                {
                    throw new HalconException("Ground truth bounding box data could not be found in DLSample.");
                }
                if ((int)(new HTuple((new HTuple(hv_BboxLabels.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    //Generate bounding box XLDs.
                    if ((int)(new HTuple(hv_InstanceType.TupleEqual("rectangle1"))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BboxPhi.Dispose();
                            HOperatorSet.TupleGenConst(new HTuple(hv_BboxRow1.TupleLength()), 0.0,
                                out hv_BboxPhi);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_BboxRectangle.Dispose();
                            HOperatorSet.GenRectangle2ContourXld(out ho_BboxRectangle, 0.5 * (hv_BboxRow1 + hv_BboxRow2),
                                0.5 * (hv_BboxCol1 + hv_BboxCol2), hv_BboxPhi, 0.5 * (hv_BboxCol2 - hv_BboxCol1),
                                0.5 * (hv_BboxRow2 - hv_BboxRow1));
                        }
                        hv_LabelRow.Dispose();
                        hv_LabelRow = new HTuple(hv_BboxRow1);
                        hv_LabelCol.Dispose();
                        hv_LabelCol = new HTuple(hv_BboxCol1);
                    }
                    else
                    {
                        ho_BboxRectangle.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_BboxRectangle, hv_BboxRow,
                            hv_BboxCol, hv_BboxPhi, hv_BboxLength1, hv_BboxLength2);
                        hv_LabelRow.Dispose();
                        hv_LabelRow = new HTuple(hv_BboxRow);
                        hv_LabelCol.Dispose();
                        hv_LabelCol = new HTuple(hv_BboxCol);
                        if ((int)(hv_ShowDirection) != 0)
                        {
                            hv_HeadSize.Dispose();
                            hv_HeadSize = 20.0;
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_OrientationArrows.Dispose();
                                gen_arrow_contour_xld(out ho_OrientationArrows, hv_BboxRow, hv_BboxCol,
                                    hv_BboxRow - ((hv_BboxLength1 + hv_HeadSize) * (hv_BboxPhi.TupleSin())),
                                    hv_BboxCol + ((hv_BboxLength1 + hv_HeadSize) * (hv_BboxPhi.TupleCos())),
                                    hv_HeadSize, hv_HeadSize);
                            }
                        }
                    }
                    //
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), hv_LineWidthBbox);
                    }
                    //
                    //Collect the ClassIDs of the bounding boxes.
                    hv_BboxIDs.Dispose();
                    hv_BboxIDs = new HTuple();
                    hv_BboxClassIDs.Dispose();
                    hv_BboxClassIDs = new HTuple();
                    //
                    //Draw the bounding boxes.
                    hv_ContourStyle.Dispose();
                    HOperatorSet.GetContourStyle(hv_WindowHandle, out hv_ContourStyle);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetContourStyle(HDevWindowStack.GetActive(), "stroke_and_fill");
                    }
                    for (hv_IndexBbox = 0; (int)hv_IndexBbox <= (int)((new HTuple(hv_BboxLabels.TupleLength()
                        )) - 1); hv_IndexBbox = (int)hv_IndexBbox + 1)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_RectangleSelected.Dispose();
                            HOperatorSet.SelectObj(ho_BboxRectangle, out ho_RectangleSelected, hv_IndexBbox + 1);
                        }
                        hv_ClassID.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ClassID = hv_ClassIDs.TupleFind(
                                hv_BboxLabels.TupleSelect(hv_IndexBbox));
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BboxClassIDs = hv_BboxClassIDs.TupleConcat(
                                    hv_ClassID);
                                hv_BboxClassIDs.Dispose();
                                hv_BboxClassIDs = ExpTmpLocalVar_BboxClassIDs;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BboxIDs = hv_BboxIDs.TupleConcat(
                                    hv_ClassID);
                                hv_BboxIDs.Dispose();
                                hv_BboxIDs = ExpTmpLocalVar_BboxIDs;
                            }
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetColor(HDevWindowStack.GetActive(), (hv_BboxColors.TupleSelect(
                                    hv_ClassID)) + "60");
                            }
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_RectangleSelected, HDevWindowStack.GetActive()
                                );
                        }
                        if ((int)((new HTuple(hv_InstanceType.TupleEqual("rectangle2"))).TupleAnd(
                            hv_ShowDirection)) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_ArrowSelected.Dispose();
                                HOperatorSet.SelectObj(ho_OrientationArrows, out ho_ArrowSelected, hv_IndexBbox + 1);
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.SetColor(HDevWindowStack.GetActive(), (hv_BboxColors.TupleSelect(
                                        hv_ClassID)) + "FF");
                                }
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_ArrowSelected, HDevWindowStack.GetActive());
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.SetColor(HDevWindowStack.GetActive(), (hv_BboxColors.TupleSelect(
                                        hv_ClassID)) + "60");
                                }
                            }
                        }
                    }
                    //
                    //Write text to the bounding boxes.
                    if ((int)(hv_ShowLabels) != 0)
                    {
                        //For better visibility the text is displayed after all bounding boxes are drawn.
                        for (hv_IndexBbox = 0; (int)hv_IndexBbox <= (int)((new HTuple(hv_BboxLabels.TupleLength()
                            )) - 1); hv_IndexBbox = (int)hv_IndexBbox + 1)
                        {
                            hv_ClassID.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ClassID = hv_BboxClassIDs.TupleSelect(
                                    hv_IndexBbox);
                            }
                            if ((int)(new HTuple(hv_TextColor.TupleEqual(""))) != 0)
                            {
                                hv_TxtColor.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_TxtColor = hv_BboxColors.TupleSelect(
                                        hv_ClassID);
                                }
                            }
                            else
                            {
                                hv_TxtColor.Dispose();
                                hv_TxtColor = new HTuple(hv_TextColor);
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_BboxLabels.TupleSelect(
                                        hv_IndexBbox), "image", hv_LabelRow.TupleSelect(hv_IndexBbox),
                                        hv_LabelCol.TupleSelect(hv_IndexBbox), hv_TextColor, ((new HTuple("box_color")).TupleConcat(
                                        "shadow")).TupleConcat("border_radius"), hv_BboxLabelColor.TupleConcat(
                                        (new HTuple("false")).TupleConcat(0)));
                                }
                            }
                        }
                    }
                    //
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetContourStyle(HDevWindowStack.GetActive(), hv_ContourStyle);
                    }
                }
                else
                {
                    //Do nothing if there are no ground truth bounding boxes.
                    hv_BboxIDs.Dispose();
                    hv_BboxIDs = new HTuple();
                }
                //
                ho_BboxRectangle.Dispose();
                ho_OrientationArrows.Dispose();
                ho_RectangleSelected.Dispose();
                ho_ArrowSelected.Dispose();

                hv_InstanceType.Dispose();
                hv_BboxRow1.Dispose();
                hv_BboxCol1.Dispose();
                hv_BboxRow2.Dispose();
                hv_BboxCol2.Dispose();
                hv_BboxLabels.Dispose();
                hv_BboxRow.Dispose();
                hv_BboxCol.Dispose();
                hv_BboxLength1.Dispose();
                hv_BboxLength2.Dispose();
                hv_BboxPhi.Dispose();
                hv_LabelRow.Dispose();
                hv_LabelCol.Dispose();
                hv_HeadSize.Dispose();
                hv_BboxClassIDs.Dispose();
                hv_ContourStyle.Dispose();
                hv_IndexBbox.Dispose();
                hv_ClassID.Dispose();
                hv_TxtColor.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_BboxRectangle.Dispose();
                ho_OrientationArrows.Dispose();
                ho_RectangleSelected.Dispose();
                ho_ArrowSelected.Dispose();

                hv_InstanceType.Dispose();
                hv_BboxRow1.Dispose();
                hv_BboxCol1.Dispose();
                hv_BboxRow2.Dispose();
                hv_BboxCol2.Dispose();
                hv_BboxLabels.Dispose();
                hv_BboxRow.Dispose();
                hv_BboxCol.Dispose();
                hv_BboxLength1.Dispose();
                hv_BboxLength2.Dispose();
                hv_BboxPhi.Dispose();
                hv_LabelRow.Dispose();
                hv_LabelCol.Dispose();
                hv_HeadSize.Dispose();
                hv_BboxClassIDs.Dispose();
                hv_ContourStyle.Dispose();
                hv_IndexBbox.Dispose();
                hv_ClassID.Dispose();
                hv_TxtColor.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Output
        // Short Description: Display a color bar next to an image. 
        public void dev_display_map_color_bar(HTuple hv_ImageWidth, HTuple hv_ImageHeight,
            HTuple hv_MapColorBarWidth, HTuple hv_Colors, HTuple hv_MaxValue, HTuple hv_WindowImageRatio,
            HTuple hv_WindowHandle)
        {



            // Local iconic variables 

            HObject ho_Rectangle = null;

            // Local control variables 

            HTuple hv_ClipRegion = new HTuple(), hv_ColorIndex = new HTuple();
            HTuple hv_RectHeight = new HTuple(), hv_DrawMode = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv__ = new HTuple(), hv_TextHeight = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Text = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            try
            {
                //
                //This procedure displays a color bar next to the image
                //specified with ImageWidth and ImageHeight.
                //
                hv_ClipRegion.Dispose();
                HOperatorSet.GetSystem("clip_region", out hv_ClipRegion);
                HOperatorSet.SetSystem("clip_region", "false");
                //
                //Display the color bar.
                hv_ColorIndex.Dispose();
                hv_ColorIndex = 0;
                hv_RectHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_RectHeight = (1.0 * hv_ImageHeight) / (new HTuple(hv_Colors.TupleLength()
                        ));
                }
                //Set draw mode to fill
                hv_DrawMode.Dispose();
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetDraw(HDevWindowStack.GetActive(), "fill");
                }
                HTuple end_val13 = 0;
                HTuple step_val13 = -hv_RectHeight;
                for (hv_Row = hv_ImageHeight - 1; hv_Row.Continue(end_val13, step_val13); hv_Row = hv_Row.TupleAdd(step_val13))
                {
                    //The color bar consists of multiple rectangle1.
                    hv_Row1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row1 = hv_Row - hv_RectHeight;
                    }
                    hv_Column1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Column1 = hv_ImageWidth + (20 / hv_WindowImageRatio);
                    }
                    hv_Row2.Dispose();
                    hv_Row2 = new HTuple(hv_Row);
                    hv_Column2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Column2 = (hv_ImageWidth + 20) + (hv_MapColorBarWidth / hv_WindowImageRatio);
                    }
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row1, hv_Column1, hv_Row2,
                        hv_Column2);
                    if (HDevWindowStack.IsOpen())
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_Colors.TupleSelect(
                                hv_ColorIndex));
                        }
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_Rectangle, HDevWindowStack.GetActive());
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ColorIndex = hv_ColorIndex + 1;
                            hv_ColorIndex.Dispose();
                            hv_ColorIndex = ExpTmpLocalVar_ColorIndex;
                        }
                    }
                }
                //
                //Display labels for color bar.
                hv__.Dispose(); hv__.Dispose(); hv__.Dispose(); hv_TextHeight.Dispose();
                HOperatorSet.GetStringExtents(hv_WindowHandle, "0123456789", out hv__, out hv__,
                    out hv__, out hv_TextHeight);
                for (hv_Index = (double)(0); (double)hv_Index <= 1; hv_Index = (double)hv_Index + 0.2)
                {
                    hv_Text.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Text = ((hv_MaxValue - (hv_Index * hv_MaxValue))).TupleString(
                            ".1f");
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "image", hv_Index * (hv_ImageHeight - (2 * (hv_TextHeight / hv_WindowImageRatio))),
                                hv_ImageWidth + (40 / hv_WindowImageRatio), "black", "box", "false");
                        }
                    }
                }
                //
                HOperatorSet.SetSystem("clip_region", hv_ClipRegion);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetDraw(HDevWindowStack.GetActive(), hv_DrawMode);
                }
                ho_Rectangle.Dispose();

                hv_ClipRegion.Dispose();
                hv_ColorIndex.Dispose();
                hv_RectHeight.Dispose();
                hv_DrawMode.Dispose();
                hv_Row.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv__.Dispose();
                hv_TextHeight.Dispose();
                hv_Index.Dispose();
                hv_Text.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Rectangle.Dispose();

                hv_ClipRegion.Dispose();
                hv_ColorIndex.Dispose();
                hv_RectHeight.Dispose();
                hv_DrawMode.Dispose();
                hv_Row.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv__.Dispose();
                hv_TextHeight.Dispose();
                hv_Index.Dispose();
                hv_Text.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Anomaly Detection
        // Short Description: Display the detected anomaly regions. 
        public void dev_display_result_anomaly_regions(HObject ho_AnomalyImage, out HObject ho_AnomalyRegion,
            HTuple hv_CurrentWindowHandle, HTuple hv_AnomalyRegionThreshold, HTuple hv_LineWidth,
            HTuple hv_AnomalyRegionResultColor)
        {




            // Local iconic variables 

            // Local control variables 

            HTuple hv_Red = new HTuple(), hv_Green = new HTuple();
            HTuple hv_Blue = new HTuple(), hv_Alpha = new HTuple();
            HTuple hv_InitialColor = new HTuple(), hv_IndexColor = new HTuple();
            HTuple hv_Color_RGBA = new HTuple(), hv_Area = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_AnomalyRegion);
            try
            {
                //
                //This procedure displays the result anomaly regions
                //given in DLResult as anomaly_image.
                //
                //Get current set color.
                hv_Red.Dispose(); hv_Green.Dispose(); hv_Blue.Dispose(); hv_Alpha.Dispose();
                HOperatorSet.GetRgba(hv_CurrentWindowHandle, out hv_Red, out hv_Green, out hv_Blue,
                    out hv_Alpha);
                hv_InitialColor.Dispose();
                hv_InitialColor = new HTuple();
                for (hv_IndexColor = 0; (int)hv_IndexColor <= (int)((new HTuple(hv_Red.TupleLength()
                    )) - 1); hv_IndexColor = (int)hv_IndexColor + 1)
                {
                    hv_Color_RGBA.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Color_RGBA = ((("#" + (((hv_Red.TupleSelect(
                            hv_IndexColor))).TupleString("2x"))) + (((hv_Green.TupleSelect(hv_IndexColor))).TupleString(
                            "2x"))) + (((hv_Blue.TupleSelect(hv_IndexColor))).TupleString("2x"))) + (((hv_Alpha.TupleSelect(
                            hv_IndexColor))).TupleString("2x"));
                    }
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleRegexpReplace(hv_Color_RGBA, (new HTuple(" ")).TupleConcat(
                            "replace_all"), "0", out ExpTmpOutVar_0);
                        hv_Color_RGBA.Dispose();
                        hv_Color_RGBA = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_InitialColor = hv_InitialColor.TupleConcat(
                                hv_Color_RGBA);
                            hv_InitialColor.Dispose();
                            hv_InitialColor = ExpTmpLocalVar_InitialColor;
                        }
                    }
                }
                //
                if ((int)((new HTuple(hv_AnomalyRegionThreshold.TupleGreater(1))).TupleOr(new HTuple(hv_AnomalyRegionThreshold.TupleLess(
                    0)))) != 0)
                {
                    throw new HalconException("Selected 'anomaly_region_threshold' out of range. It must be between 0 and 1.");
                }
                ho_AnomalyRegion.Dispose();
                HOperatorSet.Threshold(ho_AnomalyImage, out ho_AnomalyRegion, hv_AnomalyRegionThreshold,
                    1);
                //
                //Display anomaly regions.
                //Get non-empty regions.
                hv_Area.Dispose();
                HOperatorSet.RegionFeatures(ho_AnomalyRegion, "area", out hv_Area);
                //
                //Display all non-empty class regions in distinct colors.
                if ((int)(new HTuple(hv_Area.TupleGreater(0))) != 0)
                {
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_AnomalyRegionResultColor);
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), hv_LineWidth);
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetDraw(HDevWindowStack.GetActive(), "margin");
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_AnomalyRegion, HDevWindowStack.GetActive());
                    }
                }
                //
                //Reset colors.
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_InitialColor);
                }
                //

                hv_Red.Dispose();
                hv_Green.Dispose();
                hv_Blue.Dispose();
                hv_Alpha.Dispose();
                hv_InitialColor.Dispose();
                hv_IndexColor.Dispose();
                hv_Color_RGBA.Dispose();
                hv_Area.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Red.Dispose();
                hv_Green.Dispose();
                hv_Blue.Dispose();
                hv_Alpha.Dispose();
                hv_InitialColor.Dispose();
                hv_IndexColor.Dispose();
                hv_Color_RGBA.Dispose();
                hv_Area.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Output
        // Short Description: Display result bounding boxes. 
        public void dev_display_result_detection(HTuple hv_DLResult, HTuple hv_ResultKeys,
            HTuple hv_LineWidthBbox, HTuple hv_ClassIDs, HTuple hv_TextConf, HTuple hv_Colors,
            HTuple hv_BoxLabelColor, HTuple hv_WindowImageRatio, HTuple hv_TextPositionRow,
            HTuple hv_TextColor, HTuple hv_ShowLabels, HTuple hv_ShowDirection, HTuple hv_WindowHandle,
            out HTuple hv_BboxIDs)
        {



            // Local iconic variables 

            HObject ho_BboxRectangle = null, ho_OrientationArrows = null;
            HObject ho_RectangleSelected = null, ho_ArrowSelected = null;

            // Local control variables 

            HTuple hv_InstanceType = new HTuple(), hv_BboxRow1 = new HTuple();
            HTuple hv_BboxCol1 = new HTuple(), hv_BboxRow2 = new HTuple();
            HTuple hv_BboxCol2 = new HTuple(), hv_BboxClasses = new HTuple();
            HTuple hv_BboxRow = new HTuple(), hv_BboxCol = new HTuple();
            HTuple hv_BboxLength1 = new HTuple(), hv_BboxLength2 = new HTuple();
            HTuple hv_BboxPhi = new HTuple(), hv_LabelRow1 = new HTuple();
            HTuple hv_LabelRow2 = new HTuple(), hv_LabelCol = new HTuple();
            HTuple hv_HeadSize = new HTuple(), hv_DrawMode = new HTuple();
            HTuple hv_Style = new HTuple(), hv_BboxClassIDs = new HTuple();
            HTuple hv_IndexBbox = new HTuple(), hv_ClassID = new HTuple();
            HTuple hv_LineWidth = new HTuple(), hv_Text = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv__ = new HTuple(), hv_TextRow = new HTuple();
            HTuple hv_TxtColor = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_BboxRectangle);
            HOperatorSet.GenEmptyObj(out ho_OrientationArrows);
            HOperatorSet.GenEmptyObj(out ho_RectangleSelected);
            HOperatorSet.GenEmptyObj(out ho_ArrowSelected);
            hv_BboxIDs = new HTuple();
            try
            {
                //
                //This procedure displays the bounding boxes defined by DLResult.
                //The ClassIDs are necessary to display bounding boxes from the same class
                //always with the same color.
                //
                hv_InstanceType.Dispose();
                hv_InstanceType = "rectangle1";
                if ((int)(new HTuple(((hv_ResultKeys.TupleFind("bbox_row1"))).TupleNotEqual(
                    -1))) != 0)
                {
                    hv_BboxRow1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_row1", out hv_BboxRow1);
                    hv_BboxCol1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_col1", out hv_BboxCol1);
                    hv_BboxRow2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_row2", out hv_BboxRow2);
                    hv_BboxCol2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_col2", out hv_BboxCol2);
                    hv_BboxClasses.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_class_id", out hv_BboxClasses);
                }
                else if ((int)(new HTuple(((hv_ResultKeys.TupleFind("bbox_phi"))).TupleNotEqual(
                    -1))) != 0)
                {
                    hv_BboxRow.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_row", out hv_BboxRow);
                    hv_BboxCol.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_col", out hv_BboxCol);
                    hv_BboxLength1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_length1", out hv_BboxLength1);
                    hv_BboxLength2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_length2", out hv_BboxLength2);
                    hv_BboxPhi.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_phi", out hv_BboxPhi);
                    hv_BboxClasses.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_class_id", out hv_BboxClasses);
                    hv_InstanceType.Dispose();
                    hv_InstanceType = "rectangle2";
                }
                else
                {
                    throw new HalconException("Result bounding box data could not be found in DLResult.");
                }
                if ((int)(new HTuple((new HTuple(hv_BboxClasses.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    //Generate bounding box XLDs.
                    if ((int)(new HTuple(hv_InstanceType.TupleEqual("rectangle1"))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BboxPhi.Dispose();
                            HOperatorSet.TupleGenConst(new HTuple(hv_BboxRow1.TupleLength()), 0.0,
                                out hv_BboxPhi);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_BboxRectangle.Dispose();
                            HOperatorSet.GenRectangle2ContourXld(out ho_BboxRectangle, 0.5 * (hv_BboxRow1 + hv_BboxRow2),
                                0.5 * (hv_BboxCol1 + hv_BboxCol2), hv_BboxPhi, 0.5 * (hv_BboxCol2 - hv_BboxCol1),
                                0.5 * (hv_BboxRow2 - hv_BboxRow1));
                        }
                        hv_LabelRow1.Dispose();
                        hv_LabelRow1 = new HTuple(hv_BboxRow1);
                        hv_LabelRow2.Dispose();
                        hv_LabelRow2 = new HTuple(hv_BboxRow2);
                        hv_LabelCol.Dispose();
                        hv_LabelCol = new HTuple(hv_BboxCol1);
                    }
                    else
                    {
                        ho_BboxRectangle.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_BboxRectangle, hv_BboxRow,
                            hv_BboxCol, hv_BboxPhi, hv_BboxLength1, hv_BboxLength2);
                        hv_LabelRow1.Dispose();
                        hv_LabelRow1 = new HTuple(hv_BboxRow);
                        hv_LabelRow2.Dispose();
                        hv_LabelRow2 = new HTuple(hv_BboxRow);
                        hv_LabelCol.Dispose();
                        hv_LabelCol = new HTuple(hv_BboxCol);
                        if ((int)(hv_ShowDirection) != 0)
                        {
                            hv_HeadSize.Dispose();
                            hv_HeadSize = 20.0;
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_OrientationArrows.Dispose();
                                gen_arrow_contour_xld(out ho_OrientationArrows, hv_BboxRow, hv_BboxCol,
                                    hv_BboxRow - ((hv_BboxLength1 + hv_HeadSize) * (hv_BboxPhi.TupleSin())),
                                    hv_BboxCol + ((hv_BboxLength1 + hv_HeadSize) * (hv_BboxPhi.TupleCos())),
                                    hv_HeadSize, hv_HeadSize);
                            }
                        }
                    }
                    //
                    hv_DrawMode.Dispose();
                    HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetDraw(HDevWindowStack.GetActive(), "margin");
                    }
                    hv_Style.Dispose();
                    HOperatorSet.GetLineStyle(hv_WindowHandle, out hv_Style);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), hv_LineWidthBbox);
                    }
                    //
                    //Collect ClassIDs of the bounding boxes.
                    hv_BboxIDs.Dispose();
                    hv_BboxIDs = new HTuple();
                    hv_BboxClassIDs.Dispose();
                    hv_BboxClassIDs = new HTuple();
                    //
                    //Draw bounding boxes.
                    for (hv_IndexBbox = 0; (int)hv_IndexBbox <= (int)((new HTuple(hv_BboxClasses.TupleLength()
                        )) - 1); hv_IndexBbox = (int)hv_IndexBbox + 1)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_RectangleSelected.Dispose();
                            HOperatorSet.SelectObj(ho_BboxRectangle, out ho_RectangleSelected, hv_IndexBbox + 1);
                        }
                        hv_ClassID.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ClassID = hv_ClassIDs.TupleFind(
                                hv_BboxClasses.TupleSelect(hv_IndexBbox));
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BboxClassIDs = hv_BboxClassIDs.TupleConcat(
                                    hv_ClassID);
                                hv_BboxClassIDs.Dispose();
                                hv_BboxClassIDs = ExpTmpLocalVar_BboxClassIDs;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BboxIDs = hv_BboxIDs.TupleConcat(
                                    hv_ClassID);
                                hv_BboxIDs.Dispose();
                                hv_BboxIDs = ExpTmpLocalVar_BboxIDs;
                            }
                        }
                        hv_LineWidth.Dispose();
                        HOperatorSet.GetLineWidth(hv_WindowHandle, out hv_LineWidth);
                        if (HDevWindowStack.IsOpen())
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), ((hv_LineWidth + 2)).TupleInt()
                                    );
                            }
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetColor(HDevWindowStack.GetActive(), "black");
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_RectangleSelected, HDevWindowStack.GetActive()
                                );
                        }
                        if ((int)((new HTuple(hv_InstanceType.TupleEqual("rectangle2"))).TupleAnd(
                            hv_ShowDirection)) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_ArrowSelected.Dispose();
                                HOperatorSet.SelectObj(ho_OrientationArrows, out ho_ArrowSelected, hv_IndexBbox + 1);
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_ArrowSelected, HDevWindowStack.GetActive());
                            }
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), hv_LineWidth.TupleInt()
                                    );
                            }
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_Colors.TupleSelect(
                                    hv_ClassID));
                            }
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_RectangleSelected, HDevWindowStack.GetActive()
                                );
                        }
                        if ((int)((new HTuple(hv_InstanceType.TupleEqual("rectangle2"))).TupleAnd(
                            hv_ShowDirection)) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_ArrowSelected, HDevWindowStack.GetActive());
                            }
                        }
                    }
                    //
                    //Draw text of bounding boxes.
                    if ((int)(hv_ShowLabels) != 0)
                    {
                        //For better visibility the text is displayed after all bounding boxes are drawn.
                        for (hv_IndexBbox = 0; (int)hv_IndexBbox <= (int)((new HTuple(hv_BboxClasses.TupleLength()
                            )) - 1); hv_IndexBbox = (int)hv_IndexBbox + 1)
                        {
                            hv_ClassID.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ClassID = hv_BboxClassIDs.TupleSelect(
                                    hv_IndexBbox);
                            }
                            hv_Text.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Text = (hv_BboxClasses.TupleSelect(
                                    hv_IndexBbox)) + (hv_TextConf.TupleSelect(hv_IndexBbox));
                            }
                            hv_Ascent.Dispose(); hv_Descent.Dispose(); hv__.Dispose(); hv__.Dispose();
                            HOperatorSet.GetStringExtents(hv_WindowHandle, hv_Text, out hv_Ascent,
                                out hv_Descent, out hv__, out hv__);
                            if ((int)(new HTuple(hv_TextPositionRow.TupleEqual("bottom"))) != 0)
                            {
                                hv_TextRow.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_TextRow = (hv_LabelRow2.TupleSelect(
                                        hv_IndexBbox)) - ((hv_Ascent + hv_Descent) / hv_WindowImageRatio);
                                }
                            }
                            else
                            {
                                hv_TextRow.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_TextRow = hv_LabelRow1.TupleSelect(
                                        hv_IndexBbox);
                                }
                            }
                            if ((int)(new HTuple(hv_TextColor.TupleEqual(""))) != 0)
                            {
                                hv_TxtColor.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_TxtColor = hv_Colors.TupleSelect(
                                        hv_ClassID);
                                }
                            }
                            else
                            {
                                hv_TxtColor.Dispose();
                                hv_TxtColor = new HTuple(hv_TextColor);
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "image",
                                        hv_TextRow, hv_LabelCol.TupleSelect(hv_IndexBbox), hv_TxtColor,
                                        ((new HTuple("box_color")).TupleConcat("shadow")).TupleConcat("border_radius"),
                                        hv_BoxLabelColor.TupleConcat((new HTuple("false")).TupleConcat(
                                        0)));
                                }
                            }
                        }
                    }
                    //
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetDraw(HDevWindowStack.GetActive(), hv_DrawMode);
                    }
                    HOperatorSet.SetLineStyle(hv_WindowHandle, hv_Style);
                }
                else
                {
                    //Do nothing if no results are present.
                    hv_BboxIDs.Dispose();
                    hv_BboxIDs = new HTuple();
                }
                //
                ho_BboxRectangle.Dispose();
                ho_OrientationArrows.Dispose();
                ho_RectangleSelected.Dispose();
                ho_ArrowSelected.Dispose();

                hv_InstanceType.Dispose();
                hv_BboxRow1.Dispose();
                hv_BboxCol1.Dispose();
                hv_BboxRow2.Dispose();
                hv_BboxCol2.Dispose();
                hv_BboxClasses.Dispose();
                hv_BboxRow.Dispose();
                hv_BboxCol.Dispose();
                hv_BboxLength1.Dispose();
                hv_BboxLength2.Dispose();
                hv_BboxPhi.Dispose();
                hv_LabelRow1.Dispose();
                hv_LabelRow2.Dispose();
                hv_LabelCol.Dispose();
                hv_HeadSize.Dispose();
                hv_DrawMode.Dispose();
                hv_Style.Dispose();
                hv_BboxClassIDs.Dispose();
                hv_IndexBbox.Dispose();
                hv_ClassID.Dispose();
                hv_LineWidth.Dispose();
                hv_Text.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv__.Dispose();
                hv_TextRow.Dispose();
                hv_TxtColor.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_BboxRectangle.Dispose();
                ho_OrientationArrows.Dispose();
                ho_RectangleSelected.Dispose();
                ho_ArrowSelected.Dispose();

                hv_InstanceType.Dispose();
                hv_BboxRow1.Dispose();
                hv_BboxCol1.Dispose();
                hv_BboxRow2.Dispose();
                hv_BboxCol2.Dispose();
                hv_BboxClasses.Dispose();
                hv_BboxRow.Dispose();
                hv_BboxCol.Dispose();
                hv_BboxLength1.Dispose();
                hv_BboxLength2.Dispose();
                hv_BboxPhi.Dispose();
                hv_LabelRow1.Dispose();
                hv_LabelRow2.Dispose();
                hv_LabelCol.Dispose();
                hv_HeadSize.Dispose();
                hv_DrawMode.Dispose();
                hv_Style.Dispose();
                hv_BboxClassIDs.Dispose();
                hv_IndexBbox.Dispose();
                hv_ClassID.Dispose();
                hv_LineWidth.Dispose();
                hv_Text.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv__.Dispose();
                hv_TextRow.Dispose();
                hv_TxtColor.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Output
        // Short Description: Display the ground truth/result segmentation as regions. 
        public void dev_display_segmentation_regions(HObject ho_SegmentationImage, HTuple hv_ClassIDs,
            HTuple hv_ColorsSegmentation, HTuple hv_ExcludeClassIDs, out HTuple hv_ImageClassIDs)
        {




            // Local iconic variables 

            HObject ho_Regions, ho_SelectedRegion = null;

            // Local control variables 

            HTuple hv_IncludedClassIDs = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Index = new HTuple();
            HTuple hv_ClassID = new HTuple(), hv_IndexColor = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegion);
            hv_ImageClassIDs = new HTuple();
            try
            {
                //
                //This procedure displays the ground truth/result segmentation
                //given in SegmentationImage as regions. The ClassIDs are necessary to
                //display ground truth/result segmentations from the same class
                //always with the same color. It is possible to exclude certain ClassIDs
                //from being displayed. The displayed classes are returned in ImageClassIDs.
                //
                //
                //Remove excluded class IDs from the list.
                hv_IncludedClassIDs.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_IncludedClassIDs = hv_ClassIDs.TupleDifference(
                        hv_ExcludeClassIDs);
                }
                //
                //Get a region for each class ID.
                ho_Regions.Dispose();
                HOperatorSet.Threshold(ho_SegmentationImage, out ho_Regions, hv_IncludedClassIDs,
                    hv_IncludedClassIDs);
                //
                //Get classes with non-empty regions.
                hv_Area.Dispose();
                HOperatorSet.RegionFeatures(ho_Regions, "area", out hv_Area);
                hv_ImageClassIDs.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageClassIDs = hv_IncludedClassIDs.TupleSelectMask(
                        hv_Area.TupleGreaterElem(0));
                }
                //
                //Display all non-empty class regions in distinct colors.
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_IncludedClassIDs.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    if ((int)(new HTuple(((hv_Area.TupleSelect(hv_Index))).TupleGreater(0))) != 0)
                    {
                        //Use class ID to determine region color.
                        hv_ClassID.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ClassID = hv_IncludedClassIDs.TupleSelect(
                                hv_Index);
                        }
                        hv_IndexColor.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IndexColor = hv_ClassIDs.TupleFindFirst(
                                hv_ClassID);
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_ColorsSegmentation.TupleSelect(
                                    hv_IndexColor));
                            }
                        }
                        //Display the segmentation region.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_SelectedRegion.Dispose();
                            HOperatorSet.SelectObj(ho_Regions, out ho_SelectedRegion, hv_Index + 1);
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_SelectedRegion, HDevWindowStack.GetActive());
                        }
                    }
                }
                ho_Regions.Dispose();
                ho_SelectedRegion.Dispose();

                hv_IncludedClassIDs.Dispose();
                hv_Area.Dispose();
                hv_Index.Dispose();
                hv_ClassID.Dispose();
                hv_IndexColor.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Regions.Dispose();
                ho_SelectedRegion.Dispose();

                hv_IncludedClassIDs.Dispose();
                hv_Area.Dispose();
                hv_Index.Dispose();
                hv_ClassID.Dispose();
                hv_IndexColor.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Output
        // Short Description: Display a map of weights. 
        public void dev_display_weight_regions(HObject ho_ImageWeight, HTuple hv_DrawTransparency,
            HTuple hv_SegMaxWeight, out HTuple hv_Colors)
        {




            // Local iconic variables 

            HObject ho_Domain, ho_WeightsRegion = null;

            // Local control variables 

            HTuple hv_NumColors = new HTuple(), hv_WeightsColorsAlpha = new HTuple();
            HTuple hv_Rows = new HTuple(), hv_Columns = new HTuple();
            HTuple hv_GrayVal = new HTuple(), hv_GrayValWeight = new HTuple();
            HTuple hv_ColorIndex = new HTuple(), hv_ClassColor = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Domain);
            HOperatorSet.GenEmptyObj(out ho_WeightsRegion);
            hv_Colors = new HTuple();
            try
            {
                //
                //This procedure displays a map of the weights
                //given in ImageWeight as regions.
                //The transparency can be adjusted.
                //The used colors are returned.
                //
                //Define colors.
                hv_NumColors.Dispose();
                hv_NumColors = 20;
                hv_Colors.Dispose();
                get_distinct_colors(hv_NumColors, 0, 0, 160, out hv_Colors);
                {
                    HTuple ExpTmpOutVar_0;
                    HOperatorSet.TupleInverse(hv_Colors, out ExpTmpOutVar_0);
                    hv_Colors.Dispose();
                    hv_Colors = ExpTmpOutVar_0;
                }
                hv_WeightsColorsAlpha.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WeightsColorsAlpha = hv_Colors + hv_DrawTransparency;
                }
                //
                //Get gay values of ImageWeight.
                ho_Domain.Dispose();
                HOperatorSet.GetDomain(ho_ImageWeight, out ho_Domain);
                hv_Rows.Dispose(); hv_Columns.Dispose();
                HOperatorSet.GetRegionPoints(ho_Domain, out hv_Rows, out hv_Columns);
                hv_GrayVal.Dispose();
                HOperatorSet.GetGrayval(ho_ImageWeight, hv_Rows, hv_Columns, out hv_GrayVal);
                //
                //Check that the gray values of the image
                //are below the specified maximum.
                if ((int)(new HTuple(((hv_GrayVal.TupleMax())).TupleGreater(hv_SegMaxWeight))) != 0)
                {
                    throw new HalconException(((("The maximum weight (" + (hv_GrayVal.TupleMax()
                        )) + ") in the weight image is greater than the given SegMaxWeight (") + hv_SegMaxWeight) + ").");
                }
                //
                while ((int)(new HTuple(hv_GrayVal.TupleNotEqual(new HTuple()))) != 0)
                {
                    //Go through all gray value 'groups',
                    //starting from the maximum.
                    hv_GrayValWeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_GrayValWeight = hv_GrayVal.TupleMax()
                            ;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GrayVal = hv_GrayVal.TupleRemove(
                                hv_GrayVal.TupleFind(hv_GrayValWeight));
                            hv_GrayVal.Dispose();
                            hv_GrayVal = ExpTmpLocalVar_GrayVal;
                        }
                    }
                    ho_WeightsRegion.Dispose();
                    HOperatorSet.Threshold(ho_ImageWeight, out ho_WeightsRegion, hv_GrayValWeight,
                        hv_GrayValWeight);
                    //
                    //Visualize the respective group.
                    hv_ColorIndex.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ColorIndex = (((((hv_GrayValWeight / hv_SegMaxWeight) * (hv_NumColors - 1))).TupleCeil()
                            )).TupleInt();
                    }
                    hv_ClassColor.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ClassColor = hv_WeightsColorsAlpha.TupleSelect(
                            hv_ColorIndex);
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_ClassColor);
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_WeightsRegion, HDevWindowStack.GetActive());
                    }
                }
                ho_Domain.Dispose();
                ho_WeightsRegion.Dispose();

                hv_NumColors.Dispose();
                hv_WeightsColorsAlpha.Dispose();
                hv_Rows.Dispose();
                hv_Columns.Dispose();
                hv_GrayVal.Dispose();
                hv_GrayValWeight.Dispose();
                hv_ColorIndex.Dispose();
                hv_ClassColor.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Domain.Dispose();
                ho_WeightsRegion.Dispose();

                hv_NumColors.Dispose();
                hv_WeightsColorsAlpha.Dispose();
                hv_Rows.Dispose();
                hv_Columns.Dispose();
                hv_GrayVal.Dispose();
                hv_GrayValWeight.Dispose();
                hv_ColorIndex.Dispose();
                hv_ClassColor.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Develop
        // Short Description: Open a new graphics window that preserves the aspect ratio of the given image size. 
        public void dev_open_window_fit_size(HTuple hv_Row, HTuple hv_Column, HTuple hv_Width,
            HTuple hv_Height, HTuple hv_WidthLimit, HTuple hv_HeightLimit, out HTuple hv_WindowHandle)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_MinWidth = new HTuple(), hv_MaxWidth = new HTuple();
            HTuple hv_MinHeight = new HTuple(), hv_MaxHeight = new HTuple();
            HTuple hv_ResizeFactor = new HTuple(), hv_TempWidth = new HTuple();
            HTuple hv_TempHeight = new HTuple(), hv_WindowWidth = new HTuple();
            HTuple hv_WindowHeight = new HTuple();
            // Initialize local and output iconic variables 
            hv_WindowHandle = new HTuple();
            try
            {
                //This procedure open a new graphic window
                //such that it fits into the limits specified by WidthLimit
                //and HeightLimit, but also maintains the correct aspect ratio
                //given by Width and Height.
                //
                //If it is impossible to match the minimum and maximum extent requirements
                //at the same time (f.e. if the image is very long but narrow),
                //the maximum value gets a higher priority.
                //
                //Parse input tuple WidthLimit
                if ((int)((new HTuple((new HTuple(hv_WidthLimit.TupleLength())).TupleEqual(
                    0))).TupleOr(new HTuple(hv_WidthLimit.TupleLess(0)))) != 0)
                {
                    hv_MinWidth.Dispose();
                    hv_MinWidth = 500;
                    hv_MaxWidth.Dispose();
                    hv_MaxWidth = 800;
                }
                else if ((int)(new HTuple((new HTuple(hv_WidthLimit.TupleLength())).TupleEqual(
                    1))) != 0)
                {
                    hv_MinWidth.Dispose();
                    hv_MinWidth = 0;
                    hv_MaxWidth.Dispose();
                    hv_MaxWidth = new HTuple(hv_WidthLimit);
                }
                else
                {
                    hv_MinWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MinWidth = hv_WidthLimit.TupleSelect(
                            0);
                    }
                    hv_MaxWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaxWidth = hv_WidthLimit.TupleSelect(
                            1);
                    }
                }
                //Parse input tuple HeightLimit
                if ((int)((new HTuple((new HTuple(hv_HeightLimit.TupleLength())).TupleEqual(
                    0))).TupleOr(new HTuple(hv_HeightLimit.TupleLess(0)))) != 0)
                {
                    hv_MinHeight.Dispose();
                    hv_MinHeight = 400;
                    hv_MaxHeight.Dispose();
                    hv_MaxHeight = 600;
                }
                else if ((int)(new HTuple((new HTuple(hv_HeightLimit.TupleLength())).TupleEqual(
                    1))) != 0)
                {
                    hv_MinHeight.Dispose();
                    hv_MinHeight = 0;
                    hv_MaxHeight.Dispose();
                    hv_MaxHeight = new HTuple(hv_HeightLimit);
                }
                else
                {
                    hv_MinHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MinHeight = hv_HeightLimit.TupleSelect(
                            0);
                    }
                    hv_MaxHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaxHeight = hv_HeightLimit.TupleSelect(
                            1);
                    }
                }
                //
                //Test, if window size has to be changed.
                hv_ResizeFactor.Dispose();
                hv_ResizeFactor = 1;
                //First, expand window to the minimum extents (if necessary).
                if ((int)((new HTuple(hv_MinWidth.TupleGreater(hv_Width))).TupleOr(new HTuple(hv_MinHeight.TupleGreater(
                    hv_Height)))) != 0)
                {
                    hv_ResizeFactor.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ResizeFactor = (((((hv_MinWidth.TupleReal()
                            ) / hv_Width)).TupleConcat((hv_MinHeight.TupleReal()) / hv_Height))).TupleMax()
                            ;
                    }
                }
                hv_TempWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TempWidth = hv_Width * hv_ResizeFactor;
                }
                hv_TempHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TempHeight = hv_Height * hv_ResizeFactor;
                }
                //Then, shrink window to maximum extents (if necessary).
                if ((int)((new HTuple(hv_MaxWidth.TupleLess(hv_TempWidth))).TupleOr(new HTuple(hv_MaxHeight.TupleLess(
                    hv_TempHeight)))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ResizeFactor = hv_ResizeFactor * ((((((hv_MaxWidth.TupleReal()
                                ) / hv_TempWidth)).TupleConcat((hv_MaxHeight.TupleReal()) / hv_TempHeight))).TupleMin()
                                );
                            hv_ResizeFactor.Dispose();
                            hv_ResizeFactor = ExpTmpLocalVar_ResizeFactor;
                        }
                    }
                }
                hv_WindowWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowWidth = hv_Width * hv_ResizeFactor;
                }
                hv_WindowHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowHeight = hv_Height * hv_ResizeFactor;
                }
                //Resize window
                HOperatorSet.SetWindowAttr("background_color", "black");
                HOperatorSet.OpenWindow(hv_Row, hv_Column, hv_WindowWidth, hv_WindowHeight, 0, "visible", "", out hv_WindowHandle);
                HDevWindowStack.Push(hv_WindowHandle);
                if (HDevWindowStack.IsOpen())
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetPart(HDevWindowStack.GetActive(), 0, 0, hv_Height - 1, hv_Width - 1);
                    }
                }

                hv_MinWidth.Dispose();
                hv_MaxWidth.Dispose();
                hv_MinHeight.Dispose();
                hv_MaxHeight.Dispose();
                hv_ResizeFactor.Dispose();
                hv_TempWidth.Dispose();
                hv_TempHeight.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_MinWidth.Dispose();
                hv_MaxWidth.Dispose();
                hv_MinHeight.Dispose();
                hv_MaxHeight.Dispose();
                hv_ResizeFactor.Dispose();
                hv_TempWidth.Dispose();
                hv_TempHeight.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Develop
        // Short Description: Switch dev_update_pc, dev_update_var and dev_update_window to 'off'. 
        public void dev_update_off()
        {

            // Initialize local and output iconic variables 
            //This procedure sets different update settings to 'off'.
            //This is useful to get the best performance and reduce overhead.
            //
            // dev_update_pc(...); only in hdevelop
            // dev_update_var(...); only in hdevelop
            // dev_update_window(...); only in hdevelop


            return;
        }

        // Chapter: XLD / Creation
        // Short Description: Creates an arrow shaped XLD contour. 
        public void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
            HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
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
            try
            {
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
                            HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(
                                hv_Index), hv_Column1.TupleSelect(hv_Index));
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
            catch (HalconException HDevExpDefaultException)
            {
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

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Store the given images in a tuple of dictionaries DLSamples. 
        public void gen_dl_samples_from_images(HObject ho_Images, out HTuple hv_DLSampleBatch)
        {



            // Local iconic variables 

            HObject ho_Image = null;

            // Local control variables 

            HTuple hv_NumImages = new HTuple(), hv_ImageIndex = new HTuple();
            HTuple hv_DLSample = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            hv_DLSampleBatch = new HTuple();
            try
            {
                //
                //This procedure creates DLSampleBatch, a tuple
                //containing a dictionary DLSample
                //for every image given in Images.
                //
                //Initialize output tuple.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_Images, out hv_NumImages);
                hv_DLSampleBatch.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DLSampleBatch = HTuple.TupleGenConst(
                        hv_NumImages, -1);
                }
                //
                //Loop through all given images.
                HTuple end_val10 = hv_NumImages - 1;
                HTuple step_val10 = 1;
                for (hv_ImageIndex = 0; hv_ImageIndex.Continue(end_val10, step_val10); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val10))
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Image.Dispose();
                        HOperatorSet.SelectObj(ho_Images, out ho_Image, hv_ImageIndex + 1);
                    }
                    //Create DLSample from image.
                    hv_DLSample.Dispose();
                    HOperatorSet.CreateDict(out hv_DLSample);
                    HOperatorSet.SetDictObject(ho_Image, hv_DLSample, "image");
                    //
                    //Collect the DLSamples.
                    if (hv_DLSampleBatch == null)
                        hv_DLSampleBatch = new HTuple();
                    hv_DLSampleBatch[hv_ImageIndex] = hv_DLSample;
                }
                ho_Image.Dispose();

                hv_NumImages.Dispose();
                hv_ImageIndex.Dispose();
                hv_DLSample.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();

                hv_NumImages.Dispose();
                hv_ImageIndex.Dispose();
                hv_DLSample.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Anomaly Detection
        // Short Description: Get the ground truth anomaly label and label ID. 
        public void get_anomaly_ground_truth_label(HTuple hv_SampleKeys, HTuple hv_DLSample,
            out HTuple hv_AnomalyLabelGroundTruth, out HTuple hv_AnomalyLabelIDGroundTruth)
        {


            // Initialize local and output iconic variables 
            hv_AnomalyLabelGroundTruth = new HTuple();
            hv_AnomalyLabelIDGroundTruth = new HTuple();
            //
            //This procedure returns the anomaly ground truth label.
            //
            if ((int)(new HTuple(((hv_SampleKeys.TupleFind("anomaly_label"))).TupleNotEqual(
                -1))) != 0)
            {
                hv_AnomalyLabelGroundTruth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLSample, "anomaly_label", out hv_AnomalyLabelGroundTruth);
            }
            else
            {
                throw new HalconException("Ground truth class label cannot be found in DLSample.");
            }
            if ((int)(new HTuple(((hv_SampleKeys.TupleFind("anomaly_label_id"))).TupleNotEqual(
                -1))) != 0)
            {
                hv_AnomalyLabelIDGroundTruth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLSample, "anomaly_label_id", out hv_AnomalyLabelIDGroundTruth);
            }
            else
            {
                throw new HalconException("Ground truth class label id cannot be found in DLSample.");
            }
            //


            return;
        }

        // Chapter: Deep Learning / Anomaly Detection
        // Short Description: Get the anomaly image out of DLResult. 
        public void get_anomaly_result(out HObject ho_AnomalyImage, HTuple hv_ResultKeys,
            HTuple hv_DLResult, out HTuple hv_AnomalyScore, out HTuple hv_AnomalyClassID)
        {



            // Local control variables 

            HTuple hv_AnomalyImageExists = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_AnomalyImage);
            hv_AnomalyScore = new HTuple();
            hv_AnomalyClassID = new HTuple();
            try
            {
                //
                //This procedure returns the anomaly image of DLResult.
                //
                if ((int)(new HTuple(((hv_ResultKeys.TupleFind("anomaly_image"))).TupleNotEqual(
                    -1))) != 0)
                {
                    hv_AnomalyImageExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLResult, "key_exists", "anomaly_image", out hv_AnomalyImageExists);
                    if ((int)(hv_AnomalyImageExists) != 0)
                    {
                        ho_AnomalyImage.Dispose();
                        HOperatorSet.GetDictObject(out ho_AnomalyImage, hv_DLResult, "anomaly_image");
                    }
                    else
                    {
                        throw new HalconException("Result anomaly image could not be found in DLResult.");
                    }
                }
                else
                {
                    throw new HalconException("Result anomaly image could not be found in DLResult.");
                }
                //
                //This procedure returns the anomaly score of DLResult.
                //
                if ((int)(new HTuple(((hv_ResultKeys.TupleFind("anomaly_score"))).TupleNotEqual(
                    -1))) != 0)
                {
                    hv_AnomalyScore.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "anomaly_score", out hv_AnomalyScore);
                }
                else
                {
                    throw new HalconException("Result anomaly score could not be found in DLResult.");
                }
                //
                //This procedure returns the anomaly class id of DLResult.
                //
                if ((int)(new HTuple(((hv_ResultKeys.TupleFind("anomaly_class_id"))).TupleNotEqual(
                    -1))) != 0)
                {
                    hv_AnomalyClassID.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "anomaly_class_id", out hv_AnomalyClassID);
                }
                else
                {
                    throw new HalconException("Result anomaly class ID could not be found in DLResult.");
                }

                hv_AnomalyImageExists.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_AnomalyImageExists.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Window
        // Short Description: Get the next child window that can be used for visualization. 
        public void get_child_window(HTuple hv_HeightImage, HTuple hv_Font, HTuple hv_FontSize,
            HTuple hv_Text, HTuple hv_PrevWindowCoordinates, HTuple hv_WindowHandleDict,
            HTuple hv_WindowHandleKey, out HTuple hv_WindowImageRatio, out HTuple hv_PrevWindowCoordinatesOut)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_OpenNewWindow = new HTuple(), hv_WindowHandles = new HTuple();
            HTuple hv_ParentWindowHandle = new HTuple(), hv_ChildWindowHandle = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_MetaInfo = new HTuple();
            HTuple hv_WindowRow = new HTuple(), hv_WindowColumn = new HTuple();
            HTuple hv_WindowWidth = new HTuple(), hv_WindowHeight = new HTuple();
            // Initialize local and output iconic variables 
            hv_WindowImageRatio = new HTuple();
            hv_PrevWindowCoordinatesOut = new HTuple();
            try
            {
                //
                //This procedure returns the next child window that
                //is used for visualization. If ReuseWindows is true
                //and WindowHandleList is suitable, the window handles
                //that are passed over are used. Else, this procedure
                //opens a new window, either next to the last ones, or
                //in a new row.
                //
                //First, check if the requested window is already available.
                hv_OpenNewWindow.Dispose();
                hv_OpenNewWindow = 0;
                try
                {
                    hv_WindowHandles.Dispose();
                    HOperatorSet.GetDictTuple(hv_WindowHandleDict, hv_WindowHandleKey, out hv_WindowHandles);
                    hv_ParentWindowHandle.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParentWindowHandle = hv_WindowHandles.TupleSelect(
                            0);
                    }
                    hv_ChildWindowHandle.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ChildWindowHandle = hv_WindowHandles.TupleSelect(
                            1);
                    }
                    //Check if window handle is valid.
                    try
                    {
                        HOperatorSet.FlushBuffer(hv_ChildWindowHandle);
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException2)
                    {
                        HDevExpDefaultException2.ToHTuple(out hv_Exception);
                        //Since there is something wrong with the current window, create a new one.
                        hv_OpenNewWindow.Dispose();
                        hv_OpenNewWindow = 1;
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_OpenNewWindow.Dispose();
                    hv_OpenNewWindow = 1;
                }
                //
                //Get next child window.
                if ((int)(hv_OpenNewWindow.TupleNot()) != 0)
                {
                    //
                    //If possible, reuse existing window handles.
                    HDevWindowStack.SetActive(hv_ChildWindowHandle);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
                    }
                    set_display_font(hv_ChildWindowHandle, hv_FontSize, hv_Font, "true", "false");
                    //
                    hv_MetaInfo.Dispose();
                    HOperatorSet.GetDictTuple(hv_WindowHandleDict, "meta_information", out hv_MetaInfo);
                    //
                    //Get previous window coordinates.
                    hv_WindowRow.Dispose(); hv_WindowColumn.Dispose(); hv_WindowWidth.Dispose(); hv_WindowHeight.Dispose();
                    HOperatorSet.GetWindowExtents(hv_ParentWindowHandle, out hv_WindowRow, out hv_WindowColumn,
                        out hv_WindowWidth, out hv_WindowHeight);
                    hv_WindowImageRatio.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WindowImageRatio = hv_WindowHeight / (hv_HeightImage * 1.0);
                    }
                    //
                    try
                    {
                        //
                        //Get WindowImageRatio from parent window.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_WindowImageRatio.Dispose();
                            HOperatorSet.GetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_window_image_ratio_height",
                                out hv_WindowImageRatio);
                        }
                        //
                        //Get previous window coordinates.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_PrevWindowCoordinatesOut.Dispose();
                            HOperatorSet.GetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_child_window_coordinates",
                                out hv_PrevWindowCoordinatesOut);
                        }
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        //
                        //Set WindowImageRatio from parent window.
                        hv_WindowRow.Dispose(); hv_WindowColumn.Dispose(); hv_WindowWidth.Dispose(); hv_WindowHeight.Dispose();
                        HOperatorSet.GetWindowExtents(hv_ParentWindowHandle, out hv_WindowRow,
                            out hv_WindowColumn, out hv_WindowWidth, out hv_WindowHeight);
                        hv_WindowImageRatio.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_WindowImageRatio = hv_WindowHeight / (hv_HeightImage * 1.0);
                        }
                        //
                        //Set previous window coordinates.
                        if (hv_PrevWindowCoordinatesOut == null)
                            hv_PrevWindowCoordinatesOut = new HTuple();
                        hv_PrevWindowCoordinatesOut[0] = hv_WindowRow;
                        if (hv_PrevWindowCoordinatesOut == null)
                            hv_PrevWindowCoordinatesOut = new HTuple();
                        hv_PrevWindowCoordinatesOut[1] = hv_WindowColumn;
                        if (hv_PrevWindowCoordinatesOut == null)
                            hv_PrevWindowCoordinatesOut = new HTuple();
                        hv_PrevWindowCoordinatesOut[2] = hv_WindowWidth;
                        if (hv_PrevWindowCoordinatesOut == null)
                            hv_PrevWindowCoordinatesOut = new HTuple();
                        hv_PrevWindowCoordinatesOut[3] = hv_WindowHeight;
                    }
                }
                else
                {
                    //
                    //Open a new child window.
                    hv_ChildWindowHandle.Dispose(); hv_PrevWindowCoordinatesOut.Dispose();
                    open_child_window(hv_ParentWindowHandle, hv_Font, hv_FontSize, hv_Text, hv_PrevWindowCoordinates,
                        hv_WindowHandleDict, hv_WindowHandleKey, out hv_ChildWindowHandle, out hv_PrevWindowCoordinatesOut);
                    HOperatorSet.SetWindowParam(hv_ChildWindowHandle, "flush", "false");
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetDictTuple(hv_WindowHandleDict, hv_WindowHandleKey, hv_ParentWindowHandle.TupleConcat(
                            hv_ChildWindowHandle));
                    }
                }
                //

                hv_OpenNewWindow.Dispose();
                hv_WindowHandles.Dispose();
                hv_ParentWindowHandle.Dispose();
                hv_ChildWindowHandle.Dispose();
                hv_Exception.Dispose();
                hv_MetaInfo.Dispose();
                hv_WindowRow.Dispose();
                hv_WindowColumn.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_OpenNewWindow.Dispose();
                hv_WindowHandles.Dispose();
                hv_ParentWindowHandle.Dispose();
                hv_ChildWindowHandle.Dispose();
                hv_Exception.Dispose();
                hv_MetaInfo.Dispose();
                hv_WindowRow.Dispose();
                hv_WindowColumn.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Classification
        // Short Description: Get the ground truth classification label id. 
        public void get_classification_ground_truth(HTuple hv_SampleKeys, HTuple hv_DLSample,
            out HTuple hv_ClassificationLabelIDGroundTruth)
        {


            // Initialize local and output iconic variables 
            hv_ClassificationLabelIDGroundTruth = new HTuple();
            //
            //This procedure returns the classification ground truth label ID.
            //
            if ((int)(new HTuple(((hv_SampleKeys.TupleFind("image_label_id"))).TupleNotEqual(
                -1))) != 0)
            {
                hv_ClassificationLabelIDGroundTruth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLSample, "image_label_id", out hv_ClassificationLabelIDGroundTruth);
            }
            else
            {
                throw new HalconException("Ground truth class label cannot be found in DLSample.");
            }
            //


            return;
        }

        // Chapter: Deep Learning / Classification
        // Short Description: Get the predicted classification class ID. 
        public void get_classification_result(HTuple hv_ResultKeys, HTuple hv_DLResult,
            out HTuple hv_ClassificationClassID)
        {



            // Local iconic variables 
            // Initialize local and output iconic variables 
            hv_ClassificationClassID = new HTuple();
            //
            //This procedure returns the predicted classification class ID.
            //
            if ((int)(new HTuple(((hv_ResultKeys.TupleFind("classification_class_ids"))).TupleNotEqual(
                -1))) != 0)
            {
                hv_ClassificationClassID.Dispose();
                HOperatorSet.GetDictTuple(hv_DLResult, "classification_class_ids", out hv_ClassificationClassID);
                if ((int)(new HTuple((new HTuple(hv_ClassificationClassID.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ClassificationClassID = hv_ClassificationClassID.TupleSelect(
                                0);
                            hv_ClassificationClassID.Dispose();
                            hv_ClassificationClassID = ExpTmpLocalVar_ClassificationClassID;
                        }
                    }
                }
            }
            else
            {
                throw new HalconException("Key entry 'classification_class_ids' could not be found in DLResult.");
            }
            //


            return;
        }

        // Chapter: Deep Learning / Semantic Segmentation
        // Short Description: Get the confidences of the segmentation result. 
        public void get_confidence_image(out HObject ho_ImageConfidence, HTuple hv_ResultKeys,
            HTuple hv_DLResult)
        {


            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageConfidence);
            //
            //This procedure returns confidences of the segmentation result.
            //
            if ((int)(new HTuple(((hv_ResultKeys.TupleFind("segmentation_confidence"))).TupleNotEqual(
                -1))) != 0)
            {
                ho_ImageConfidence.Dispose();
                HOperatorSet.GetDictObject(out ho_ImageConfidence, hv_DLResult, "segmentation_confidence");
            }
            else if ((int)(new HTuple(((hv_ResultKeys.TupleFind("segmentation_confidences"))).TupleNotEqual(
                -1))) != 0)
            {
                ho_ImageConfidence.Dispose();
                HOperatorSet.GetDictObject(out ho_ImageConfidence, hv_DLResult, "segmentation_confidences");
            }
            else
            {
                throw new HalconException("Confidence image could not be found in DLSample.");
            }


            return;
        }

        // Chapter: Deep Learning / Model
        // Short Description: Generate NumColors distinct colors 
        public void get_distinct_colors(HTuple hv_NumColors, HTuple hv_Random, HTuple hv_StartColor,
            HTuple hv_EndColor, out HTuple hv_Colors)
        {



            // Local iconic variables 

            HObject ho_HLSImageH, ho_HLSImageL, ho_HLSImageS;
            HObject ho_ImageR, ho_ImageG, ho_ImageB;

            // Local control variables 

            HTuple hv_IsString = new HTuple(), hv_Hue = new HTuple();
            HTuple hv_Lightness = new HTuple(), hv_Saturation = new HTuple();
            HTuple hv_Rows = new HTuple(), hv_Columns = new HTuple();
            HTuple hv_Red = new HTuple(), hv_Green = new HTuple();
            HTuple hv_Blue = new HTuple();
            HTuple hv_EndColor_COPY_INP_TMP = new HTuple(hv_EndColor);
            HTuple hv_Random_COPY_INP_TMP = new HTuple(hv_Random);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_HLSImageH);
            HOperatorSet.GenEmptyObj(out ho_HLSImageL);
            HOperatorSet.GenEmptyObj(out ho_HLSImageS);
            HOperatorSet.GenEmptyObj(out ho_ImageR);
            HOperatorSet.GenEmptyObj(out ho_ImageG);
            HOperatorSet.GenEmptyObj(out ho_ImageB);
            hv_Colors = new HTuple();
            try
            {
                //
                //We get distinct color-values first in HLS color-space.
                //Assumes hue [0, EndColor), lightness [0, 1), saturation [0, 1).
                //
                //Parameter checks.
                //NumColors.
                if ((int)(new HTuple(hv_NumColors.TupleLess(1))) != 0)
                {
                    throw new HalconException("NumColors should be at least 1");
                }
                if ((int)(((hv_NumColors.TupleIsInt())).TupleNot()) != 0)
                {
                    throw new HalconException("NumColors should be of type int");
                }
                if ((int)(new HTuple((new HTuple(hv_NumColors.TupleLength())).TupleNotEqual(
                    1))) != 0)
                {
                    throw new HalconException("NumColors should have length 1");
                }
                //Random.
                if ((int)((new HTuple(hv_Random_COPY_INP_TMP.TupleNotEqual(0))).TupleAnd(new HTuple(hv_Random_COPY_INP_TMP.TupleNotEqual(
                    1)))) != 0)
                {
                    hv_IsString.Dispose();
                    HOperatorSet.TupleIsString(hv_Random_COPY_INP_TMP, out hv_IsString);
                    if ((int)(hv_IsString) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Random = (new HTuple(hv_Random_COPY_INP_TMP.TupleEqual(
                                    "true"))).TupleOr("false");
                                hv_Random_COPY_INP_TMP.Dispose();
                                hv_Random_COPY_INP_TMP = ExpTmpLocalVar_Random;
                            }
                        }
                    }
                    else
                    {
                        throw new HalconException("Random should be either true or false");
                    }
                }
                //StartColor.
                if ((int)(new HTuple((new HTuple(hv_StartColor.TupleLength())).TupleNotEqual(
                    1))) != 0)
                {
                    throw new HalconException("StartColor should have length 1");
                }
                if ((int)((new HTuple(hv_StartColor.TupleLess(0))).TupleOr(new HTuple(hv_StartColor.TupleGreater(
                    255)))) != 0)
                {
                    throw new HalconException(new HTuple("StartColor should be in the range [0, 255]"));
                }
                if ((int)(((hv_StartColor.TupleIsInt())).TupleNot()) != 0)
                {
                    throw new HalconException("StartColor should be of type int");
                }
                //EndColor.
                if ((int)(new HTuple((new HTuple(hv_EndColor_COPY_INP_TMP.TupleLength())).TupleNotEqual(
                    1))) != 0)
                {
                    throw new HalconException("EndColor should have length 1");
                }
                if ((int)((new HTuple(hv_EndColor_COPY_INP_TMP.TupleLess(0))).TupleOr(new HTuple(hv_EndColor_COPY_INP_TMP.TupleGreater(
                    255)))) != 0)
                {
                    throw new HalconException(new HTuple("EndColor should be in the range [0, 255]"));
                }
                if ((int)(((hv_EndColor_COPY_INP_TMP.TupleIsInt())).TupleNot()) != 0)
                {
                    throw new HalconException("EndColor should be of type int");
                }
                //
                //Color generation.
                if ((int)(new HTuple(hv_StartColor.TupleGreater(hv_EndColor_COPY_INP_TMP))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_EndColor = hv_EndColor_COPY_INP_TMP + 255;
                            hv_EndColor_COPY_INP_TMP.Dispose();
                            hv_EndColor_COPY_INP_TMP = ExpTmpLocalVar_EndColor;
                        }
                    }
                }
                if ((int)(new HTuple(hv_NumColors.TupleNotEqual(1))) != 0)
                {
                    hv_Hue.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Hue = (hv_StartColor + (((((hv_EndColor_COPY_INP_TMP - hv_StartColor) * ((HTuple.TupleGenSequence(
                            0, hv_NumColors - 1, 1)).TupleReal())) / (((hv_NumColors - 1)).TupleReal()))).TupleInt()
                            )) % 255;
                    }
                }
                else
                {
                    hv_Hue.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Hue = ((hv_StartColor.TupleConcat(
                            hv_EndColor_COPY_INP_TMP))).TupleMean();
                    }
                }
                if ((int)(hv_Random_COPY_INP_TMP) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Hue = hv_Hue.TupleSelect(
                                (HTuple.TupleRand(hv_NumColors)).TupleSortIndex());
                            hv_Hue.Dispose();
                            hv_Hue = ExpTmpLocalVar_Hue;
                        }
                    }
                    hv_Lightness.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Lightness = ((((5.0 + HTuple.TupleRand(
                            hv_NumColors)) * 255.0) / 10.0)).TupleInt();
                    }
                    hv_Saturation.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Saturation = ((((9.0 + HTuple.TupleRand(
                            hv_NumColors)) * 255.0) / 10.0)).TupleInt();
                    }
                }
                else
                {
                    hv_Lightness.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Lightness = ((HTuple.TupleGenConst(
                            hv_NumColors, 0.55) * 255.0)).TupleInt();
                    }
                    hv_Saturation.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Saturation = ((HTuple.TupleGenConst(
                            hv_NumColors, 0.95) * 255.0)).TupleInt();
                    }
                }
                //
                //Write colors to a 3-channel image in order to transform easier.
                ho_HLSImageH.Dispose();
                HOperatorSet.GenImageConst(out ho_HLSImageH, "byte", 1, hv_NumColors);
                ho_HLSImageL.Dispose();
                HOperatorSet.GenImageConst(out ho_HLSImageL, "byte", 1, hv_NumColors);
                ho_HLSImageS.Dispose();
                HOperatorSet.GenImageConst(out ho_HLSImageS, "byte", 1, hv_NumColors);
                hv_Rows.Dispose(); hv_Columns.Dispose();
                HOperatorSet.GetRegionPoints(ho_HLSImageH, out hv_Rows, out hv_Columns);
                HOperatorSet.SetGrayval(ho_HLSImageH, hv_Rows, hv_Columns, hv_Hue);
                HOperatorSet.SetGrayval(ho_HLSImageL, hv_Rows, hv_Columns, hv_Lightness);
                HOperatorSet.SetGrayval(ho_HLSImageS, hv_Rows, hv_Columns, hv_Saturation);
                //
                //Convert from HLS to RGB.
                ho_ImageR.Dispose(); ho_ImageG.Dispose(); ho_ImageB.Dispose();
                HOperatorSet.TransToRgb(ho_HLSImageH, ho_HLSImageL, ho_HLSImageS, out ho_ImageR,
                    out ho_ImageG, out ho_ImageB, "hls");
                //
                //Get RGB-values and transform to Hex.
                hv_Red.Dispose();
                HOperatorSet.GetGrayval(ho_ImageR, hv_Rows, hv_Columns, out hv_Red);
                hv_Green.Dispose();
                HOperatorSet.GetGrayval(ho_ImageG, hv_Rows, hv_Columns, out hv_Green);
                hv_Blue.Dispose();
                HOperatorSet.GetGrayval(ho_ImageB, hv_Rows, hv_Columns, out hv_Blue);
                hv_Colors.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Colors = (("#" + (hv_Red.TupleString(
                        "02x"))) + (hv_Green.TupleString("02x"))) + (hv_Blue.TupleString("02x"));
                }
                ho_HLSImageH.Dispose();
                ho_HLSImageL.Dispose();
                ho_HLSImageS.Dispose();
                ho_ImageR.Dispose();
                ho_ImageG.Dispose();
                ho_ImageB.Dispose();

                hv_EndColor_COPY_INP_TMP.Dispose();
                hv_Random_COPY_INP_TMP.Dispose();
                hv_IsString.Dispose();
                hv_Hue.Dispose();
                hv_Lightness.Dispose();
                hv_Saturation.Dispose();
                hv_Rows.Dispose();
                hv_Columns.Dispose();
                hv_Red.Dispose();
                hv_Green.Dispose();
                hv_Blue.Dispose();

                return;
                //
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_HLSImageH.Dispose();
                ho_HLSImageL.Dispose();
                ho_HLSImageS.Dispose();
                ho_ImageR.Dispose();
                ho_ImageG.Dispose();
                ho_ImageB.Dispose();

                hv_EndColor_COPY_INP_TMP.Dispose();
                hv_Random_COPY_INP_TMP.Dispose();
                hv_IsString.Dispose();
                hv_Hue.Dispose();
                hv_Lightness.Dispose();
                hv_Saturation.Dispose();
                hv_Rows.Dispose();
                hv_Columns.Dispose();
                hv_Red.Dispose();
                hv_Green.Dispose();
                hv_Blue.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Generates certain colors for different ClassNames 
        public void get_dl_class_colors(HTuple hv_ClassNames, out HTuple hv_Colors)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_NumColors = new HTuple(), hv_ColorsRainbow = new HTuple();
            HTuple hv_ClassNamesGood = new HTuple(), hv_IndexFind = new HTuple();
            HTuple hv_GoodIdx = new HTuple(), hv_CurrentColor = new HTuple();
            HTuple hv_GreenIdx = new HTuple();
            // Initialize local and output iconic variables 
            hv_Colors = new HTuple();
            try
            {
                //
                //This procedure returns for each class a certain color.
                //
                //Define distinct colors for the classes.
                hv_NumColors.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_NumColors = new HTuple(hv_ClassNames.TupleLength()
                        );
                }
                //Get distinct colors without randomness makes neighboring colors look very similar.
                //We use a workaround to get deterministic colors where subsequent colors are distinguishable.
                hv_ColorsRainbow.Dispose();
                get_distinct_colors(hv_NumColors, 0, 0, 200, out hv_ColorsRainbow);
                {
                    HTuple ExpTmpOutVar_0;
                    HOperatorSet.TupleInverse(hv_ColorsRainbow, out ExpTmpOutVar_0);
                    hv_ColorsRainbow.Dispose();
                    hv_ColorsRainbow = ExpTmpOutVar_0;
                }
                hv_Colors.Dispose();
                make_neighboring_colors_distinguishable(hv_ColorsRainbow, out hv_Colors);
                //If a class 'OK','ok', 'good' or 'GOOD' is present set this class to green.
                //Only the first occurrence found is set to a green shade.
                hv_ClassNamesGood.Dispose();
                hv_ClassNamesGood = new HTuple();
                hv_ClassNamesGood[0] = "good";
                hv_ClassNamesGood[1] = "GOOD";
                hv_ClassNamesGood[2] = "ok";
                hv_ClassNamesGood[3] = "OK";
                for (hv_IndexFind = 0; (int)hv_IndexFind <= (int)((new HTuple(hv_ClassNamesGood.TupleLength()
                    )) - 1); hv_IndexFind = (int)hv_IndexFind + 1)
                {
                    hv_GoodIdx.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_GoodIdx = hv_ClassNames.TupleFindFirst(
                            hv_ClassNamesGood.TupleSelect(hv_IndexFind));
                    }
                    if ((int)((new HTuple(hv_GoodIdx.TupleNotEqual(-1))).TupleAnd(new HTuple((new HTuple(hv_ClassNames.TupleLength()
                        )).TupleLessEqual(8)))) != 0)
                    {
                        //If number of classes is <= 8, swap color with a green color.
                        hv_CurrentColor.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CurrentColor = hv_Colors.TupleSelect(
                                hv_GoodIdx);
                        }
                        hv_GreenIdx.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_GreenIdx = (new HTuple((new HTuple(hv_ClassNames.TupleLength()
                                )) / 2.0)).TupleFloor();
                        }
                        //Set to pure green.
                        if (hv_Colors == null)
                            hv_Colors = new HTuple();
                        hv_Colors[hv_GoodIdx] = "#00ff00";
                        //Write original color to a green entry.
                        if (hv_Colors == null)
                            hv_Colors = new HTuple();
                        hv_Colors[hv_GreenIdx] = hv_CurrentColor;
                        break;
                    }
                    else if ((int)((new HTuple(hv_GoodIdx.TupleNotEqual(-1))).TupleAnd(
                        new HTuple((new HTuple(hv_ClassNames.TupleLength())).TupleGreater(8)))) != 0)
                    {
                        //If number of classes is larger than 8, set the respective color to green.
                        if (hv_Colors == null)
                            hv_Colors = new HTuple();
                        hv_Colors[hv_GoodIdx] = "#00ff00";
                        break;
                    }
                }

                hv_NumColors.Dispose();
                hv_ColorsRainbow.Dispose();
                hv_ClassNamesGood.Dispose();
                hv_IndexFind.Dispose();
                hv_GoodIdx.Dispose();
                hv_CurrentColor.Dispose();
                hv_GreenIdx.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_NumColors.Dispose();
                hv_ColorsRainbow.Dispose();
                hv_ClassNamesGood.Dispose();
                hv_IndexFind.Dispose();
                hv_GoodIdx.Dispose();
                hv_CurrentColor.Dispose();
                hv_GreenIdx.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Get the image of a sample. 
        public void get_image(out HObject ho_Image, HTuple hv_SampleKeys, HTuple hv_DLSample)
        {


            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            //
            //This procedure returns the image of a sample.
            //
            if ((int)(new HTuple(((hv_SampleKeys.TupleFind("image"))).TupleNotEqual(-1))) != 0)
            {
                ho_Image.Dispose();
                HOperatorSet.GetDictObject(out ho_Image, hv_DLSample, "image");
            }
            else
            {
                throw new HalconException("Image could not be found in DLSample.");
            }


            return;
        }

        // Chapter: Graphics / Window
        // Short Description: Get the next window that can be used for visualization. 
        public void get_next_window(HTuple hv_Font, HTuple hv_FontSize, HTuple hv_ShowBottomDesc,
            HTuple hv_WidthImage, HTuple hv_HeightImage, HTuple hv_MapColorBarWidth, HTuple hv_ScaleWindows,
            HTuple hv_ThresholdWidth, HTuple hv_PrevWindowCoordinates, HTuple hv_WindowHandleDict,
            HTuple hv_WindowHandleKey, out HTuple hv_CurrentWindowHandle, out HTuple hv_WindowImageRatioHeight,
            out HTuple hv_PrevWindowCoordinatesOut)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_OpenNewWindow = new HTuple(), hv_WindowHandles = new HTuple();
            HTuple hv_Value = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv__ = new HTuple(), hv_MarginBottom = new HTuple();
            HTuple hv_WindowImageRatioWidth = new HTuple(), hv_SetPartRow2 = new HTuple();
            HTuple hv_SetPartColumn2 = new HTuple(), hv_MetaInfo = new HTuple();
            // Initialize local and output iconic variables 
            hv_CurrentWindowHandle = new HTuple();
            hv_WindowImageRatioHeight = new HTuple();
            hv_PrevWindowCoordinatesOut = new HTuple();
            try
            {
                //
                //This procedure returns the next window that
                //is used for visualization. If ReuseWindows is true
                //and WindowHandleList is suitable, the window handles
                //that are passed over are used. Else, this procedure
                //opens a new window, either next to the last ones, or
                //in a new row.
                //
                //First, check if the requested window is already available.
                hv_OpenNewWindow.Dispose();
                hv_OpenNewWindow = 0;
                try
                {
                    hv_WindowHandles.Dispose();
                    HOperatorSet.GetDictTuple(hv_WindowHandleDict, hv_WindowHandleKey, out hv_WindowHandles);
                    hv_CurrentWindowHandle.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CurrentWindowHandle = hv_WindowHandles.TupleSelect(
                            0);
                    }
                    //Check if window handle is valid.
                    try
                    {
                        hv_Value.Dispose();
                        HOperatorSet.GetWindowParam(hv_CurrentWindowHandle, "flush", out hv_Value);
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException2)
                    {
                        HDevExpDefaultException2.ToHTuple(out hv_Exception);
                        //If there is something wrong with the current window, create a new one.
                        hv_OpenNewWindow.Dispose();
                        hv_OpenNewWindow = 1;
                        HOperatorSet.RemoveDictKey(hv_WindowHandleDict, hv_WindowHandleKey);
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_OpenNewWindow.Dispose();
                    hv_OpenNewWindow = 1;
                }
                //
                //Get next window.
                if ((int)(hv_OpenNewWindow.TupleNot()) != 0)
                {
                    //
                    //If possible, reuse existing window handles.
                    HDevWindowStack.SetActive(hv_CurrentWindowHandle);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
                    }
                    set_display_font(hv_CurrentWindowHandle, hv_FontSize, hv_Font, "true", "false");
                    //
                    //Calculate MarginBottom.
                    if ((int)(hv_ShowBottomDesc) != 0)
                    {
                        hv_Ascent.Dispose(); hv_Descent.Dispose(); hv__.Dispose(); hv__.Dispose();
                        HOperatorSet.GetStringExtents(hv_CurrentWindowHandle, "test_string", out hv_Ascent,
                            out hv_Descent, out hv__, out hv__);
                        hv_MarginBottom.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_MarginBottom = ((2 * 12) + hv_Ascent) + hv_Descent;
                        }
                    }
                    else
                    {
                        hv_MarginBottom.Dispose();
                        hv_MarginBottom = 0;
                    }
                    //
                    //Get and set meta information for current window.
                    hv_WindowImageRatioHeight.Dispose(); hv_WindowImageRatioWidth.Dispose(); hv_SetPartRow2.Dispose(); hv_SetPartColumn2.Dispose(); hv_PrevWindowCoordinatesOut.Dispose();
                    update_window_meta_information(hv_CurrentWindowHandle, hv_WidthImage, hv_HeightImage,
                        0, 0, hv_MapColorBarWidth, hv_MarginBottom, out hv_WindowImageRatioHeight,
                        out hv_WindowImageRatioWidth, out hv_SetPartRow2, out hv_SetPartColumn2,
                        out hv_PrevWindowCoordinatesOut);
                    //
                    //Update meta information.
                    hv_MetaInfo.Dispose();
                    HOperatorSet.GetDictTuple(hv_WindowHandleDict, "meta_information", out hv_MetaInfo);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_window_image_ratio_height",
                            hv_WindowImageRatioHeight);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_window_image_ratio_width",
                            hv_WindowImageRatioWidth);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_set_part_row2",
                            hv_SetPartRow2);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_set_part_column2",
                            hv_SetPartColumn2);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_margin_bottom",
                            hv_MarginBottom);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_map_color_bar_with",
                            hv_MapColorBarWidth);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_window_coordinates",
                            hv_PrevWindowCoordinatesOut);
                    }
                }
                else
                {
                    //
                    //Open a new window.
                    hv_CurrentWindowHandle.Dispose(); hv_WindowImageRatioHeight.Dispose(); hv_PrevWindowCoordinatesOut.Dispose();
                    open_next_window(hv_Font, hv_FontSize, hv_ShowBottomDesc, hv_WidthImage,
                        hv_HeightImage, hv_MapColorBarWidth, hv_ScaleWindows, hv_ThresholdWidth,
                        hv_PrevWindowCoordinates, hv_WindowHandleDict, hv_WindowHandleKey, out hv_CurrentWindowHandle,
                        out hv_WindowImageRatioHeight, out hv_PrevWindowCoordinatesOut);
                    HOperatorSet.SetWindowParam(hv_CurrentWindowHandle, "flush", "false");
                }
                //

                hv_OpenNewWindow.Dispose();
                hv_WindowHandles.Dispose();
                hv_Value.Dispose();
                hv_Exception.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv__.Dispose();
                hv_MarginBottom.Dispose();
                hv_WindowImageRatioWidth.Dispose();
                hv_SetPartRow2.Dispose();
                hv_SetPartColumn2.Dispose();
                hv_MetaInfo.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_OpenNewWindow.Dispose();
                hv_WindowHandles.Dispose();
                hv_Value.Dispose();
                hv_Exception.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv__.Dispose();
                hv_MarginBottom.Dispose();
                hv_WindowImageRatioWidth.Dispose();
                hv_SetPartRow2.Dispose();
                hv_SetPartColumn2.Dispose();
                hv_MetaInfo.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Semantic Segmentation
        // Short Description: Get the ground truth segmentation image. 
        public void get_segmentation_image_ground_truth(out HObject ho_SegmentationImagGroundTruth,
            HTuple hv_SampleKeys, HTuple hv_DLSample)
        {


            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SegmentationImagGroundTruth);
            //
            //This procedure returns the ground truth segmentation image.
            //
            if ((int)(new HTuple(((hv_SampleKeys.TupleFind("segmentation_image"))).TupleNotEqual(
                -1))) != 0)
            {
                ho_SegmentationImagGroundTruth.Dispose();
                HOperatorSet.GetDictObject(out ho_SegmentationImagGroundTruth, hv_DLSample,
                    "segmentation_image");
            }
            else
            {
                throw new HalconException("Ground truth segmentation image could not be found in DLSample.");
            }


            return;
        }

        // Chapter: Deep Learning / Semantic Segmentation
        // Short Description: Get the predicted segmentation result image. 
        public void get_segmentation_image_result(out HObject ho_SegmentationImageResult,
            HTuple hv_ResultKeys, HTuple hv_DLResult)
        {


            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SegmentationImageResult);
            //
            //This procedure returns the predicted segmentation result image.
            //
            if ((int)(new HTuple(((hv_ResultKeys.TupleFind("segmentation_image"))).TupleNotEqual(
                -1))) != 0)
            {
                ho_SegmentationImageResult.Dispose();
                HOperatorSet.GetDictObject(out ho_SegmentationImageResult, hv_DLResult, "segmentation_image");
            }
            else
            {
                throw new HalconException("Result segmentation data could not be found in DLSample.");
            }


            return;
        }

        // Chapter: Deep Learning / Semantic Segmentation
        // Short Description: Get the weight image of a sample. 
        public void get_weight_image(out HObject ho_ImageWeight, HTuple hv_SampleKeys,
            HTuple hv_DLSample)
        {


            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageWeight);
            //
            //This procedure returns the segmentation weight image of a sample.
            //
            if ((int)(new HTuple(((hv_SampleKeys.TupleFind("weight_image"))).TupleNotEqual(
                -1))) != 0)
            {
                ho_ImageWeight.Dispose();
                HOperatorSet.GetDictObject(out ho_ImageWeight, hv_DLSample, "weight_image");
            }
            else
            {
                throw new HalconException("Weight image could not be found in DLSample.");
            }


            return;
        }

        // Chapter: File / Misc
        // Short Description: Get all image files under the given path 
        public void list_image_files(HTuple hv_ImageDirectory, HTuple hv_Extensions, HTuple hv_Options,
            out HTuple hv_ImageFiles)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ImageDirectoryIndex = new HTuple();
            HTuple hv_ImageFilesTmp = new HTuple(), hv_CurrentImageDirectory = new HTuple();
            HTuple hv_HalconImages = new HTuple(), hv_OS = new HTuple();
            HTuple hv_Directories = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Length = new HTuple(), hv_NetworkDrive = new HTuple();
            HTuple hv_Substring = new HTuple(), hv_FileExists = new HTuple();
            HTuple hv_AllFiles = new HTuple(), hv_i = new HTuple();
            HTuple hv_Selection = new HTuple();
            HTuple hv_Extensions_COPY_INP_TMP = new HTuple(hv_Extensions);

            // Initialize local and output iconic variables 
            hv_ImageFiles = new HTuple();
            try
            {
                //This procedure returns all files in a given directory
                //with one of the suffixes specified in Extensions.
                //
                //Input parameters:
                //ImageDirectory: Directory or a tuple of directories with images.
                //   If a directory is not found locally, the respective directory
                //   is searched under %HALCONIMAGES%/ImageDirectory.
                //   See the Installation Guide for further information
                //   in case %HALCONIMAGES% is not set.
                //Extensions: A string tuple containing the extensions to be found
                //   e.g. ['png','tif',jpg'] or others
                //If Extensions is set to 'default' or the empty string '',
                //   all image suffixes supported by HALCON are used.
                //Options: as in the operator list_files, except that the 'files'
                //   option is always used. Note that the 'directories' option
                //   has no effect but increases runtime, because only files are
                //   returned.
                //
                //Output parameter:
                //ImageFiles: A tuple of all found image file names
                //
                if ((int)((new HTuple((new HTuple(hv_Extensions_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Extensions_COPY_INP_TMP.TupleEqual(""))))).TupleOr(new HTuple(hv_Extensions_COPY_INP_TMP.TupleEqual(
                    "default")))) != 0)
                {
                    hv_Extensions_COPY_INP_TMP.Dispose();
                    hv_Extensions_COPY_INP_TMP = new HTuple();
                    hv_Extensions_COPY_INP_TMP[0] = "ima";
                    hv_Extensions_COPY_INP_TMP[1] = "tif";
                    hv_Extensions_COPY_INP_TMP[2] = "tiff";
                    hv_Extensions_COPY_INP_TMP[3] = "gif";
                    hv_Extensions_COPY_INP_TMP[4] = "bmp";
                    hv_Extensions_COPY_INP_TMP[5] = "jpg";
                    hv_Extensions_COPY_INP_TMP[6] = "jpeg";
                    hv_Extensions_COPY_INP_TMP[7] = "jp2";
                    hv_Extensions_COPY_INP_TMP[8] = "jxr";
                    hv_Extensions_COPY_INP_TMP[9] = "png";
                    hv_Extensions_COPY_INP_TMP[10] = "pcx";
                    hv_Extensions_COPY_INP_TMP[11] = "ras";
                    hv_Extensions_COPY_INP_TMP[12] = "xwd";
                    hv_Extensions_COPY_INP_TMP[13] = "pbm";
                    hv_Extensions_COPY_INP_TMP[14] = "pnm";
                    hv_Extensions_COPY_INP_TMP[15] = "pgm";
                    hv_Extensions_COPY_INP_TMP[16] = "ppm";
                    //
                }
                hv_ImageFiles.Dispose();
                hv_ImageFiles = new HTuple();
                //Loop through all given image directories.
                for (hv_ImageDirectoryIndex = 0; (int)hv_ImageDirectoryIndex <= (int)((new HTuple(hv_ImageDirectory.TupleLength()
                    )) - 1); hv_ImageDirectoryIndex = (int)hv_ImageDirectoryIndex + 1)
                {
                    hv_ImageFilesTmp.Dispose();
                    hv_ImageFilesTmp = new HTuple();
                    hv_CurrentImageDirectory.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CurrentImageDirectory = hv_ImageDirectory.TupleSelect(
                            hv_ImageDirectoryIndex);
                    }
                    if ((int)(new HTuple(hv_CurrentImageDirectory.TupleEqual(""))) != 0)
                    {
                        hv_CurrentImageDirectory.Dispose();
                        hv_CurrentImageDirectory = ".";
                    }
                    hv_HalconImages.Dispose();
                    HOperatorSet.GetSystem("image_dir", out hv_HalconImages);
                    hv_OS.Dispose();
                    HOperatorSet.GetSystem("operating_system", out hv_OS);
                    if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_HalconImages = hv_HalconImages.TupleSplit(
                                    ";");
                                hv_HalconImages.Dispose();
                                hv_HalconImages = ExpTmpLocalVar_HalconImages;
                            }
                        }
                    }
                    else
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_HalconImages = hv_HalconImages.TupleSplit(
                                    ":");
                                hv_HalconImages.Dispose();
                                hv_HalconImages = ExpTmpLocalVar_HalconImages;
                            }
                        }
                    }
                    hv_Directories.Dispose();
                    hv_Directories = new HTuple(hv_CurrentImageDirectory);
                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_HalconImages.TupleLength()
                        )) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Directories = hv_Directories.TupleConcat(
                                    ((hv_HalconImages.TupleSelect(hv_Index)) + "/") + hv_CurrentImageDirectory);
                                hv_Directories.Dispose();
                                hv_Directories = ExpTmpLocalVar_Directories;
                            }
                        }
                    }
                    hv_Length.Dispose();
                    HOperatorSet.TupleStrlen(hv_Directories, out hv_Length);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NetworkDrive.Dispose();
                        HOperatorSet.TupleGenConst(new HTuple(hv_Length.TupleLength()), 0, out hv_NetworkDrive);
                    }
                    if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                    {
                        for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength()
                            )) - 1); hv_Index = (int)hv_Index + 1)
                        {
                            if ((int)(new HTuple(((((hv_Directories.TupleSelect(hv_Index))).TupleStrlen()
                                )).TupleGreater(1))) != 0)
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Substring.Dispose();
                                    HOperatorSet.TupleStrFirstN(hv_Directories.TupleSelect(hv_Index), 1,
                                        out hv_Substring);
                                }
                                if ((int)((new HTuple(hv_Substring.TupleEqual("//"))).TupleOr(new HTuple(hv_Substring.TupleEqual(
                                    "\\\\")))) != 0)
                                {
                                    if (hv_NetworkDrive == null)
                                        hv_NetworkDrive = new HTuple();
                                    hv_NetworkDrive[hv_Index] = 1;
                                }
                            }
                        }
                    }
                    hv_ImageFilesTmp.Dispose();
                    hv_ImageFilesTmp = new HTuple();
                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Directories.TupleLength()
                        )) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_FileExists.Dispose();
                            HOperatorSet.FileExists(hv_Directories.TupleSelect(hv_Index), out hv_FileExists);
                        }
                        if ((int)(hv_FileExists) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_AllFiles.Dispose();
                                HOperatorSet.ListFiles(hv_Directories.TupleSelect(hv_Index), (new HTuple("files")).TupleConcat(
                                    hv_Options), out hv_AllFiles);
                            }
                            hv_ImageFilesTmp.Dispose();
                            hv_ImageFilesTmp = new HTuple();
                            for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Extensions_COPY_INP_TMP.TupleLength()
                                )) - 1); hv_i = (int)hv_i + 1)
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Selection.Dispose();
                                    HOperatorSet.TupleRegexpSelect(hv_AllFiles, (((".*" + (hv_Extensions_COPY_INP_TMP.TupleSelect(
                                        hv_i))) + "$")).TupleConcat("ignore_case"), out hv_Selection);
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_ImageFilesTmp = hv_ImageFilesTmp.TupleConcat(
                                            hv_Selection);
                                        hv_ImageFilesTmp.Dispose();
                                        hv_ImageFilesTmp = ExpTmpLocalVar_ImageFilesTmp;
                                    }
                                }
                            }
                            {
                                HTuple ExpTmpOutVar_0;
                                HOperatorSet.TupleRegexpReplace(hv_ImageFilesTmp, (new HTuple("\\\\")).TupleConcat(
                                    "replace_all"), "/", out ExpTmpOutVar_0);
                                hv_ImageFilesTmp.Dispose();
                                hv_ImageFilesTmp = ExpTmpOutVar_0;
                            }
                            if ((int)(hv_NetworkDrive.TupleSelect(hv_Index)) != 0)
                            {
                                {
                                    HTuple ExpTmpOutVar_0;
                                    HOperatorSet.TupleRegexpReplace(hv_ImageFilesTmp, (new HTuple("//")).TupleConcat(
                                        "replace_all"), "/", out ExpTmpOutVar_0);
                                    hv_ImageFilesTmp.Dispose();
                                    hv_ImageFilesTmp = ExpTmpOutVar_0;
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_ImageFilesTmp = "/" + hv_ImageFilesTmp;
                                        hv_ImageFilesTmp.Dispose();
                                        hv_ImageFilesTmp = ExpTmpLocalVar_ImageFilesTmp;
                                    }
                                }
                            }
                            else
                            {
                                {
                                    HTuple ExpTmpOutVar_0;
                                    HOperatorSet.TupleRegexpReplace(hv_ImageFilesTmp, (new HTuple("//")).TupleConcat(
                                        "replace_all"), "/", out ExpTmpOutVar_0);
                                    hv_ImageFilesTmp.Dispose();
                                    hv_ImageFilesTmp = ExpTmpOutVar_0;
                                }
                            }
                            break;
                        }
                    }
                    //Concatenate the output image paths.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ImageFiles = hv_ImageFiles.TupleConcat(
                                hv_ImageFilesTmp);
                            hv_ImageFiles.Dispose();
                            hv_ImageFiles = ExpTmpLocalVar_ImageFiles;
                        }
                    }
                }

                hv_Extensions_COPY_INP_TMP.Dispose();
                hv_ImageDirectoryIndex.Dispose();
                hv_ImageFilesTmp.Dispose();
                hv_CurrentImageDirectory.Dispose();
                hv_HalconImages.Dispose();
                hv_OS.Dispose();
                hv_Directories.Dispose();
                hv_Index.Dispose();
                hv_Length.Dispose();
                hv_NetworkDrive.Dispose();
                hv_Substring.Dispose();
                hv_FileExists.Dispose();
                hv_AllFiles.Dispose();
                hv_i.Dispose();
                hv_Selection.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Extensions_COPY_INP_TMP.Dispose();
                hv_ImageDirectoryIndex.Dispose();
                hv_ImageFilesTmp.Dispose();
                hv_CurrentImageDirectory.Dispose();
                hv_HalconImages.Dispose();
                hv_OS.Dispose();
                hv_Directories.Dispose();
                hv_Index.Dispose();
                hv_Length.Dispose();
                hv_NetworkDrive.Dispose();
                hv_Substring.Dispose();
                hv_FileExists.Dispose();
                hv_AllFiles.Dispose();
                hv_i.Dispose();
                hv_Selection.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: shuffles the input colors in a deterministic way 
        public void make_neighboring_colors_distinguishable(HTuple hv_ColorsRainbow, out HTuple hv_Colors)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_NumColors = new HTuple(), hv_NumChunks = new HTuple();
            HTuple hv_NumLeftOver = new HTuple(), hv_ColorsPerChunk = new HTuple();
            HTuple hv_StartIdx = new HTuple(), hv_S = new HTuple();
            HTuple hv_EndIdx = new HTuple(), hv_IdxsLeft = new HTuple();
            HTuple hv_IdxsRight = new HTuple();
            // Initialize local and output iconic variables 
            hv_Colors = new HTuple();
            try
            {
                //
                //Shuffle the input colors in a deterministic way
                //to make adjacent colors more distinguishable.
                //Neighboring colors from the input are distributed to every NumChunks
                //position in the output.
                //Depending on the number of colors, increase NumChunks.
                hv_NumColors.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_NumColors = new HTuple(hv_ColorsRainbow.TupleLength()
                        );
                }
                if ((int)(new HTuple(hv_NumColors.TupleGreaterEqual(8))) != 0)
                {
                    hv_NumChunks.Dispose();
                    hv_NumChunks = 3;
                    if ((int)(new HTuple(hv_NumColors.TupleGreaterEqual(40))) != 0)
                    {
                        hv_NumChunks.Dispose();
                        hv_NumChunks = 6;
                    }
                    else if ((int)(new HTuple(hv_NumColors.TupleGreaterEqual(20))) != 0)
                    {
                        hv_NumChunks.Dispose();
                        hv_NumChunks = 4;
                    }
                    hv_Colors.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Colors = HTuple.TupleGenConst(
                            hv_NumColors, -1);
                    }
                    //Check if the Number of Colors is dividable by NumChunks.
                    hv_NumLeftOver.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumLeftOver = hv_NumColors % hv_NumChunks;
                    }
                    hv_ColorsPerChunk.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ColorsPerChunk = ((hv_NumColors / hv_NumChunks)).TupleInt()
                            ;
                    }
                    hv_StartIdx.Dispose();
                    hv_StartIdx = 0;
                    HTuple end_val19 = hv_NumChunks - 1;
                    HTuple step_val19 = 1;
                    for (hv_S = 0; hv_S.Continue(end_val19, step_val19); hv_S = hv_S.TupleAdd(step_val19))
                    {
                        hv_EndIdx.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_EndIdx = (hv_StartIdx + hv_ColorsPerChunk) - 1;
                        }
                        if ((int)(new HTuple(hv_S.TupleLess(hv_NumLeftOver))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_EndIdx = hv_EndIdx + 1;
                                    hv_EndIdx.Dispose();
                                    hv_EndIdx = ExpTmpLocalVar_EndIdx;
                                }
                            }
                        }
                        hv_IdxsLeft.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IdxsLeft = HTuple.TupleGenSequence(
                                hv_S, hv_NumColors - 1, hv_NumChunks);
                        }
                        hv_IdxsRight.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IdxsRight = HTuple.TupleGenSequence(
                                hv_StartIdx, hv_EndIdx, 1);
                        }
                        if (hv_Colors == null)
                            hv_Colors = new HTuple();
                        hv_Colors[HTuple.TupleGenSequence(hv_S, hv_NumColors - 1, hv_NumChunks)] = hv_ColorsRainbow.TupleSelectRange(
                            hv_StartIdx, hv_EndIdx);
                        hv_StartIdx.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_StartIdx = hv_EndIdx + 1;
                        }
                    }
                }
                else
                {
                    hv_Colors.Dispose();
                    hv_Colors = new HTuple(hv_ColorsRainbow);
                }

                hv_NumColors.Dispose();
                hv_NumChunks.Dispose();
                hv_NumLeftOver.Dispose();
                hv_ColorsPerChunk.Dispose();
                hv_StartIdx.Dispose();
                hv_S.Dispose();
                hv_EndIdx.Dispose();
                hv_IdxsLeft.Dispose();
                hv_IdxsRight.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_NumColors.Dispose();
                hv_NumChunks.Dispose();
                hv_NumLeftOver.Dispose();
                hv_ColorsPerChunk.Dispose();
                hv_StartIdx.Dispose();
                hv_S.Dispose();
                hv_EndIdx.Dispose();
                hv_IdxsLeft.Dispose();
                hv_IdxsRight.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Window
        // Short Description: Open a window next to the given WindowHandleFather.  
        public void open_child_window(HTuple hv_WindowHandleFather, HTuple hv_Font, HTuple hv_FontSize,
            HTuple hv_Text, HTuple hv_PrevWindowCoordinates, HTuple hv_WindowHandleDict,
            HTuple hv_WindowHandleKey, out HTuple hv_WindowHandleChild, out HTuple hv_PrevWindowCoordinatesOut)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_StringWidth = new HTuple(), hv_IndexText = new HTuple();
            HTuple hv__ = new HTuple(), hv_TextWidth = new HTuple();
            HTuple hv_WindowRow = new HTuple(), hv_WindowColumn = new HTuple();
            HTuple hv_WindowWidth = new HTuple(), hv_WindowHeight = new HTuple();
            HTuple hv_MetaInfo = new HTuple();
            // Initialize local and output iconic variables 
            hv_WindowHandleChild = new HTuple();
            hv_PrevWindowCoordinatesOut = new HTuple();
            try
            {
                //
                //This procedure opens a window next to the given WindowHandleFather.
                //
                //Get the maximum width of the text to be displayed.
                //The width should be at leat 200.
                hv_StringWidth.Dispose();
                hv_StringWidth = 150;
                for (hv_IndexText = 0; (int)hv_IndexText <= (int)((new HTuple(hv_Text.TupleLength()
                    )) - 1); hv_IndexText = (int)hv_IndexText + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv__.Dispose(); hv__.Dispose(); hv_TextWidth.Dispose(); hv__.Dispose();
                        HOperatorSet.GetStringExtents(hv_WindowHandleFather, hv_Text.TupleSelect(
                            hv_IndexText), out hv__, out hv__, out hv_TextWidth, out hv__);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_StringWidth = hv_StringWidth.TupleMax2(
                                hv_TextWidth);
                            hv_StringWidth.Dispose();
                            hv_StringWidth = ExpTmpLocalVar_StringWidth;
                        }
                    }
                }
                //
                //Define window coordinates.
                hv_WindowRow.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowRow = hv_PrevWindowCoordinates.TupleSelect(
                        0);
                }
                hv_WindowColumn.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowColumn = ((hv_PrevWindowCoordinates.TupleSelect(
                        1)) + (hv_PrevWindowCoordinates.TupleSelect(2))) + 5;
                }
                hv_WindowWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowWidth = hv_StringWidth + (2 * 12.0);
                }
                hv_WindowHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowHeight = hv_PrevWindowCoordinates.TupleSelect(
                        3);
                }
                //
                HOperatorSet.SetWindowAttr("background_color", "black");
                HOperatorSet.OpenWindow(hv_WindowRow, hv_WindowColumn, hv_WindowWidth, hv_WindowHeight, 0, "visible", "", out hv_WindowHandleChild);
                HDevWindowStack.Push(hv_WindowHandleChild);
                set_display_font(hv_WindowHandleChild, hv_FontSize, hv_Font, "true", "false");
                //
                //Return the coordinates of the new window.
                hv_PrevWindowCoordinatesOut.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PrevWindowCoordinatesOut = new HTuple();
                    hv_PrevWindowCoordinatesOut = hv_PrevWindowCoordinatesOut.TupleConcat(hv_WindowRow, hv_WindowColumn, hv_WindowWidth, hv_WindowHeight);
                }
                //
                //Set some meta information about the new child window handle.
                hv_MetaInfo.Dispose();
                HOperatorSet.GetDictTuple(hv_WindowHandleDict, "meta_information", out hv_MetaInfo);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_child_window_coordinates",
                        hv_PrevWindowCoordinatesOut);
                }
                HOperatorSet.SetDictTuple(hv_WindowHandleDict, "meta_information", hv_MetaInfo);
                //

                hv_StringWidth.Dispose();
                hv_IndexText.Dispose();
                hv__.Dispose();
                hv_TextWidth.Dispose();
                hv_WindowRow.Dispose();
                hv_WindowColumn.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();
                hv_MetaInfo.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_StringWidth.Dispose();
                hv_IndexText.Dispose();
                hv__.Dispose();
                hv_TextWidth.Dispose();
                hv_WindowRow.Dispose();
                hv_WindowColumn.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();
                hv_MetaInfo.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Window
        // Short Description: Open a new window, either next to the last ones, or in a new row. 
        public void open_next_window(HTuple hv_Font, HTuple hv_FontSize, HTuple hv_ShowBottomDesc,
            HTuple hv_WidthImage, HTuple hv_HeightImage, HTuple hv_MapColorBarWidth, HTuple hv_ScaleWindows,
            HTuple hv_ThresholdWidth, HTuple hv_PrevWindowCoordinates, HTuple hv_WindowHandleDict,
            HTuple hv_WindowHandleKey, out HTuple hv_WindowHandleNew, out HTuple hv_WindowImageRatioHeight,
            out HTuple hv_PrevWindowCoordinatesOut)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_PrevWindowRow = new HTuple(), hv_PrevWindowColumn = new HTuple();
            HTuple hv_PrevWindowWidth = new HTuple(), hv_PrevWindowHeight = new HTuple();
            HTuple hv_WindowRow = new HTuple(), hv_WindowColumn = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv__ = new HTuple(), hv_MarginBottom = new HTuple();
            HTuple hv_WindowWidth = new HTuple(), hv_WindowHeight = new HTuple();
            HTuple hv_WindowImageRatioWidth = new HTuple(), hv_SetPartRow2 = new HTuple();
            HTuple hv_SetPartColumn2 = new HTuple(), hv_MetaInfo = new HTuple();
            // Initialize local and output iconic variables 
            hv_WindowHandleNew = new HTuple();
            hv_WindowImageRatioHeight = new HTuple();
            hv_PrevWindowCoordinatesOut = new HTuple();
            try
            {
                //
                //This procedure opens a new window, either next to
                //the last ones, or in a new row.
                //
                //Get coordinates of previous window.
                hv_PrevWindowRow.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PrevWindowRow = hv_PrevWindowCoordinates.TupleSelect(
                        0);
                }
                hv_PrevWindowColumn.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PrevWindowColumn = hv_PrevWindowCoordinates.TupleSelect(
                        1);
                }
                hv_PrevWindowWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PrevWindowWidth = hv_PrevWindowCoordinates.TupleSelect(
                        2);
                }
                hv_PrevWindowHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PrevWindowHeight = hv_PrevWindowCoordinates.TupleSelect(
                        3);
                }
                //
                if ((int)(new HTuple(((hv_PrevWindowColumn + hv_PrevWindowWidth)).TupleGreater(
                    hv_ThresholdWidth))) != 0)
                {
                    //Open window in new row.
                    hv_WindowRow.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WindowRow = (hv_PrevWindowRow + hv_PrevWindowHeight) + 55;
                    }
                    hv_WindowColumn.Dispose();
                    hv_WindowColumn = 0;
                }
                else
                {
                    //Open window in same row.
                    hv_WindowRow.Dispose();
                    hv_WindowRow = new HTuple(hv_PrevWindowRow);
                    hv_WindowColumn.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WindowColumn = hv_PrevWindowColumn + hv_PrevWindowWidth;
                    }
                    if ((int)(new HTuple(hv_WindowColumn.TupleNotEqual(0))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_WindowColumn = hv_WindowColumn + 5;
                                hv_WindowColumn.Dispose();
                                hv_WindowColumn = ExpTmpLocalVar_WindowColumn;
                            }
                        }
                    }
                }
                //
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowHandleNew.Dispose();
                    dev_open_window_fit_size(hv_WindowRow, hv_WindowColumn, hv_WidthImage, hv_HeightImage,
                        (new HTuple(500)).TupleConcat(800) * hv_ScaleWindows, (new HTuple(400)).TupleConcat(
                        600) * hv_ScaleWindows, out hv_WindowHandleNew);
                }
                set_display_font(hv_WindowHandleNew, hv_FontSize, hv_Font, "true", "false");
                //
                //Add MarginBottom and MapColorBarWidth to window.
                if ((int)(hv_ShowBottomDesc) != 0)
                {
                    hv_Ascent.Dispose(); hv_Descent.Dispose(); hv__.Dispose(); hv__.Dispose();
                    HOperatorSet.GetStringExtents(hv_WindowHandleNew, "Test_string", out hv_Ascent,
                        out hv_Descent, out hv__, out hv__);
                    hv_MarginBottom.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MarginBottom = ((2 * 12) + hv_Ascent) + hv_Descent;
                    }
                }
                else
                {
                    hv_MarginBottom.Dispose();
                    hv_MarginBottom = 0;
                }
                hv__.Dispose(); hv__.Dispose(); hv_WindowWidth.Dispose(); hv_WindowHeight.Dispose();
                HOperatorSet.GetWindowExtents(hv_WindowHandleNew, out hv__, out hv__, out hv_WindowWidth,
                    out hv_WindowHeight);
                if (HDevWindowStack.IsOpen())
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetWindowExtents(HDevWindowStack.GetActive(), hv_WindowRow,
                            hv_WindowColumn, hv_WindowWidth + hv_MapColorBarWidth, hv_WindowHeight + hv_MarginBottom);
                    }
                }
                //
                //Get and set meta information of new window handle.
                hv_WindowImageRatioHeight.Dispose(); hv_WindowImageRatioWidth.Dispose(); hv_SetPartRow2.Dispose(); hv_SetPartColumn2.Dispose(); hv_PrevWindowCoordinatesOut.Dispose();
                update_window_meta_information(hv_WindowHandleNew, hv_WidthImage, hv_HeightImage,
                    hv_WindowRow, hv_WindowColumn, hv_MapColorBarWidth, hv_MarginBottom, out hv_WindowImageRatioHeight,
                    out hv_WindowImageRatioWidth, out hv_SetPartRow2, out hv_SetPartColumn2,
                    out hv_PrevWindowCoordinatesOut);
                //
                //Set window handle and some meta information about the new window handle.
                HOperatorSet.SetDictTuple(hv_WindowHandleDict, hv_WindowHandleKey, hv_WindowHandleNew);
                hv_MetaInfo.Dispose();
                HOperatorSet.GetDictTuple(hv_WindowHandleDict, "meta_information", out hv_MetaInfo);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_window_image_ratio_height",
                        hv_WindowImageRatioHeight);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_window_image_ratio_width",
                        hv_WindowImageRatioWidth);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_set_part_row2",
                        hv_SetPartRow2);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_set_part_column2",
                        hv_SetPartColumn2);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_margin_bottom",
                        hv_MarginBottom);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_map_color_bar_with",
                        hv_MapColorBarWidth);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictTuple(hv_MetaInfo, hv_WindowHandleKey + "_window_coordinates",
                        hv_PrevWindowCoordinatesOut);
                }
                HOperatorSet.SetDictTuple(hv_WindowHandleDict, "meta_information", hv_MetaInfo);
                //

                hv_PrevWindowRow.Dispose();
                hv_PrevWindowColumn.Dispose();
                hv_PrevWindowWidth.Dispose();
                hv_PrevWindowHeight.Dispose();
                hv_WindowRow.Dispose();
                hv_WindowColumn.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv__.Dispose();
                hv_MarginBottom.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();
                hv_WindowImageRatioWidth.Dispose();
                hv_SetPartRow2.Dispose();
                hv_SetPartColumn2.Dispose();
                hv_MetaInfo.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_PrevWindowRow.Dispose();
                hv_PrevWindowColumn.Dispose();
                hv_PrevWindowWidth.Dispose();
                hv_PrevWindowHeight.Dispose();
                hv_WindowRow.Dispose();
                hv_WindowColumn.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv__.Dispose();
                hv_MarginBottom.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();
                hv_WindowImageRatioWidth.Dispose();
                hv_SetPartRow2.Dispose();
                hv_SetPartColumn2.Dispose();
                hv_MetaInfo.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess anomaly images for evaluation and visualization of the deep-learning-based anomaly detection. 
        public void preprocess_dl_model_anomaly(HObject ho_AnomalyImages, out HObject ho_AnomalyImagesPreprocessed,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            // Local copy input parameter variables 
            HObject ho_AnomalyImages_COPY_INP_TMP;
            ho_AnomalyImages_COPY_INP_TMP = new HObject(ho_AnomalyImages);



            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_Min = new HTuple();
            HTuple hv_Max = new HTuple(), hv_Range = new HTuple();
            HTuple hv_ImageWidthInput = new HTuple(), hv_ImageHeightInput = new HTuple();
            HTuple hv_EqualWidth = new HTuple(), hv_EqualHeight = new HTuple();
            HTuple hv_Type = new HTuple(), hv_NumMatches = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_EqualByte = new HTuple();
            HTuple hv_NumChannelsAllImages = new HTuple(), hv_ImageNumChannelsTuple = new HTuple();
            HTuple hv_IndicesWrongChannels = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_AnomalyImagesPreprocessed);
            try
            {
                //
                //This procedure preprocesses the anomaly images given by AnomalyImages
                //according to the parameters in the dictionary DLPreprocessParam.
                //Note that depending on the images,
                //additional preprocessing steps might be beneficial.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                hv_ModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_ModelType);
                //
                hv_ImageNumChannels.Dispose();
                hv_ImageNumChannels = 1;
                //
                //Preprocess the images.
                //
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.CropDomain(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)((new HTuple(hv_DomainHandling.TupleEqual("keep_domain"))).TupleAnd(
                    new HTuple(hv_ModelType.TupleEqual("anomaly_detection")))) != 0)
                {
                    //Anomaly detection models accept the additional option 'keep_domain'.
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                HOperatorSet.MinMaxGray(ho_AnomalyImages_COPY_INP_TMP, ho_AnomalyImages_COPY_INP_TMP,
                    0, out hv_Min, out hv_Max, out hv_Range);
                if ((int)(new HTuple(hv_Min.TupleLess(0.0))) != 0)
                {
                    throw new HalconException("Values of anomaly image must not be smaller than 0.0.");
                }
                //
                //Zoom images only if they have a different size than the specified size.
                hv_ImageWidthInput.Dispose(); hv_ImageHeightInput.Dispose();
                HOperatorSet.GetImageSize(ho_AnomalyImages_COPY_INP_TMP, out hv_ImageWidthInput,
                    out hv_ImageHeightInput);
                hv_EqualWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualWidth = hv_ImageWidth.TupleEqualElem(
                        hv_ImageWidthInput);
                }
                hv_EqualHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualHeight = hv_ImageHeight.TupleEqualElem(
                        hv_ImageHeightInput);
                }
                if ((int)((new HTuple(((hv_EqualWidth.TupleMin())).TupleEqual(0))).TupleOr(
                    new HTuple(((hv_EqualHeight.TupleMin())).TupleEqual(0)))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0,
                            hv_ImageWidth, hv_ImageHeight, "nearest_neighbor");
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the type of the input images.
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_AnomalyImages_COPY_INP_TMP, out hv_Type);
                hv_NumMatches.Dispose();
                HOperatorSet.TupleRegexpTest(hv_Type, "byte|real", out hv_NumMatches);
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_AnomalyImages_COPY_INP_TMP, out hv_NumImages);
                if ((int)(new HTuple(hv_NumMatches.TupleNotEqual(hv_NumImages))) != 0)
                {
                    throw new HalconException("Please provide only images of type 'byte' or 'real'.");
                }
                //
                //If the type is 'byte', convert it to 'real' and scale it.
                //The gray value scaling does not work on 'byte' images.
                //For 'real' images it is assumed that the range is already correct.
                hv_EqualByte.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualByte = hv_Type.TupleEqualElem(
                        "byte");
                }
                if ((int)(new HTuple(((hv_EqualByte.TupleMax())).TupleEqual(1))) != 0)
                {
                    if ((int)(new HTuple(((hv_EqualByte.TupleMin())).TupleEqual(0))) != 0)
                    {
                        throw new HalconException("Passing mixed type images is not supported.");
                    }
                    //Convert the image type from 'byte' to 'real',
                    //because the model expects 'real' images.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the number of channels.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_AnomalyImages_COPY_INP_TMP, out hv_NumImages);
                //Check all images for number of channels.
                hv_NumChannelsAllImages.Dispose();
                HOperatorSet.CountChannels(ho_AnomalyImages_COPY_INP_TMP, out hv_NumChannelsAllImages);
                hv_ImageNumChannelsTuple.Dispose();
                HOperatorSet.TupleGenConst(hv_NumImages, hv_ImageNumChannels, out hv_ImageNumChannelsTuple);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_IndicesWrongChannels.Dispose();
                    HOperatorSet.TupleFind(hv_NumChannelsAllImages.TupleNotEqualElem(hv_ImageNumChannelsTuple),
                        1, out hv_IndicesWrongChannels);
                }
                //
                //Check for anomaly image channels.
                //Only single channel images are accepted.
                if ((int)(new HTuple(hv_IndicesWrongChannels.TupleNotEqual(-1))) != 0)
                {
                    throw new HalconException("Number of channels in anomaly image is not supported. Please check for anomaly images with a number of channels different from 1.");
                }
                //
                //Write preprocessed image to output variable.
                ho_AnomalyImagesPreprocessed.Dispose();
                ho_AnomalyImagesPreprocessed = new HObject(ho_AnomalyImages_COPY_INP_TMP);
                //
                ho_AnomalyImages_COPY_INP_TMP.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_ModelType.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_ImageWidthInput.Dispose();
                hv_ImageHeightInput.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_NumImages.Dispose();
                hv_EqualByte.Dispose();
                hv_NumChannelsAllImages.Dispose();
                hv_ImageNumChannelsTuple.Dispose();
                hv_IndicesWrongChannels.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_AnomalyImages_COPY_INP_TMP.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_ModelType.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_ImageWidthInput.Dispose();
                hv_ImageHeightInput.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_NumImages.Dispose();
                hv_EqualByte.Dispose();
                hv_NumChannelsAllImages.Dispose();
                hv_ImageNumChannelsTuple.Dispose();
                hv_IndicesWrongChannels.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Object Detection
        // Short Description: This procedure preprocesses the bounding boxes of type 'rectangle1' for a given sample. 
        public void preprocess_dl_model_bbox_rect1(HObject ho_ImageRaw, HTuple hv_DLSample,
            HTuple hv_DLPreprocessParam)
        {




            // Local iconic variables 

            HObject ho_DomainRaw = null;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_BBoxCol1 = new HTuple();
            HTuple hv_BBoxCol2 = new HTuple(), hv_BBoxRow1 = new HTuple();
            HTuple hv_BBoxRow2 = new HTuple(), hv_BBoxLabel = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_ImageId = new HTuple();
            HTuple hv_ExceptionMessage = new HTuple(), hv_BoxesInvalid = new HTuple();
            HTuple hv_RowDomain1 = new HTuple(), hv_ColumnDomain1 = new HTuple();
            HTuple hv_RowDomain2 = new HTuple(), hv_ColumnDomain2 = new HTuple();
            HTuple hv_WidthRaw = new HTuple(), hv_HeightRaw = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Col1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Col2 = new HTuple();
            HTuple hv_MaskDelete = new HTuple(), hv_MaskNewBbox = new HTuple();
            HTuple hv_BBoxCol1New = new HTuple(), hv_BBoxCol2New = new HTuple();
            HTuple hv_BBoxRow1New = new HTuple(), hv_BBoxRow2New = new HTuple();
            HTuple hv_BBoxLabelNew = new HTuple(), hv_FactorResampleWidth = new HTuple();
            HTuple hv_FactorResampleHeight = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_DomainRaw);
            try
            {
                //
                //This procedure preprocesses the bounding boxes of type 'rectangle1' for a given sample.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                //
                //Get bounding box coordinates and labels.
                try
                {
                    hv_BBoxCol1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col1", out hv_BBoxCol1);
                    hv_BBoxCol2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col2", out hv_BBoxCol2);
                    hv_BBoxRow1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row1", out hv_BBoxRow1);
                    hv_BBoxRow2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row2", out hv_BBoxRow2);
                    hv_BBoxLabel.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_label_id", out hv_BBoxLabel);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_ImageId.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                    if ((int)(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1302))) != 0)
                    {
                        hv_ExceptionMessage.Dispose();
                        hv_ExceptionMessage = "A bounding box coordinate key is missing.";
                    }
                    else
                    {
                        hv_ExceptionMessage.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ExceptionMessage = hv_Exception.TupleSelect(
                                2);
                        }
                    }
                    throw new HalconException((("An error has occurred during preprocessing image_id " + hv_ImageId) + " when getting bounding box coordinates : ") + hv_ExceptionMessage);
                }
                //
                //Check that there are no invalid boxes.
                if ((int)(new HTuple((new HTuple(hv_BBoxRow1.TupleLength())).TupleGreater(0))) != 0)
                {
                    hv_BoxesInvalid.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BoxesInvalid = ((hv_BBoxRow1.TupleGreaterEqualElem(
                            hv_BBoxRow2))).TupleOr(hv_BBoxCol1.TupleGreaterEqualElem(hv_BBoxCol2));
                    }
                    if ((int)(new HTuple(((hv_BoxesInvalid.TupleSum())).TupleGreater(0))) != 0)
                    {
                        hv_ImageId.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                        throw new HalconException(("An error has occurred during preprocessing image_id " + hv_ImageId) + new HTuple(": Sample contains at least one box with zero-area, i.e. bbox_col1 >= bbox_col2 or bbox_row1 >= bbox_row2."));
                    }
                }
                else
                {
                    //If there are no bounding box coordinates, there is nothing to do.
                    ho_DomainRaw.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_DomainHandling.Dispose();
                    hv_BBoxCol1.Dispose();
                    hv_BBoxCol2.Dispose();
                    hv_BBoxRow1.Dispose();
                    hv_BBoxRow2.Dispose();
                    hv_BBoxLabel.Dispose();
                    hv_Exception.Dispose();
                    hv_ImageId.Dispose();
                    hv_ExceptionMessage.Dispose();
                    hv_BoxesInvalid.Dispose();
                    hv_RowDomain1.Dispose();
                    hv_ColumnDomain1.Dispose();
                    hv_RowDomain2.Dispose();
                    hv_ColumnDomain2.Dispose();
                    hv_WidthRaw.Dispose();
                    hv_HeightRaw.Dispose();
                    hv_Row1.Dispose();
                    hv_Col1.Dispose();
                    hv_Row2.Dispose();
                    hv_Col2.Dispose();
                    hv_MaskDelete.Dispose();
                    hv_MaskNewBbox.Dispose();
                    hv_BBoxCol1New.Dispose();
                    hv_BBoxCol2New.Dispose();
                    hv_BBoxRow1New.Dispose();
                    hv_BBoxRow2New.Dispose();
                    hv_BBoxLabelNew.Dispose();
                    hv_FactorResampleWidth.Dispose();
                    hv_FactorResampleHeight.Dispose();

                    return;
                }
                //
                //If the domain is cropped, crop bounding boxes.
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    //
                    //Get domain.
                    ho_DomainRaw.Dispose();
                    HOperatorSet.GetDomain(ho_ImageRaw, out ho_DomainRaw);
                    //
                    //Set the size of the raw image to the domain extensions.
                    hv_RowDomain1.Dispose(); hv_ColumnDomain1.Dispose(); hv_RowDomain2.Dispose(); hv_ColumnDomain2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_DomainRaw, out hv_RowDomain1, out hv_ColumnDomain1,
                        out hv_RowDomain2, out hv_ColumnDomain2);
                    //The domain is always given as a pixel-precise region.
                    hv_WidthRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WidthRaw = (hv_ColumnDomain2 - hv_ColumnDomain1) + 1.0;
                    }
                    hv_HeightRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_HeightRaw = (hv_RowDomain2 - hv_RowDomain1) + 1.0;
                    }
                    //
                    //Crop the bounding boxes.
                    hv_Row1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row1 = hv_BBoxRow1.TupleMax2(
                            hv_RowDomain1 - .5);
                    }
                    hv_Col1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Col1 = hv_BBoxCol1.TupleMax2(
                            hv_ColumnDomain1 - .5);
                    }
                    hv_Row2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row2 = hv_BBoxRow2.TupleMin2(
                            hv_RowDomain2 + .5);
                    }
                    hv_Col2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Col2 = hv_BBoxCol2.TupleMin2(
                            hv_ColumnDomain2 + .5);
                    }
                    hv_MaskDelete.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskDelete = ((hv_Row1.TupleGreaterEqualElem(
                            hv_Row2))).TupleOr(hv_Col1.TupleGreaterEqualElem(hv_Col2));
                    }
                    hv_MaskNewBbox.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskNewBbox = 1 - hv_MaskDelete;
                    }
                    //Store the preprocessed bounding box entries.
                    hv_BBoxCol1New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxCol1New = (hv_Col1.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_ColumnDomain1;
                    }
                    hv_BBoxCol2New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxCol2New = (hv_Col2.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_ColumnDomain1;
                    }
                    hv_BBoxRow1New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxRow1New = (hv_Row1.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_RowDomain1;
                    }
                    hv_BBoxRow2New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxRow2New = (hv_Row2.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_RowDomain1;
                    }
                    hv_BBoxLabelNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLabelNew = hv_BBoxLabel.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    //
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    //If the entire image is used, set the variables accordingly.
                    //Get the original size.
                    hv_WidthRaw.Dispose(); hv_HeightRaw.Dispose();
                    HOperatorSet.GetImageSize(ho_ImageRaw, out hv_WidthRaw, out hv_HeightRaw);
                    //Set new coordinates to input coordinates.
                    hv_BBoxCol1New.Dispose();
                    hv_BBoxCol1New = new HTuple(hv_BBoxCol1);
                    hv_BBoxCol2New.Dispose();
                    hv_BBoxCol2New = new HTuple(hv_BBoxCol2);
                    hv_BBoxRow1New.Dispose();
                    hv_BBoxRow1New = new HTuple(hv_BBoxRow1);
                    hv_BBoxRow2New.Dispose();
                    hv_BBoxRow2New = new HTuple(hv_BBoxRow2);
                    hv_BBoxLabelNew.Dispose();
                    hv_BBoxLabelNew = new HTuple(hv_BBoxLabel);
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Rescale the bounding boxes.
                //
                //Get required images width and height.
                //
                //Only rescale bounding boxes if the required image dimensions are not the raw dimensions.
                if ((int)((new HTuple(hv_ImageHeight.TupleNotEqual(hv_HeightRaw))).TupleOr(
                    new HTuple(hv_ImageWidth.TupleNotEqual(hv_WidthRaw)))) != 0)
                {
                    //Calculate rescaling factor.
                    hv_FactorResampleWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleWidth = (hv_ImageWidth.TupleReal()
                            ) / hv_WidthRaw;
                    }
                    hv_FactorResampleHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleHeight = (hv_ImageHeight.TupleReal()
                            ) / hv_HeightRaw;
                    }
                    //Rescale the bounding box coordinates.
                    //As we use XLD-coordinates we temporarily move the boxes by (.5,.5) for rescaling.
                    //Doing so, the center of the XLD-coordinate system (-0.5,-0.5) is used
                    //for scaling, hence the scaling is performed w.r.t. the pixel coordinate system.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol1New = ((hv_BBoxCol1New + .5) * hv_FactorResampleWidth) - .5;
                            hv_BBoxCol1New.Dispose();
                            hv_BBoxCol1New = ExpTmpLocalVar_BBoxCol1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol2New = ((hv_BBoxCol2New + .5) * hv_FactorResampleWidth) - .5;
                            hv_BBoxCol2New.Dispose();
                            hv_BBoxCol2New = ExpTmpLocalVar_BBoxCol2New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow1New = ((hv_BBoxRow1New + .5) * hv_FactorResampleHeight) - .5;
                            hv_BBoxRow1New.Dispose();
                            hv_BBoxRow1New = ExpTmpLocalVar_BBoxRow1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow2New = ((hv_BBoxRow2New + .5) * hv_FactorResampleHeight) - .5;
                            hv_BBoxRow2New.Dispose();
                            hv_BBoxRow2New = ExpTmpLocalVar_BBoxRow2New;
                        }
                    }
                    //
                }
                //
                //Make a final check and remove bounding boxes that have zero area.
                if ((int)(new HTuple((new HTuple(hv_BBoxRow1New.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    hv_MaskDelete.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskDelete = ((hv_BBoxRow1New.TupleGreaterEqualElem(
                            hv_BBoxRow2New))).TupleOr(hv_BBoxCol1New.TupleGreaterEqualElem(hv_BBoxCol2New));
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol1New = hv_BBoxCol1New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxCol1New.Dispose();
                            hv_BBoxCol1New = ExpTmpLocalVar_BBoxCol1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol2New = hv_BBoxCol2New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxCol2New.Dispose();
                            hv_BBoxCol2New = ExpTmpLocalVar_BBoxCol2New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow1New = hv_BBoxRow1New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxRow1New.Dispose();
                            hv_BBoxRow1New = ExpTmpLocalVar_BBoxRow1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow2New = hv_BBoxRow2New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxRow2New.Dispose();
                            hv_BBoxRow2New = ExpTmpLocalVar_BBoxRow2New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxLabelNew = hv_BBoxLabelNew.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxLabelNew.Dispose();
                            hv_BBoxLabelNew = ExpTmpLocalVar_BBoxLabelNew;
                        }
                    }
                }
                //
                //Set new bounding box coordinates in the dictionary.
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_col1", hv_BBoxCol1New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_col2", hv_BBoxCol2New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_row1", hv_BBoxRow1New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_row2", hv_BBoxRow2New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_label_id", hv_BBoxLabelNew);
                //
                ho_DomainRaw.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_RowDomain1.Dispose();
                hv_ColumnDomain1.Dispose();
                hv_RowDomain2.Dispose();
                hv_ColumnDomain2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Row2.Dispose();
                hv_Col2.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_DomainRaw.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_RowDomain1.Dispose();
                hv_ColumnDomain1.Dispose();
                hv_RowDomain2.Dispose();
                hv_ColumnDomain2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Row2.Dispose();
                hv_Col2.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Object Detection
        // Short Description: This procedure preprocesses the bounding boxes of type 'rectangle2' for a given sample. 
        public void preprocess_dl_model_bbox_rect2(HObject ho_ImageRaw, HTuple hv_DLSample,
            HTuple hv_DLPreprocessParam)
        {




            // Local iconic variables 

            HObject ho_DomainRaw = null, ho_Rectangle2XLD = null;
            HObject ho_Rectangle2XLDSheared = null;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_IgnoreDirection = new HTuple();
            HTuple hv_ClassIDsNoOrientation = new HTuple(), hv_KeyExists = new HTuple();
            HTuple hv_BBoxRow = new HTuple(), hv_BBoxCol = new HTuple();
            HTuple hv_BBoxLength1 = new HTuple(), hv_BBoxLength2 = new HTuple();
            HTuple hv_BBoxPhi = new HTuple(), hv_BBoxLabel = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_ImageId = new HTuple();
            HTuple hv_ExceptionMessage = new HTuple(), hv_BoxesInvalid = new HTuple();
            HTuple hv_RowDomain1 = new HTuple(), hv_ColumnDomain1 = new HTuple();
            HTuple hv_RowDomain2 = new HTuple(), hv_ColumnDomain2 = new HTuple();
            HTuple hv_WidthRaw = new HTuple(), hv_HeightRaw = new HTuple();
            HTuple hv_MaskDelete = new HTuple(), hv_MaskNewBbox = new HTuple();
            HTuple hv_BBoxRowNew = new HTuple(), hv_BBoxColNew = new HTuple();
            HTuple hv_BBoxLength1New = new HTuple(), hv_BBoxLength2New = new HTuple();
            HTuple hv_BBoxPhiNew = new HTuple(), hv_BBoxLabelNew = new HTuple();
            HTuple hv_ClassIDsNoOrientationIndices = new HTuple();
            HTuple hv_Index = new HTuple(), hv_ClassIDsNoOrientationIndicesTmp = new HTuple();
            HTuple hv_DirectionLength1Row = new HTuple(), hv_DirectionLength1Col = new HTuple();
            HTuple hv_DirectionLength2Row = new HTuple(), hv_DirectionLength2Col = new HTuple();
            HTuple hv_Corner1Row = new HTuple(), hv_Corner1Col = new HTuple();
            HTuple hv_Corner2Row = new HTuple(), hv_Corner2Col = new HTuple();
            HTuple hv_FactorResampleWidth = new HTuple(), hv_FactorResampleHeight = new HTuple();
            HTuple hv_BBoxCol1 = new HTuple(), hv_BBoxCol1New = new HTuple();
            HTuple hv_BBoxCol2 = new HTuple(), hv_BBoxCol2New = new HTuple();
            HTuple hv_BBoxCol3 = new HTuple(), hv_BBoxCol3New = new HTuple();
            HTuple hv_BBoxCol4 = new HTuple(), hv_BBoxCol4New = new HTuple();
            HTuple hv_BBoxRow1 = new HTuple(), hv_BBoxRow1New = new HTuple();
            HTuple hv_BBoxRow2 = new HTuple(), hv_BBoxRow2New = new HTuple();
            HTuple hv_BBoxRow3 = new HTuple(), hv_BBoxRow3New = new HTuple();
            HTuple hv_BBoxRow4 = new HTuple(), hv_BBoxRow4New = new HTuple();
            HTuple hv_HomMat2DIdentity = new HTuple(), hv_HomMat2DScale = new HTuple();
            HTuple hv_BBoxPhiTmp = new HTuple(), hv_PhiDelta = new HTuple();
            HTuple hv_PhiDeltaNegativeIndices = new HTuple(), hv_IndicesRot90 = new HTuple();
            HTuple hv_IndicesRot180 = new HTuple(), hv_IndicesRot270 = new HTuple();
            HTuple hv_SwapIndices = new HTuple(), hv_Tmp = new HTuple();
            HTuple hv_BBoxPhiNewIndices = new HTuple(), hv__ = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_DomainRaw);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2XLD);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2XLDSheared);
            try
            {
                //This procedure preprocesses the bounding boxes of type 'rectangle2' for a given sample.
                //
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get preprocess parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                //The keys 'ignore_direction' and 'class_ids_no_orientation' are optional.
                hv_IgnoreDirection.Dispose();
                hv_IgnoreDirection = 0;
                hv_ClassIDsNoOrientation.Dispose();
                hv_ClassIDsNoOrientation = new HTuple();
                hv_KeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", (new HTuple("ignore_direction")).TupleConcat(
                    "class_ids_no_orientation"), out hv_KeyExists);
                if ((int)(hv_KeyExists.TupleSelect(0)) != 0)
                {
                    hv_IgnoreDirection.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_direction", out hv_IgnoreDirection);
                    if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("true"))) != 0)
                    {
                        hv_IgnoreDirection.Dispose();
                        hv_IgnoreDirection = 1;
                    }
                    else if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("false"))) != 0)
                    {
                        hv_IgnoreDirection.Dispose();
                        hv_IgnoreDirection = 0;
                    }
                }
                if ((int)(hv_KeyExists.TupleSelect(1)) != 0)
                {
                    hv_ClassIDsNoOrientation.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "class_ids_no_orientation",
                        out hv_ClassIDsNoOrientation);
                }
                //
                //Get bounding box coordinates and labels.
                try
                {
                    hv_BBoxRow.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row", out hv_BBoxRow);
                    hv_BBoxCol.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col", out hv_BBoxCol);
                    hv_BBoxLength1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_length1", out hv_BBoxLength1);
                    hv_BBoxLength2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_length2", out hv_BBoxLength2);
                    hv_BBoxPhi.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_phi", out hv_BBoxPhi);
                    hv_BBoxLabel.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_label_id", out hv_BBoxLabel);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_ImageId.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                    if ((int)(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1302))) != 0)
                    {
                        hv_ExceptionMessage.Dispose();
                        hv_ExceptionMessage = "A bounding box coordinate key is missing.";
                    }
                    else
                    {
                        hv_ExceptionMessage.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ExceptionMessage = hv_Exception.TupleSelect(
                                2);
                        }
                    }
                    throw new HalconException((("An error has occurred during preprocessing image_id " + hv_ImageId) + " when getting bounding box coordinates : ") + hv_ExceptionMessage);
                }
                //
                //Check that there are no invalid boxes.
                if ((int)(new HTuple((new HTuple(hv_BBoxRow.TupleLength())).TupleGreater(0))) != 0)
                {
                    hv_BoxesInvalid.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BoxesInvalid = (((hv_BBoxLength1.TupleEqualElem(
                            0))).TupleSum()) + (((hv_BBoxLength2.TupleEqualElem(0))).TupleSum());
                    }
                    if ((int)(new HTuple(hv_BoxesInvalid.TupleGreater(0))) != 0)
                    {
                        hv_ImageId.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                        throw new HalconException(("An error has occurred during preprocessing image_id " + hv_ImageId) + new HTuple(": Sample contains at least one bounding box with zero-area, i.e. bbox_length1 == 0 or bbox_length2 == 0!"));
                    }
                }
                //
                //If the domain is cropped, crop bounding boxes.
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    //
                    //Get domain.
                    ho_DomainRaw.Dispose();
                    HOperatorSet.GetDomain(ho_ImageRaw, out ho_DomainRaw);
                    //
                    //Set the size of the raw image to the domain extensions.
                    hv_RowDomain1.Dispose(); hv_ColumnDomain1.Dispose(); hv_RowDomain2.Dispose(); hv_ColumnDomain2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_DomainRaw, out hv_RowDomain1, out hv_ColumnDomain1,
                        out hv_RowDomain2, out hv_ColumnDomain2);
                    hv_WidthRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WidthRaw = (hv_ColumnDomain2 - hv_ColumnDomain1) + 1;
                    }
                    hv_HeightRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_HeightRaw = (hv_RowDomain2 - hv_RowDomain1) + 1;
                    }
                    //
                    //Crop the bounding boxes.
                    //Remove the boxes with center outside of the domain.
                    hv_MaskDelete.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskDelete = (new HTuple((new HTuple(((hv_BBoxRow.TupleLessElem(
                            hv_RowDomain1))).TupleOr(hv_BBoxCol.TupleLessElem(hv_ColumnDomain1)))).TupleOr(
                            hv_BBoxRow.TupleGreaterElem(hv_RowDomain2)))).TupleOr(hv_BBoxCol.TupleGreaterElem(
                            hv_ColumnDomain2));
                    }
                    hv_MaskNewBbox.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskNewBbox = 1 - hv_MaskDelete;
                    }
                    //Store the preprocessed bounding box entries.
                    hv_BBoxRowNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxRowNew = (hv_BBoxRow.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_RowDomain1;
                    }
                    hv_BBoxColNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxColNew = (hv_BBoxCol.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_ColumnDomain1;
                    }
                    hv_BBoxLength1New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLength1New = hv_BBoxLength1.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    hv_BBoxLength2New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLength2New = hv_BBoxLength2.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    hv_BBoxPhiNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxPhiNew = hv_BBoxPhi.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    hv_BBoxLabelNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLabelNew = hv_BBoxLabel.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    //
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    //If the entire image is used, set the variables accordingly.
                    //Get the original size.
                    hv_WidthRaw.Dispose(); hv_HeightRaw.Dispose();
                    HOperatorSet.GetImageSize(ho_ImageRaw, out hv_WidthRaw, out hv_HeightRaw);
                    //Set new coordinates to input coordinates.
                    hv_BBoxRowNew.Dispose();
                    hv_BBoxRowNew = new HTuple(hv_BBoxRow);
                    hv_BBoxColNew.Dispose();
                    hv_BBoxColNew = new HTuple(hv_BBoxCol);
                    hv_BBoxLength1New.Dispose();
                    hv_BBoxLength1New = new HTuple(hv_BBoxLength1);
                    hv_BBoxLength2New.Dispose();
                    hv_BBoxLength2New = new HTuple(hv_BBoxLength2);
                    hv_BBoxPhiNew.Dispose();
                    hv_BBoxPhiNew = new HTuple(hv_BBoxPhi);
                    hv_BBoxLabelNew.Dispose();
                    hv_BBoxLabelNew = new HTuple(hv_BBoxLabel);
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Generate smallest enclosing axis-aligned bounding box for classes in ClassIDsNoOrientation.
                hv_ClassIDsNoOrientationIndices.Dispose();
                hv_ClassIDsNoOrientationIndices = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ClassIDsNoOrientation.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    hv_ClassIDsNoOrientationIndicesTmp.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ClassIDsNoOrientationIndicesTmp = ((hv_BBoxLabelNew.TupleEqualElem(
                            hv_ClassIDsNoOrientation.TupleSelect(hv_Index)))).TupleFind(1);
                    }
                    if ((int)(new HTuple(hv_ClassIDsNoOrientationIndicesTmp.TupleNotEqual(-1))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_ClassIDsNoOrientationIndices = hv_ClassIDsNoOrientationIndices.TupleConcat(
                                    hv_ClassIDsNoOrientationIndicesTmp);
                                hv_ClassIDsNoOrientationIndices.Dispose();
                                hv_ClassIDsNoOrientationIndices = ExpTmpLocalVar_ClassIDsNoOrientationIndices;
                            }
                        }
                    }
                }
                if ((int)(new HTuple((new HTuple(hv_ClassIDsNoOrientationIndices.TupleLength()
                    )).TupleGreater(0))) != 0)
                {
                    //Calculate length1 and length2 using position of corners.
                    hv_DirectionLength1Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DirectionLength1Row = -(((hv_BBoxPhiNew.TupleSelect(
                            hv_ClassIDsNoOrientationIndices))).TupleSin());
                    }
                    hv_DirectionLength1Col.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DirectionLength1Col = ((hv_BBoxPhiNew.TupleSelect(
                            hv_ClassIDsNoOrientationIndices))).TupleCos();
                    }
                    hv_DirectionLength2Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DirectionLength2Row = -hv_DirectionLength1Col;
                    }
                    hv_DirectionLength2Col.Dispose();
                    hv_DirectionLength2Col = new HTuple(hv_DirectionLength1Row);
                    hv_Corner1Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner1Row = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Row) + ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Row);
                    }
                    hv_Corner1Col.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner1Col = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Col) + ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Col);
                    }
                    hv_Corner2Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner2Row = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Row) - ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Row);
                    }
                    hv_Corner2Col.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner2Col = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Col) - ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Col);
                    }
                    //
                    if (hv_BBoxPhiNew == null)
                        hv_BBoxPhiNew = new HTuple();
                    hv_BBoxPhiNew[hv_ClassIDsNoOrientationIndices] = 0.0;
                    if (hv_BBoxLength1New == null)
                        hv_BBoxLength1New = new HTuple();
                    hv_BBoxLength1New[hv_ClassIDsNoOrientationIndices] = ((hv_Corner1Col.TupleAbs()
                        )).TupleMax2(hv_Corner2Col.TupleAbs());
                    if (hv_BBoxLength2New == null)
                        hv_BBoxLength2New = new HTuple();
                    hv_BBoxLength2New[hv_ClassIDsNoOrientationIndices] = ((hv_Corner1Row.TupleAbs()
                        )).TupleMax2(hv_Corner2Row.TupleAbs());
                }
                //
                //Rescale bounding boxes.
                //
                //Get required images width and height.
                //
                //Only rescale bounding boxes if the required image dimensions are not the raw dimensions.
                if ((int)((new HTuple(hv_ImageHeight.TupleNotEqual(hv_HeightRaw))).TupleOr(
                    new HTuple(hv_ImageWidth.TupleNotEqual(hv_WidthRaw)))) != 0)
                {
                    //Calculate rescaling factor.
                    hv_FactorResampleWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleWidth = (hv_ImageWidth.TupleReal()
                            ) / hv_WidthRaw;
                    }
                    hv_FactorResampleHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleHeight = (hv_ImageHeight.TupleReal()
                            ) / hv_HeightRaw;
                    }
                    if ((int)(new HTuple(hv_FactorResampleHeight.TupleNotEqual(hv_FactorResampleWidth))) != 0)
                    {
                        //In order to preserve the correct orientation we have to transform the points individually.
                        //Get the coordinates of the four corner points.
                        hv_BBoxRow1.Dispose(); hv_BBoxCol1.Dispose(); hv_BBoxRow2.Dispose(); hv_BBoxCol2.Dispose(); hv_BBoxRow3.Dispose(); hv_BBoxCol3.Dispose(); hv_BBoxRow4.Dispose(); hv_BBoxCol4.Dispose();
                        convert_rect2_5to8param(hv_BBoxRowNew, hv_BBoxColNew, hv_BBoxLength1New,
                            hv_BBoxLength2New, hv_BBoxPhiNew, out hv_BBoxRow1, out hv_BBoxCol1,
                            out hv_BBoxRow2, out hv_BBoxCol2, out hv_BBoxRow3, out hv_BBoxCol3,
                            out hv_BBoxRow4, out hv_BBoxCol4);
                        //
                        //Rescale the coordinates.
                        hv_BBoxCol1New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol1New = hv_BBoxCol1 * hv_FactorResampleWidth;
                        }
                        hv_BBoxCol2New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol2New = hv_BBoxCol2 * hv_FactorResampleWidth;
                        }
                        hv_BBoxCol3New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol3New = hv_BBoxCol3 * hv_FactorResampleWidth;
                        }
                        hv_BBoxCol4New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol4New = hv_BBoxCol4 * hv_FactorResampleWidth;
                        }
                        hv_BBoxRow1New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow1New = hv_BBoxRow1 * hv_FactorResampleHeight;
                        }
                        hv_BBoxRow2New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow2New = hv_BBoxRow2 * hv_FactorResampleHeight;
                        }
                        hv_BBoxRow3New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow3New = hv_BBoxRow3 * hv_FactorResampleHeight;
                        }
                        hv_BBoxRow4New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow4New = hv_BBoxRow4 * hv_FactorResampleHeight;
                        }
                        //
                        //The rectangles will get sheared, that is why new rectangles have to be found.
                        //Generate homography to scale rectangles.
                        hv_HomMat2DIdentity.Dispose();
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        hv_HomMat2DScale.Dispose();
                        HOperatorSet.HomMat2dScale(hv_HomMat2DIdentity, hv_FactorResampleHeight,
                            hv_FactorResampleWidth, 0, 0, out hv_HomMat2DScale);
                        //Generate XLD contours for the rectangles.
                        ho_Rectangle2XLD.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle2XLD, hv_BBoxRowNew,
                            hv_BBoxColNew, hv_BBoxPhiNew, hv_BBoxLength1New, hv_BBoxLength2New);
                        //Scale the XLD contours --> results in sheared regions.
                        ho_Rectangle2XLDSheared.Dispose();
                        HOperatorSet.AffineTransContourXld(ho_Rectangle2XLD, out ho_Rectangle2XLDSheared,
                            hv_HomMat2DScale);
                        hv_BBoxRowNew.Dispose(); hv_BBoxColNew.Dispose(); hv_BBoxPhiNew.Dispose(); hv_BBoxLength1New.Dispose(); hv_BBoxLength2New.Dispose();
                        HOperatorSet.SmallestRectangle2Xld(ho_Rectangle2XLDSheared, out hv_BBoxRowNew,
                            out hv_BBoxColNew, out hv_BBoxPhiNew, out hv_BBoxLength1New, out hv_BBoxLength2New);
                        //
                        //smallest_rectangle2_xld might change the orientation of the bounding box.
                        //Hence, take the orientation that is closest to the one obtained out of the 4 corner points.
                        hv__.Dispose(); hv__.Dispose(); hv__.Dispose(); hv__.Dispose(); hv_BBoxPhiTmp.Dispose();
                        convert_rect2_8to5param(hv_BBoxRow1New, hv_BBoxCol1New, hv_BBoxRow2New,
                            hv_BBoxCol2New, hv_BBoxRow3New, hv_BBoxCol3New, hv_BBoxRow4New, hv_BBoxCol4New,
                            hv_IgnoreDirection, out hv__, out hv__, out hv__, out hv__, out hv_BBoxPhiTmp);
                        hv_PhiDelta.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_PhiDelta = ((hv_BBoxPhiTmp - hv_BBoxPhiNew)).TupleFmod(
                                (new HTuple(360)).TupleRad());
                        }
                        //Guarantee that angles are positive.
                        hv_PhiDeltaNegativeIndices.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_PhiDeltaNegativeIndices = ((hv_PhiDelta.TupleLessElem(
                                0.0))).TupleFind(1);
                        }
                        if ((int)(new HTuple(hv_PhiDeltaNegativeIndices.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_PhiDelta == null)
                                hv_PhiDelta = new HTuple();
                            hv_PhiDelta[hv_PhiDeltaNegativeIndices] = (hv_PhiDelta.TupleSelect(hv_PhiDeltaNegativeIndices)) + ((new HTuple(360)).TupleRad()
                                );
                        }
                        hv_IndicesRot90.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IndicesRot90 = (new HTuple(((hv_PhiDelta.TupleGreaterElem(
                                (new HTuple(45)).TupleRad()))).TupleAnd(hv_PhiDelta.TupleLessEqualElem(
                                (new HTuple(135)).TupleRad())))).TupleFind(1);
                        }
                        hv_IndicesRot180.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IndicesRot180 = (new HTuple(((hv_PhiDelta.TupleGreaterElem(
                                (new HTuple(135)).TupleRad()))).TupleAnd(hv_PhiDelta.TupleLessEqualElem(
                                (new HTuple(225)).TupleRad())))).TupleFind(1);
                        }
                        hv_IndicesRot270.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IndicesRot270 = (new HTuple(((hv_PhiDelta.TupleGreaterElem(
                                (new HTuple(225)).TupleRad()))).TupleAnd(hv_PhiDelta.TupleLessEqualElem(
                                (new HTuple(315)).TupleRad())))).TupleFind(1);
                        }
                        hv_SwapIndices.Dispose();
                        hv_SwapIndices = new HTuple();
                        if ((int)(new HTuple(hv_IndicesRot90.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_IndicesRot90] = (hv_BBoxPhiNew.TupleSelect(hv_IndicesRot90)) + ((new HTuple(90)).TupleRad()
                                );
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_SwapIndices = hv_SwapIndices.TupleConcat(
                                        hv_IndicesRot90);
                                    hv_SwapIndices.Dispose();
                                    hv_SwapIndices = ExpTmpLocalVar_SwapIndices;
                                }
                            }
                        }
                        if ((int)(new HTuple(hv_IndicesRot180.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_IndicesRot180] = (hv_BBoxPhiNew.TupleSelect(hv_IndicesRot180)) + ((new HTuple(180)).TupleRad()
                                );
                        }
                        if ((int)(new HTuple(hv_IndicesRot270.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_IndicesRot270] = (hv_BBoxPhiNew.TupleSelect(hv_IndicesRot270)) + ((new HTuple(270)).TupleRad()
                                );
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_SwapIndices = hv_SwapIndices.TupleConcat(
                                        hv_IndicesRot270);
                                    hv_SwapIndices.Dispose();
                                    hv_SwapIndices = ExpTmpLocalVar_SwapIndices;
                                }
                            }
                        }
                        if ((int)(new HTuple(hv_SwapIndices.TupleNotEqual(new HTuple()))) != 0)
                        {
                            hv_Tmp.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Tmp = hv_BBoxLength1New.TupleSelect(
                                    hv_SwapIndices);
                            }
                            if (hv_BBoxLength1New == null)
                                hv_BBoxLength1New = new HTuple();
                            hv_BBoxLength1New[hv_SwapIndices] = hv_BBoxLength2New.TupleSelect(hv_SwapIndices);
                            if (hv_BBoxLength2New == null)
                                hv_BBoxLength2New = new HTuple();
                            hv_BBoxLength2New[hv_SwapIndices] = hv_Tmp;
                        }
                        //Change angles such that they lie in the range (-180°, 180°].
                        hv_BBoxPhiNewIndices.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxPhiNewIndices = ((hv_BBoxPhiNew.TupleGreaterElem(
                                (new HTuple(180)).TupleRad()))).TupleFind(1);
                        }
                        if ((int)(new HTuple(hv_BBoxPhiNewIndices.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_BBoxPhiNewIndices] = (hv_BBoxPhiNew.TupleSelect(hv_BBoxPhiNewIndices)) - ((new HTuple(360)).TupleRad()
                                );
                        }
                    }
                    else
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxColNew = hv_BBoxColNew * hv_FactorResampleWidth;
                                hv_BBoxColNew.Dispose();
                                hv_BBoxColNew = ExpTmpLocalVar_BBoxColNew;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxRowNew = hv_BBoxRowNew * hv_FactorResampleWidth;
                                hv_BBoxRowNew.Dispose();
                                hv_BBoxRowNew = ExpTmpLocalVar_BBoxRowNew;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxLength1New = hv_BBoxLength1New * hv_FactorResampleWidth;
                                hv_BBoxLength1New.Dispose();
                                hv_BBoxLength1New = ExpTmpLocalVar_BBoxLength1New;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxLength2New = hv_BBoxLength2New * hv_FactorResampleWidth;
                                hv_BBoxLength2New.Dispose();
                                hv_BBoxLength2New = ExpTmpLocalVar_BBoxLength2New;
                            }
                        }
                        //Phi stays the same.
                    }
                    //
                }
                //Check that there are no invalid boxes.
                if ((int)(new HTuple((new HTuple(hv_BBoxRowNew.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    hv_BoxesInvalid.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BoxesInvalid = (((hv_BBoxLength1New.TupleEqualElem(
                            0))).TupleSum()) + (((hv_BBoxLength2New.TupleEqualElem(0))).TupleSum());
                    }
                    if ((int)(new HTuple(hv_BoxesInvalid.TupleGreater(0))) != 0)
                    {
                        hv_ImageId.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                        throw new HalconException(("An error has occurred during preprocessing image_id " + hv_ImageId) + new HTuple(": Sample contains at least one box with zero-area, i.e. bbox_length1 == 0 or bbox_length2 == 0!"));
                    }
                }
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_row", hv_BBoxRowNew);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_col", hv_BBoxColNew);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_length1", hv_BBoxLength1New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_length2", hv_BBoxLength2New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_phi", hv_BBoxPhiNew);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_label_id", hv_BBoxLabelNew);
                //
                ho_DomainRaw.Dispose();
                ho_Rectangle2XLD.Dispose();
                ho_Rectangle2XLDSheared.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_KeyExists.Dispose();
                hv_BBoxRow.Dispose();
                hv_BBoxCol.Dispose();
                hv_BBoxLength1.Dispose();
                hv_BBoxLength2.Dispose();
                hv_BBoxPhi.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_RowDomain1.Dispose();
                hv_ColumnDomain1.Dispose();
                hv_RowDomain2.Dispose();
                hv_ColumnDomain2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxRowNew.Dispose();
                hv_BBoxColNew.Dispose();
                hv_BBoxLength1New.Dispose();
                hv_BBoxLength2New.Dispose();
                hv_BBoxPhiNew.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_ClassIDsNoOrientationIndices.Dispose();
                hv_Index.Dispose();
                hv_ClassIDsNoOrientationIndicesTmp.Dispose();
                hv_DirectionLength1Row.Dispose();
                hv_DirectionLength1Col.Dispose();
                hv_DirectionLength2Row.Dispose();
                hv_DirectionLength2Col.Dispose();
                hv_Corner1Row.Dispose();
                hv_Corner1Col.Dispose();
                hv_Corner2Row.Dispose();
                hv_Corner2Col.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxCol3.Dispose();
                hv_BBoxCol3New.Dispose();
                hv_BBoxCol4.Dispose();
                hv_BBoxCol4New.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxRow3.Dispose();
                hv_BBoxRow3New.Dispose();
                hv_BBoxRow4.Dispose();
                hv_BBoxRow4New.Dispose();
                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DScale.Dispose();
                hv_BBoxPhiTmp.Dispose();
                hv_PhiDelta.Dispose();
                hv_PhiDeltaNegativeIndices.Dispose();
                hv_IndicesRot90.Dispose();
                hv_IndicesRot180.Dispose();
                hv_IndicesRot270.Dispose();
                hv_SwapIndices.Dispose();
                hv_Tmp.Dispose();
                hv_BBoxPhiNewIndices.Dispose();
                hv__.Dispose();

                return;

            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_DomainRaw.Dispose();
                ho_Rectangle2XLD.Dispose();
                ho_Rectangle2XLDSheared.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_KeyExists.Dispose();
                hv_BBoxRow.Dispose();
                hv_BBoxCol.Dispose();
                hv_BBoxLength1.Dispose();
                hv_BBoxLength2.Dispose();
                hv_BBoxPhi.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_RowDomain1.Dispose();
                hv_ColumnDomain1.Dispose();
                hv_RowDomain2.Dispose();
                hv_ColumnDomain2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxRowNew.Dispose();
                hv_BBoxColNew.Dispose();
                hv_BBoxLength1New.Dispose();
                hv_BBoxLength2New.Dispose();
                hv_BBoxPhiNew.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_ClassIDsNoOrientationIndices.Dispose();
                hv_Index.Dispose();
                hv_ClassIDsNoOrientationIndicesTmp.Dispose();
                hv_DirectionLength1Row.Dispose();
                hv_DirectionLength1Col.Dispose();
                hv_DirectionLength2Row.Dispose();
                hv_DirectionLength2Col.Dispose();
                hv_Corner1Row.Dispose();
                hv_Corner1Col.Dispose();
                hv_Corner2Row.Dispose();
                hv_Corner2Col.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxCol3.Dispose();
                hv_BBoxCol3New.Dispose();
                hv_BBoxCol4.Dispose();
                hv_BBoxCol4New.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxRow3.Dispose();
                hv_BBoxRow3New.Dispose();
                hv_BBoxRow4.Dispose();
                hv_BBoxRow4New.Dispose();
                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DScale.Dispose();
                hv_BBoxPhiTmp.Dispose();
                hv_PhiDelta.Dispose();
                hv_PhiDeltaNegativeIndices.Dispose();
                hv_IndicesRot90.Dispose();
                hv_IndicesRot180.Dispose();
                hv_IndicesRot270.Dispose();
                hv_SwapIndices.Dispose();
                hv_Tmp.Dispose();
                hv_BBoxPhiNewIndices.Dispose();
                hv__.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess images for deep-learning-based training and inference. 
        public void preprocess_dl_model_images(HObject ho_Images, out HObject ho_ImagesPreprocessed,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImagesNew = null, ho_ImageSelected = null;
            HObject ho_ImagesScaled = null, ho_Channel = null, ho_ImagesScaledMultiChannel = null;
            HObject ho_OutputNormImages = null, ho_Image = null, ho_ImageNormChannels = null;
            HObject ho_ImageNormChannel = null, ho_ObjectSelected = null;
            HObject ho_ThreeChannelImage = null, ho_SingleChannelImage = null;

            // Local copy input parameter variables 
            HObject ho_Images_COPY_INP_TMP;
            ho_Images_COPY_INP_TMP = new HObject(ho_Images);



            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_ImageRangeMin = new HTuple();
            HTuple hv_ImageRangeMax = new HTuple(), hv_DomainHandling = new HTuple();
            HTuple hv_NormalizationType = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_ImageWidthInput = new HTuple(), hv_ImageHeightInput = new HTuple();
            HTuple hv_EqualWidth = new HTuple(), hv_EqualHeight = new HTuple();
            HTuple hv_Type = new HTuple(), hv_NumMatches = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_ImageIndex = new HTuple();
            HTuple hv_Channels = new HTuple(), hv_ChannelIndex = new HTuple();
            HTuple hv_MinChannel = new HTuple(), hv_MaxChannel = new HTuple();
            HTuple hv_Range = new HTuple(), hv_Scale = new HTuple();
            HTuple hv_Shift = new HTuple(), hv_Min = new HTuple();
            HTuple hv_Max = new HTuple(), hv_UsePredefinedNormalizationValues = new HTuple();
            HTuple hv_MeanValues = new HTuple(), hv_DeviationValues = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_NumChannels = new HTuple();
            HTuple hv_IndexImage = new HTuple(), hv_IndexChannel = new HTuple();
            HTuple hv_EqualByte = new HTuple(), hv_RescaleRange = new HTuple();
            HTuple hv_NumChannelsAllImages = new HTuple(), hv_ImageNumChannelsTuple = new HTuple();
            HTuple hv_IndicesWrongChannels = new HTuple(), hv_IndexWrongImages = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImagesPreprocessed);
            HOperatorSet.GenEmptyObj(out ho_ImagesNew);
            HOperatorSet.GenEmptyObj(out ho_ImageSelected);
            HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
            HOperatorSet.GenEmptyObj(out ho_Channel);
            HOperatorSet.GenEmptyObj(out ho_ImagesScaledMultiChannel);
            HOperatorSet.GenEmptyObj(out ho_OutputNormImages);
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageNormChannels);
            HOperatorSet.GenEmptyObj(out ho_ImageNormChannel);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_ThreeChannelImage);
            HOperatorSet.GenEmptyObj(out ho_SingleChannelImage);
            try
            {
                //
                //This procedure preprocesses the provided Images
                //according to the parameters in the dictionary DLPreprocessParam.
                //Note that depending on the images,
                //additional preprocessing steps might be beneficial.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageNumChannels.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_num_channels", out hv_ImageNumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                hv_NormalizationType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "normalization_type", out hv_NormalizationType);
                hv_ModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_ModelType);
                //
                //Preprocess the images.
                //
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.CropDomain(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)((new HTuple(hv_DomainHandling.TupleEqual("keep_domain"))).TupleAnd(
                    new HTuple(hv_ModelType.TupleEqual("anomaly_detection")))) != 0)
                {
                    //Anomaly detection models accept the additional option 'keep_domain'.
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Zoom images only if they have a different size than the specified size.
                hv_ImageWidthInput.Dispose(); hv_ImageHeightInput.Dispose();
                HOperatorSet.GetImageSize(ho_Images_COPY_INP_TMP, out hv_ImageWidthInput, out hv_ImageHeightInput);
                hv_EqualWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualWidth = hv_ImageWidth.TupleEqualElem(
                        hv_ImageWidthInput);
                }
                hv_EqualHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualHeight = hv_ImageHeight.TupleEqualElem(
                        hv_ImageHeightInput);
                }
                if ((int)((new HTuple(((hv_EqualWidth.TupleMin())).TupleEqual(0))).TupleOr(
                    new HTuple(((hv_EqualHeight.TupleMin())).TupleEqual(0)))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0, hv_ImageWidth,
                            hv_ImageHeight, "constant");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                if ((int)(new HTuple(hv_NormalizationType.TupleEqual("all_channels"))) != 0)
                {
                    //Check the type of the input images.
                    //Channel normalization works here only for byte, integer and real images.
                    hv_Type.Dispose();
                    HOperatorSet.GetImageType(ho_Images_COPY_INP_TMP, out hv_Type);
                    hv_NumMatches.Dispose();
                    HOperatorSet.TupleRegexpTest(hv_Type, "byte|int|real", out hv_NumMatches);
                    hv_NumImages.Dispose();
                    HOperatorSet.CountObj(ho_Images_COPY_INP_TMP, out hv_NumImages);
                    if ((int)(new HTuple(hv_NumMatches.TupleNotEqual(hv_NumImages))) != 0)
                    {
                        throw new HalconException(new HTuple("In case of normalization type 'all_channels', please provide only images of type 'byte', 'int1', 'int2', 'uint2', 'int4', 'int8', or 'real'."));
                    }
                    //
                    //Perform all channels normalization.
                    if ((int)(new HTuple(hv_Type.TupleEqual("byte"))) != 0)
                    {
                        //Scale the gray values to [0-255].
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ScaleImageMax(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                            ho_Images_COPY_INP_TMP.Dispose();
                            ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                    else
                    {
                        //Scale the gray values to [ImageRangeMin-ImageRangeMax].
                        //Scaling is performed for each image separately.
                        ho_ImagesNew.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_ImagesNew);
                        HTuple end_val56 = hv_NumImages;
                        HTuple step_val56 = 1;
                        for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val56, step_val56); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val56))
                        {
                            ho_ImageSelected.Dispose();
                            HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected,
                                hv_ImageIndex);
                            hv_Channels.Dispose();
                            HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_Channels);
                            //
                            //Calculate the channel-wise minimum and maximum grey values.
                            ho_ImagesScaled.Dispose();
                            HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
                            HTuple end_val62 = hv_Channels;
                            HTuple step_val62 = 1;
                            for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val62, step_val62); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val62))
                            {
                                ho_Channel.Dispose();
                                HOperatorSet.AccessChannel(ho_ImageSelected, out ho_Channel, hv_ChannelIndex);
                                hv_MinChannel.Dispose(); hv_MaxChannel.Dispose(); hv_Range.Dispose();
                                HOperatorSet.MinMaxGray(ho_Channel, ho_Channel, 0, out hv_MinChannel,
                                    out hv_MaxChannel, out hv_Range);
                                //Scale and shift the channel.
                                hv_Scale.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Scale = (hv_ImageRangeMax - hv_ImageRangeMin) / (hv_MaxChannel - hv_MinChannel);
                                }
                                hv_Shift.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Shift = ((-hv_Scale) * hv_MinChannel) + hv_ImageRangeMin;
                                }
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ScaleImage(ho_Channel, out ExpTmpOutVar_0, hv_Scale, hv_Shift);
                                    ho_Channel.Dispose();
                                    ho_Channel = ExpTmpOutVar_0;
                                }
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho_ImagesScaled, ho_Channel, out ExpTmpOutVar_0
                                        );
                                    ho_ImagesScaled.Dispose();
                                    ho_ImagesScaled = ExpTmpOutVar_0;
                                }
                            }
                            ho_ImagesScaledMultiChannel.Dispose();
                            HOperatorSet.ChannelsToImage(ho_ImagesScaled, out ho_ImagesScaledMultiChannel
                                );
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_ImagesNew, ho_ImagesScaledMultiChannel, out ExpTmpOutVar_0
                                    );
                                ho_ImagesNew.Dispose();
                                ho_ImagesNew = ExpTmpOutVar_0;
                            }
                        }
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = new HObject(ho_ImagesNew);
                        //Integer image convert to real image
                        if ((int)(new HTuple(hv_Type.TupleNotEqual("real"))) != 0)
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConvertImageType(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                                    "real");
                                ho_Images_COPY_INP_TMP.Dispose();
                                ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                            }
                        }
                    }
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("first_channel"))) != 0)
                {
                    //Check the type of the input images.
                    //First channel normalization works here only for byte, integer and real images.
                    hv_Type.Dispose();
                    HOperatorSet.GetImageType(ho_Images_COPY_INP_TMP, out hv_Type);
                    hv_NumMatches.Dispose();
                    HOperatorSet.TupleRegexpTest(hv_Type, "byte|int|real", out hv_NumMatches);
                    hv_NumImages.Dispose();
                    HOperatorSet.CountObj(ho_Images_COPY_INP_TMP, out hv_NumImages);
                    if ((int)(new HTuple(hv_NumMatches.TupleNotEqual(hv_NumImages))) != 0)
                    {
                        throw new HalconException(new HTuple("In case of normalization type 'first_channel', please provide only images of type 'byte', 'int1', 'int2', 'uint2', 'int4', 'int8', or 'real'."));
                    }
                    //
                    //Perform first channel normalization.
                    if ((int)(new HTuple(hv_Type.TupleEqual("byte"))) != 0)
                    {
                        //Scale the gray values to [0-255].
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ScaleImageMax(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                            ho_Images_COPY_INP_TMP.Dispose();
                            ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                    else
                    {
                        //Scale the gray values to [ImageRangeMin-ImageRangeMax].
                        //Scaling is performed for each image separately.
                        ho_ImagesNew.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_ImagesNew);
                        HTuple end_val98 = hv_NumImages;
                        HTuple step_val98 = 1;
                        for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val98, step_val98); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val98))
                        {
                            ho_ImageSelected.Dispose();
                            HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected,
                                hv_ImageIndex);
                            hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                            HOperatorSet.MinMaxGray(ho_ImageSelected, ho_ImageSelected, 0, out hv_Min,
                                out hv_Max, out hv_Range);
                            hv_Scale.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Scale = (hv_ImageRangeMax - hv_ImageRangeMin) / (hv_Max - hv_Min);
                            }
                            hv_Shift.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Shift = ((-hv_Scale) * hv_Min) + hv_ImageRangeMin;
                            }
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ScaleImage(ho_ImageSelected, out ExpTmpOutVar_0, hv_Scale,
                                    hv_Shift);
                                ho_ImageSelected.Dispose();
                                ho_ImageSelected = ExpTmpOutVar_0;
                            }
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_ImagesNew, ho_ImageSelected, out ExpTmpOutVar_0
                                    );
                                ho_ImagesNew.Dispose();
                                ho_ImagesNew = ExpTmpOutVar_0;
                            }
                        }
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = new HObject(ho_ImagesNew);
                        //Integer image convert to real image
                        if ((int)(new HTuple(hv_Type.TupleNotEqual("real"))) != 0)
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConvertImageType(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                                    "real");
                                ho_Images_COPY_INP_TMP.Dispose();
                                ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                            }
                        }
                    }
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("constant_values"))) != 0)
                {
                    //
                    //Constant values normalization works here only for byte, integer and real images.
                    hv_Type.Dispose();
                    HOperatorSet.GetImageType(ho_Images_COPY_INP_TMP, out hv_Type);
                    hv_NumMatches.Dispose();
                    HOperatorSet.TupleRegexpTest(hv_Type, "byte|int|real", out hv_NumMatches);
                    hv_NumImages.Dispose();
                    HOperatorSet.CountObj(ho_Images_COPY_INP_TMP, out hv_NumImages);
                    if ((int)(new HTuple(hv_NumMatches.TupleNotEqual(hv_NumImages))) != 0)
                    {
                        throw new HalconException(new HTuple("In case of normalization type 'constant_values', please provide only images of type 'byte', 'int1', 'int2', 'uint2', 'int4', 'int8', or 'real'."));
                    }
                    //For a correct normalization we have to use real images.
                    if ((int)(new HTuple(hv_Type.TupleNotEqual("real"))) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConvertImageType(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                                "real");
                            ho_Images_COPY_INP_TMP.Dispose();
                            ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                    //
                    //Get the normalization values set by create_dl_preprocess_param or
                    //use predefined values.
                    hv_UsePredefinedNormalizationValues.Dispose();
                    hv_UsePredefinedNormalizationValues = "false";
                    try
                    {
                        hv_MeanValues.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "mean_values_normalization",
                            out hv_MeanValues);
                        hv_DeviationValues.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "deviation_values_normalization",
                            out hv_DeviationValues);
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        hv_UsePredefinedNormalizationValues.Dispose();
                        hv_UsePredefinedNormalizationValues = "true";
                    }
                    //
                    hv_NumChannels.Dispose();
                    HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_NumChannels);
                    if ((int)(new HTuple(hv_UsePredefinedNormalizationValues.TupleEqual("true"))) != 0)
                    {
                        //This type of normalization works for one-channel images by composing them to three-channel images.
                        if ((int)((new HTuple(((hv_NumChannels.TupleMin())).TupleEqual(1))).TupleAnd(
                            new HTuple(((hv_NumChannels.TupleMax())).TupleEqual(1)))) != 0)
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Compose3(ho_Images_COPY_INP_TMP, ho_Images_COPY_INP_TMP,
                                    ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                                ho_Images_COPY_INP_TMP.Dispose();
                                ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                            }
                            hv_NumChannels.Dispose();
                            HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_NumChannels);
                        }
                        //Use predefined values for normalization.
                        if ((int)((new HTuple(((hv_NumChannels.TupleMin())).TupleNotEqual(3))).TupleOr(
                            new HTuple(((hv_NumChannels.TupleMax())).TupleNotEqual(3)))) != 0)
                        {
                            throw new HalconException("Using predefined values for normalization type 'constant_values' is allowed only for one- and three-channel images.");
                        }
                        //Predefined values.
                        hv_MeanValues.Dispose();
                        hv_MeanValues = new HTuple();
                        hv_MeanValues[0] = 123.675;
                        hv_MeanValues[1] = 116.28;
                        hv_MeanValues[2] = 103.53;
                        hv_DeviationValues.Dispose();
                        hv_DeviationValues = new HTuple();
                        hv_DeviationValues[0] = 58.395;
                        hv_DeviationValues[1] = 57.12;
                        hv_DeviationValues[2] = 57.375;
                    }
                    else
                    {
                        //Use user-defined values.
                        if ((int)((new HTuple((new HTuple(hv_MeanValues.TupleLength())).TupleNotEqual(
                            hv_NumChannels))).TupleOr(new HTuple((new HTuple(hv_DeviationValues.TupleLength()
                            )).TupleNotEqual(hv_NumChannels)))) != 0)
                        {
                            throw new HalconException("The length for mean and deviation values for normalization type 'constant_values' have to be the same size as the number of channels of the image");
                        }
                    }
                    //
                    ho_OutputNormImages.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_OutputNormImages);
                    hv_NumImages.Dispose();
                    HOperatorSet.CountObj(ho_Images_COPY_INP_TMP, out hv_NumImages);
                    HTuple end_val159 = hv_NumImages;
                    HTuple step_val159 = 1;
                    for (hv_IndexImage = 1; hv_IndexImage.Continue(end_val159, step_val159); hv_IndexImage = hv_IndexImage.TupleAdd(step_val159))
                    {
                        ho_Image.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_Image, hv_IndexImage);
                        ho_ImageNormChannels.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_ImageNormChannels);
                        HTuple end_val162 = hv_NumChannels;
                        HTuple step_val162 = 1;
                        for (hv_IndexChannel = 1; hv_IndexChannel.Continue(end_val162, step_val162); hv_IndexChannel = hv_IndexChannel.TupleAdd(step_val162))
                        {
                            ho_Channel.Dispose();
                            HOperatorSet.AccessChannel(ho_Image, out ho_Channel, hv_IndexChannel);
                            hv_ImageWidth.Dispose(); hv_ImageHeight.Dispose();
                            HOperatorSet.GetImageSize(ho_Channel, out hv_ImageWidth, out hv_ImageHeight);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_ImageNormChannel.Dispose();
                                HOperatorSet.ScaleImage(ho_Channel, out ho_ImageNormChannel, 1 / (((hv_DeviationValues.TupleSelect(
                                    hv_IndexChannel - 1))).TupleReal()), (-(((hv_MeanValues.TupleSelect(
                                    hv_IndexChannel - 1))).TupleReal())) / (hv_DeviationValues.TupleSelect(
                                    hv_IndexChannel - 1)));
                            }
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.AppendChannel(ho_ImageNormChannels, ho_ImageNormChannel,
                                    out ExpTmpOutVar_0);
                                ho_ImageNormChannels.Dispose();
                                ho_ImageNormChannels = ExpTmpOutVar_0;
                            }
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_OutputNormImages, ho_ImageNormChannels, out ExpTmpOutVar_0
                                );
                            ho_OutputNormImages.Dispose();
                            ho_OutputNormImages = ExpTmpOutVar_0;
                        }
                    }
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_Images_COPY_INP_TMP = new HObject(ho_OutputNormImages);
                }
                else if ((int)((new HTuple((new HTuple((new HTuple(hv_NormalizationType.TupleNotEqual(
                    "all_channels"))).TupleAnd(new HTuple(hv_NormalizationType.TupleNotEqual(
                    "first_channel"))))).TupleAnd(new HTuple(hv_NormalizationType.TupleNotEqual(
                    "constant_values"))))).TupleAnd(new HTuple(hv_NormalizationType.TupleNotEqual(
                    "none")))) != 0)
                {
                    throw new HalconException("Unsupported parameter value for 'normalization_type'");
                }
                //
                //Check the type of the input images.
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_Images_COPY_INP_TMP, out hv_Type);
                hv_NumMatches.Dispose();
                HOperatorSet.TupleRegexpTest(hv_Type, "byte|real", out hv_NumMatches);
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_Images_COPY_INP_TMP, out hv_NumImages);
                if ((int)(new HTuple(hv_NumMatches.TupleNotEqual(hv_NumImages))) != 0)
                {
                    throw new HalconException("Please provide only images of type 'byte' or 'real'.");
                }
                //If the type is 'byte', convert it to 'real' and scale it.
                //The gray value scaling does not work on 'byte' images.
                //For 'real' images it is assumed that the range is already correct.
                hv_EqualByte.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualByte = hv_Type.TupleEqualElem(
                        "byte");
                }
                if ((int)(new HTuple(((hv_EqualByte.TupleMax())).TupleEqual(1))) != 0)
                {
                    if ((int)(new HTuple(((hv_EqualByte.TupleMin())).TupleEqual(0))) != 0)
                    {
                        throw new HalconException("Passing mixed type images is not supported.");
                    }
                    //Convert the image type from 'byte' to 'real',
                    //because the model expects 'real' images.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    //Scale/Shift the gray values from [0-255] to the expected range.
                    hv_RescaleRange.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RescaleRange = (hv_ImageRangeMax - hv_ImageRangeMin) / 255.0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ScaleImage(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0, hv_RescaleRange,
                            hv_ImageRangeMin);
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the number of channels.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_Images_COPY_INP_TMP, out hv_NumImages);
                //Check all images for number of channels.
                hv_NumChannelsAllImages.Dispose();
                HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_NumChannelsAllImages);
                hv_ImageNumChannelsTuple.Dispose();
                HOperatorSet.TupleGenConst(hv_NumImages, hv_ImageNumChannels, out hv_ImageNumChannelsTuple);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_IndicesWrongChannels.Dispose();
                    HOperatorSet.TupleFind(hv_NumChannelsAllImages.TupleNotEqualElem(hv_ImageNumChannelsTuple),
                        1, out hv_IndicesWrongChannels);
                }
                //
                //Correct images with a wrong number of channels.
                //
                if ((int)(new HTuple(hv_IndicesWrongChannels.TupleNotEqual(-1))) != 0)
                {
                    //
                    for (hv_IndexWrongImages = 0; (int)hv_IndexWrongImages <= (int)((new HTuple(hv_IndicesWrongChannels.TupleLength()
                        )) - 1); hv_IndexWrongImages = (int)hv_IndexWrongImages + 1)
                    {
                        //Get the index, the number of channels and the image
                        //for each image with wrong number of channels.
                        hv_ImageIndex.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ImageIndex = (hv_IndicesWrongChannels.TupleSelect(
                                hv_IndexWrongImages)) + 1;
                        }
                        hv_NumChannels.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_NumChannels = hv_NumChannelsAllImages.TupleSelect(
                                hv_ImageIndex - 1);
                        }
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ObjectSelected, hv_ImageIndex);
                        //
                        if ((int)((new HTuple(hv_NumChannels.TupleEqual(1))).TupleAnd(new HTuple(hv_ImageNumChannels.TupleEqual(
                            3)))) != 0)
                        {
                            //If the image is a grayscale image, but the model expects a color image:
                            //convert it to an image with three channels.
                            ho_ThreeChannelImage.Dispose();
                            HOperatorSet.Compose3(ho_ObjectSelected, ho_ObjectSelected, ho_ObjectSelected,
                                out ho_ThreeChannelImage);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_ThreeChannelImage,
                                    out ExpTmpOutVar_0, hv_ImageIndex);
                                ho_Images_COPY_INP_TMP.Dispose();
                                ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                            }
                        }
                        else if ((int)((new HTuple(hv_NumChannels.TupleEqual(3))).TupleAnd(
                            new HTuple(hv_ImageNumChannels.TupleEqual(1)))) != 0)
                        {
                            //If the image is a color image, but the model expects a grayscale image:
                            //convert it to an image with only one channel.
                            ho_SingleChannelImage.Dispose();
                            HOperatorSet.Rgb1ToGray(ho_ObjectSelected, out ho_SingleChannelImage);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_SingleChannelImage,
                                    out ExpTmpOutVar_0, hv_ImageIndex);
                                ho_Images_COPY_INP_TMP.Dispose();
                                ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                            }
                        }
                        else
                        {
                            throw new HalconException("Number of channels is not supported. Please check for images with a number of channels different to 1 and 3 and perform their preprocessing yourself.");
                        }
                        //
                    }
                }
                //
                //Write preprocessed image to output variable.
                ho_ImagesPreprocessed.Dispose();
                ho_ImagesPreprocessed = new HObject(ho_Images_COPY_INP_TMP);
                //
                ho_Images_COPY_INP_TMP.Dispose();
                ho_ImagesNew.Dispose();
                ho_ImageSelected.Dispose();
                ho_ImagesScaled.Dispose();
                ho_Channel.Dispose();
                ho_ImagesScaledMultiChannel.Dispose();
                ho_OutputNormImages.Dispose();
                ho_Image.Dispose();
                ho_ImageNormChannels.Dispose();
                ho_ImageNormChannel.Dispose();
                ho_ObjectSelected.Dispose();
                ho_ThreeChannelImage.Dispose();
                ho_SingleChannelImage.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_ImageWidthInput.Dispose();
                hv_ImageHeightInput.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_NumImages.Dispose();
                hv_ImageIndex.Dispose();
                hv_Channels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_MinChannel.Dispose();
                hv_MaxChannel.Dispose();
                hv_Range.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_UsePredefinedNormalizationValues.Dispose();
                hv_MeanValues.Dispose();
                hv_DeviationValues.Dispose();
                hv_Exception.Dispose();
                hv_NumChannels.Dispose();
                hv_IndexImage.Dispose();
                hv_IndexChannel.Dispose();
                hv_EqualByte.Dispose();
                hv_RescaleRange.Dispose();
                hv_NumChannelsAllImages.Dispose();
                hv_ImageNumChannelsTuple.Dispose();
                hv_IndicesWrongChannels.Dispose();
                hv_IndexWrongImages.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Images_COPY_INP_TMP.Dispose();
                ho_ImagesNew.Dispose();
                ho_ImageSelected.Dispose();
                ho_ImagesScaled.Dispose();
                ho_Channel.Dispose();
                ho_ImagesScaledMultiChannel.Dispose();
                ho_OutputNormImages.Dispose();
                ho_Image.Dispose();
                ho_ImageNormChannels.Dispose();
                ho_ImageNormChannel.Dispose();
                ho_ObjectSelected.Dispose();
                ho_ThreeChannelImage.Dispose();
                ho_SingleChannelImage.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_ImageWidthInput.Dispose();
                hv_ImageHeightInput.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_NumImages.Dispose();
                hv_ImageIndex.Dispose();
                hv_Channels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_MinChannel.Dispose();
                hv_MaxChannel.Dispose();
                hv_Range.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_UsePredefinedNormalizationValues.Dispose();
                hv_MeanValues.Dispose();
                hv_DeviationValues.Dispose();
                hv_Exception.Dispose();
                hv_NumChannels.Dispose();
                hv_IndexImage.Dispose();
                hv_IndexChannel.Dispose();
                hv_EqualByte.Dispose();
                hv_RescaleRange.Dispose();
                hv_NumChannelsAllImages.Dispose();
                hv_ImageNumChannelsTuple.Dispose();
                hv_IndicesWrongChannels.Dispose();
                hv_IndexWrongImages.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Semantic Segmentation
        // Short Description: Preprocess segmentation and weight images for deep-learning-based segmentation training and inference. 
        public void preprocess_dl_model_segmentations(HObject ho_ImagesRaw, HObject ho_Segmentations,
            out HObject ho_SegmentationsPreprocessed, HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Domain = null, ho_SelectedSeg = null;
            HObject ho_SelectedDomain = null;

            // Local copy input parameter variables 
            HObject ho_Segmentations_COPY_INP_TMP;
            ho_Segmentations_COPY_INP_TMP = new HObject(ho_Segmentations);



            // Local control variables 

            HTuple hv_NumberImages = new HTuple(), hv_NumberSegmentations = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_WidthSeg = new HTuple(), hv_HeightSeg = new HTuple();
            HTuple hv_DLModelType = new HTuple(), hv_ImageWidth = new HTuple();
            HTuple hv_ImageHeight = new HTuple(), hv_ImageNumChannels = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_SetBackgroundID = new HTuple();
            HTuple hv_ClassesToBackground = new HTuple(), hv_IgnoreClassIDs = new HTuple();
            HTuple hv_IsInt = new HTuple(), hv_IndexImage = new HTuple();
            HTuple hv_ImageWidthRaw = new HTuple(), hv_ImageHeightRaw = new HTuple();
            HTuple hv_EqualWidth = new HTuple(), hv_EqualHeight = new HTuple();
            HTuple hv_Type = new HTuple(), hv_EqualReal = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SegmentationsPreprocessed);
            HOperatorSet.GenEmptyObj(out ho_Domain);
            HOperatorSet.GenEmptyObj(out ho_SelectedSeg);
            HOperatorSet.GenEmptyObj(out ho_SelectedDomain);
            try
            {
                //
                //This procedure preprocesses the segmentation or weight images
                //given by Segmentations so that they can be handled by
                //train_dl_model_batch and apply_dl_model.
                //
                //Check input data.
                //Examine number of images.
                hv_NumberImages.Dispose();
                HOperatorSet.CountObj(ho_ImagesRaw, out hv_NumberImages);
                hv_NumberSegmentations.Dispose();
                HOperatorSet.CountObj(ho_Segmentations_COPY_INP_TMP, out hv_NumberSegmentations);
                if ((int)(new HTuple(hv_NumberImages.TupleNotEqual(hv_NumberSegmentations))) != 0)
                {
                    throw new HalconException("Equal number of images given in ImagesRaw and Segmentations required");
                }
                //Size of images.
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_ImagesRaw, out hv_Width, out hv_Height);
                hv_WidthSeg.Dispose(); hv_HeightSeg.Dispose();
                HOperatorSet.GetImageSize(ho_Segmentations_COPY_INP_TMP, out hv_WidthSeg, out hv_HeightSeg);
                if ((int)((new HTuple(hv_Width.TupleNotEqual(hv_WidthSeg))).TupleOr(new HTuple(hv_Height.TupleNotEqual(
                    hv_HeightSeg)))) != 0)
                {
                    throw new HalconException("Equal size of the images given in ImagesRaw and Segmentations required.");
                }
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the relevant preprocessing parameters.
                hv_DLModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_DLModelType);
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageNumChannels.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_num_channels", out hv_ImageNumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                //Segmentation specific parameters.
                hv_SetBackgroundID.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "set_background_id", out hv_SetBackgroundID);
                hv_ClassesToBackground.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "class_ids_background", out hv_ClassesToBackground);
                hv_IgnoreClassIDs.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", out hv_IgnoreClassIDs);
                //
                //Check the input parameter for setting the background ID.
                if ((int)(new HTuple(hv_SetBackgroundID.TupleNotEqual(new HTuple()))) != 0)
                {
                    //Check that the model is a segmentation model.
                    if ((int)(new HTuple(hv_DLModelType.TupleNotEqual("segmentation"))) != 0)
                    {
                        throw new HalconException("Setting class IDs to background is only implemented for segmentation.");
                    }
                    //Check the background ID.
                    hv_IsInt.Dispose();
                    HOperatorSet.TupleIsIntElem(hv_SetBackgroundID, out hv_IsInt);
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleNotEqual(
                        1))) != 0)
                    {
                        throw new HalconException("Only one class_id as 'set_background_id' allowed.");
                    }
                    else if ((int)(hv_IsInt.TupleNot()) != 0)
                    {
                        //Given class_id has to be of type int.
                        throw new HalconException("The class_id given as 'set_background_id' has to be of type int.");
                    }
                    //Check the values of ClassesToBackground.
                    if ((int)(new HTuple((new HTuple(hv_ClassesToBackground.TupleLength())).TupleEqual(
                        0))) != 0)
                    {
                        //Check that the given classes are of length > 0.
                        throw new HalconException(new HTuple("If 'set_background_id' is given, 'class_ids_background' must at least contain this class ID."));
                    }
                    else if ((int)(new HTuple(((hv_ClassesToBackground.TupleIntersection(
                        hv_IgnoreClassIDs))).TupleNotEqual(new HTuple()))) != 0)
                    {
                        //Check that class_ids_background is not included in the ignore_class_ids of the DLModel.
                        throw new HalconException("The given 'class_ids_background' must not be included in the 'ignore_class_ids' of the model.");
                    }
                }
                //
                //Domain handling of the image to be preprocessed.
                //
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    //If the domain should be cropped the domain has to be transferred
                    //from the raw image to the segmentation image.
                    ho_Domain.Dispose();
                    HOperatorSet.GetDomain(ho_ImagesRaw, out ho_Domain);
                    HTuple end_val66 = hv_NumberImages;
                    HTuple step_val66 = 1;
                    for (hv_IndexImage = 1; hv_IndexImage.Continue(end_val66, step_val66); hv_IndexImage = hv_IndexImage.TupleAdd(step_val66))
                    {
                        ho_SelectedSeg.Dispose();
                        HOperatorSet.SelectObj(ho_Segmentations_COPY_INP_TMP, out ho_SelectedSeg,
                            hv_IndexImage);
                        ho_SelectedDomain.Dispose();
                        HOperatorSet.SelectObj(ho_Domain, out ho_SelectedDomain, hv_IndexImage);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ChangeDomain(ho_SelectedSeg, ho_SelectedDomain, out ExpTmpOutVar_0
                                );
                            ho_SelectedSeg.Dispose();
                            ho_SelectedSeg = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReplaceObj(ho_Segmentations_COPY_INP_TMP, ho_SelectedSeg,
                                out ExpTmpOutVar_0, hv_IndexImage);
                            ho_Segmentations_COPY_INP_TMP.Dispose();
                            ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.CropDomain(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Preprocess the segmentation images.
                //
                //Set all background classes to the given background class ID.
                if ((int)(new HTuple(hv_SetBackgroundID.TupleNotEqual(new HTuple()))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        reassign_pixel_values(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0,
                            hv_ClassesToBackground, hv_SetBackgroundID);
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Zoom images only if they have a different size than the specified size.
                hv_ImageWidthRaw.Dispose(); hv_ImageHeightRaw.Dispose();
                HOperatorSet.GetImageSize(ho_Segmentations_COPY_INP_TMP, out hv_ImageWidthRaw,
                    out hv_ImageHeightRaw);
                hv_EqualWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualWidth = hv_ImageWidth.TupleEqualElem(
                        hv_ImageWidthRaw);
                }
                hv_EqualHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualHeight = hv_ImageHeight.TupleEqualElem(
                        hv_ImageHeightRaw);
                }
                if ((int)((new HTuple(((hv_EqualWidth.TupleMin())).TupleEqual(0))).TupleOr(
                    new HTuple(((hv_EqualHeight.TupleMin())).TupleEqual(0)))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0,
                            hv_ImageWidth, hv_ImageHeight, "nearest_neighbor");
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the type of the input images
                //and convert if necessary.
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_Segmentations_COPY_INP_TMP, out hv_Type);
                hv_EqualReal.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualReal = hv_Type.TupleEqualElem(
                        "real");
                }
                //
                if ((int)(new HTuple(((hv_EqualReal.TupleMin())).TupleEqual(0))) != 0)
                {
                    //Convert the image type to 'real',
                    //because the model expects 'real' images.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Write preprocessed Segmentations to output variable.
                ho_SegmentationsPreprocessed.Dispose();
                ho_SegmentationsPreprocessed = new HObject(ho_Segmentations_COPY_INP_TMP);
                ho_Segmentations_COPY_INP_TMP.Dispose();
                ho_Domain.Dispose();
                ho_SelectedSeg.Dispose();
                ho_SelectedDomain.Dispose();

                hv_NumberImages.Dispose();
                hv_NumberSegmentations.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WidthSeg.Dispose();
                hv_HeightSeg.Dispose();
                hv_DLModelType.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassesToBackground.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_IsInt.Dispose();
                hv_IndexImage.Dispose();
                hv_ImageWidthRaw.Dispose();
                hv_ImageHeightRaw.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_EqualReal.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Segmentations_COPY_INP_TMP.Dispose();
                ho_Domain.Dispose();
                ho_SelectedSeg.Dispose();
                ho_SelectedDomain.Dispose();

                hv_NumberImages.Dispose();
                hv_NumberSegmentations.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WidthSeg.Dispose();
                hv_HeightSeg.Dispose();
                hv_DLModelType.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassesToBackground.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_IsInt.Dispose();
                hv_IndexImage.Dispose();
                hv_ImageWidthRaw.Dispose();
                hv_ImageHeightRaw.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_EqualReal.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess given DLSamples according to the preprocessing parameters given in DLPreprocessParam. 
        public void preprocess_dl_samples(HTuple hv_DLSampleBatch, HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            HObject ho_ImageRaw = null, ho_ImagePreprocessed = null;
            HObject ho_AnomalyImageRaw = null, ho_AnomalyImagePreprocessed = null;
            HObject ho_SegmentationRaw = null, ho_SegmentationPreprocessed = null;

            // Local control variables 

            HTuple hv_ModelType = new HTuple(), hv_SampleIndex = new HTuple();
            HTuple hv_ImageExists = new HTuple(), hv_KeysExists = new HTuple();
            HTuple hv_AnomalyParamExist = new HTuple(), hv_Rectangle1ParamExist = new HTuple();
            HTuple hv_Rectangle2ParamExist = new HTuple(), hv_SegmentationParamExist = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageRaw);
            HOperatorSet.GenEmptyObj(out ho_ImagePreprocessed);
            HOperatorSet.GenEmptyObj(out ho_AnomalyImageRaw);
            HOperatorSet.GenEmptyObj(out ho_AnomalyImagePreprocessed);
            HOperatorSet.GenEmptyObj(out ho_SegmentationRaw);
            HOperatorSet.GenEmptyObj(out ho_SegmentationPreprocessed);
            try
            {
                //
                //This procedure preprocesses all images of the sample dictionaries in the tuple DLSampleBatch.
                //The images are preprocessed according to the parameters provided in DLPreprocessParam.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                hv_ModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_ModelType);
                //
                //Preprocess the sample entries.
                //
                for (hv_SampleIndex = 0; (int)hv_SampleIndex <= (int)((new HTuple(hv_DLSampleBatch.TupleLength()
                    )) - 1); hv_SampleIndex = (int)hv_SampleIndex + 1)
                {
                    //
                    //Check the existence of the sample keys.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageExists.Dispose();
                        HOperatorSet.GetDictParam(hv_DLSampleBatch.TupleSelect(hv_SampleIndex), "key_exists",
                            "image", out hv_ImageExists);
                    }
                    //
                    //Preprocess the images.
                    if ((int)(hv_ImageExists) != 0)
                    {
                        //
                        //Get the image.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_ImageRaw.Dispose();
                            HOperatorSet.GetDictObject(out ho_ImageRaw, hv_DLSampleBatch.TupleSelect(
                                hv_SampleIndex), "image");
                        }
                        //
                        //Preprocess the image.
                        ho_ImagePreprocessed.Dispose();
                        preprocess_dl_model_images(ho_ImageRaw, out ho_ImagePreprocessed, hv_DLPreprocessParam);
                        //
                        //Replace the image in the dictionary.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictObject(ho_ImagePreprocessed, hv_DLSampleBatch.TupleSelect(
                                hv_SampleIndex), "image");
                        }
                        //
                        //Check existence of model specific sample keys:
                        //- bbox_row1 for 'rectangle1'
                        //- bbox_phi for 'rectangle2'
                        //- segmentation_image for 'semantic segmentation'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_KeysExists.Dispose();
                            HOperatorSet.GetDictParam(hv_DLSampleBatch.TupleSelect(hv_SampleIndex),
                                "key_exists", (((new HTuple("anomaly_ground_truth")).TupleConcat("bbox_row1")).TupleConcat(
                                "bbox_phi")).TupleConcat("segmentation_image"), out hv_KeysExists);
                        }
                        hv_AnomalyParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AnomalyParamExist = hv_KeysExists.TupleSelect(
                                0);
                        }
                        hv_Rectangle1ParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Rectangle1ParamExist = hv_KeysExists.TupleSelect(
                                1);
                        }
                        hv_Rectangle2ParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Rectangle2ParamExist = hv_KeysExists.TupleSelect(
                                2);
                        }
                        hv_SegmentationParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_SegmentationParamExist = hv_KeysExists.TupleSelect(
                                3);
                        }
                        //
                        //Preprocess the anomaly ground truth if present.
                        if ((int)(hv_AnomalyParamExist) != 0)
                        {
                            //
                            //Get the anomaly image.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_AnomalyImageRaw.Dispose();
                                HOperatorSet.GetDictObject(out ho_AnomalyImageRaw, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), "anomaly_ground_truth");
                            }
                            //
                            //Preprocess the anomaly image.
                            ho_AnomalyImagePreprocessed.Dispose();
                            preprocess_dl_model_anomaly(ho_AnomalyImageRaw, out ho_AnomalyImagePreprocessed,
                                hv_DLPreprocessParam);
                            //
                            //Set preprocessed anomaly image.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetDictObject(ho_AnomalyImagePreprocessed, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), "anomaly_ground_truth");
                            }
                        }
                        //
                        //Preprocess depending on the model type.
                        //If bounding boxes are given, rescale them as well.
                        if ((int)(hv_Rectangle1ParamExist) != 0)
                        {
                            //
                            //Preprocess the bounding boxes of type 'rectangle1'.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                preprocess_dl_model_bbox_rect1(ho_ImageRaw, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), hv_DLPreprocessParam);
                            }
                        }
                        else if ((int)(hv_Rectangle2ParamExist) != 0)
                        {
                            //
                            //Preprocess the bounding boxes of type 'rectangle2'.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                preprocess_dl_model_bbox_rect2(ho_ImageRaw, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), hv_DLPreprocessParam);
                            }
                        }
                        //
                        //Preprocess the segmentation image if present.
                        if ((int)(hv_SegmentationParamExist) != 0)
                        {
                            //
                            //Get the segmentation image.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_SegmentationRaw.Dispose();
                                HOperatorSet.GetDictObject(out ho_SegmentationRaw, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), "segmentation_image");
                            }
                            //
                            //Preprocess the segmentation image.
                            ho_SegmentationPreprocessed.Dispose();
                            preprocess_dl_model_segmentations(ho_ImageRaw, ho_SegmentationRaw, out ho_SegmentationPreprocessed,
                                hv_DLPreprocessParam);
                            //
                            //Set preprocessed segmentation image.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetDictObject(ho_SegmentationPreprocessed, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), "segmentation_image");
                            }
                        }
                    }
                    else
                    {
                        throw new HalconException((new HTuple("All samples processed need to include an image, but the sample with index ") + hv_SampleIndex) + " does not.");
                    }
                }
                //
                ho_ImageRaw.Dispose();
                ho_ImagePreprocessed.Dispose();
                ho_AnomalyImageRaw.Dispose();
                ho_AnomalyImagePreprocessed.Dispose();
                ho_SegmentationRaw.Dispose();
                ho_SegmentationPreprocessed.Dispose();

                hv_ModelType.Dispose();
                hv_SampleIndex.Dispose();
                hv_ImageExists.Dispose();
                hv_KeysExists.Dispose();
                hv_AnomalyParamExist.Dispose();
                hv_Rectangle1ParamExist.Dispose();
                hv_Rectangle2ParamExist.Dispose();
                hv_SegmentationParamExist.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageRaw.Dispose();
                ho_ImagePreprocessed.Dispose();
                ho_AnomalyImageRaw.Dispose();
                ho_AnomalyImagePreprocessed.Dispose();
                ho_SegmentationRaw.Dispose();
                ho_SegmentationPreprocessed.Dispose();

                hv_ModelType.Dispose();
                hv_SampleIndex.Dispose();
                hv_ImageExists.Dispose();
                hv_KeysExists.Dispose();
                hv_AnomalyParamExist.Dispose();
                hv_Rectangle1ParamExist.Dispose();
                hv_Rectangle2ParamExist.Dispose();
                hv_SegmentationParamExist.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Image / Manipulation
        // Short Description: Changes a value of ValuesToChange in Image to NewValue. 
        public void reassign_pixel_values(HObject ho_Image, out HObject ho_ImageOut, HTuple hv_ValuesToChange,
            HTuple hv_NewValue)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionToChange, ho_RegionClass = null;

            // Local control variables 

            HTuple hv_IndexReset = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageOut);
            HOperatorSet.GenEmptyObj(out ho_RegionToChange);
            HOperatorSet.GenEmptyObj(out ho_RegionClass);
            try
            {
                //
                //This procedure sets all pixels of Image
                //with the values given in ValuesToChange to the given value NewValue.
                //
                ho_RegionToChange.Dispose();
                HOperatorSet.GenEmptyRegion(out ho_RegionToChange);
                for (hv_IndexReset = 0; (int)hv_IndexReset <= (int)((new HTuple(hv_ValuesToChange.TupleLength()
                    )) - 1); hv_IndexReset = (int)hv_IndexReset + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_RegionClass.Dispose();
                        HOperatorSet.Threshold(ho_Image, out ho_RegionClass, hv_ValuesToChange.TupleSelect(
                            hv_IndexReset), hv_ValuesToChange.TupleSelect(hv_IndexReset));
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.Union2(ho_RegionToChange, ho_RegionClass, out ExpTmpOutVar_0
                            );
                        ho_RegionToChange.Dispose();
                        ho_RegionToChange = ExpTmpOutVar_0;
                    }
                }
                HOperatorSet.OverpaintRegion(ho_Image, ho_RegionToChange, hv_NewValue, "fill");
                ho_ImageOut.Dispose();
                ho_ImageOut = new HObject(ho_Image);
                ho_RegionToChange.Dispose();
                ho_RegionClass.Dispose();

                hv_IndexReset.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_RegionToChange.Dispose();
                ho_RegionClass.Dispose();

                hv_IndexReset.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: This procedure replaces legacy preprocessing parameters. 
        public void replace_legacy_preprocessing_parameters(HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Exception = new HTuple(), hv_NormalizationTypeExists = new HTuple();
            HTuple hv_NormalizationType = new HTuple(), hv_LegacyNormalizationKeyExists = new HTuple();
            HTuple hv_ContrastNormalization = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //
                //This procedure adapts the dictionary DLPreprocessParam
                //if a legacy preprocessing parameter is set.
                //
                //Map legacy value set to new parameter.
                hv_Exception.Dispose();
                hv_Exception = 0;
                try
                {
                    hv_NormalizationTypeExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "normalization_type",
                        out hv_NormalizationTypeExists);
                    //
                    if ((int)(hv_NormalizationTypeExists) != 0)
                    {
                        hv_NormalizationType.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "normalization_type", out hv_NormalizationType);
                        if ((int)(new HTuple(hv_NormalizationType.TupleEqual("true"))) != 0)
                        {
                            hv_NormalizationType.Dispose();
                            hv_NormalizationType = "first_channel";
                        }
                        else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("false"))) != 0)
                        {
                            hv_NormalizationType.Dispose();
                            hv_NormalizationType = "none";
                        }
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type", hv_NormalizationType);
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }
                //
                //Map legacy parameter to new parameter and corresponding value.
                hv_Exception.Dispose();
                hv_Exception = 0;
                try
                {
                    hv_LegacyNormalizationKeyExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "contrast_normalization",
                        out hv_LegacyNormalizationKeyExists);
                    if ((int)(hv_LegacyNormalizationKeyExists) != 0)
                    {
                        hv_ContrastNormalization.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "contrast_normalization",
                            out hv_ContrastNormalization);
                        //Replace 'contrast_normalization' by 'normalization_type'.
                        if ((int)(new HTuple(hv_ContrastNormalization.TupleEqual("false"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type",
                                "none");
                        }
                        else if ((int)(new HTuple(hv_ContrastNormalization.TupleEqual(
                            "true"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type",
                                "first_channel");
                        }
                        HOperatorSet.RemoveDictKey(hv_DLPreprocessParam, "contrast_normalization");
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }

                hv_Exception.Dispose();
                hv_NormalizationTypeExists.Dispose();
                hv_NormalizationType.Dispose();
                hv_LegacyNormalizationKeyExists.Dispose();
                hv_ContrastNormalization.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Exception.Dispose();
                hv_NormalizationTypeExists.Dispose();
                hv_NormalizationType.Dispose();
                hv_LegacyNormalizationKeyExists.Dispose();
                hv_ContrastNormalization.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Filters / Arithmetic
        // Short Description: Scale the gray values of an image from the interval [Min,Max] to [0,255] 
        public void scale_image_range(HObject ho_Image, out HObject ho_ImageScaled, HTuple hv_Min,
            HTuple hv_Max)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImageSelected = null, ho_SelectedChannel = null;
            HObject ho_LowerRegion = null, ho_UpperRegion = null, ho_ImageSelectedScaled = null;

            // Local copy input parameter variables 
            HObject ho_Image_COPY_INP_TMP;
            ho_Image_COPY_INP_TMP = new HObject(ho_Image);



            // Local control variables 

            HTuple hv_LowerLimit = new HTuple(), hv_UpperLimit = new HTuple();
            HTuple hv_Mult = new HTuple(), hv_Add = new HTuple(), hv_NumImages = new HTuple();
            HTuple hv_ImageIndex = new HTuple(), hv_Channels = new HTuple();
            HTuple hv_ChannelIndex = new HTuple(), hv_MinGray = new HTuple();
            HTuple hv_MaxGray = new HTuple(), hv_Range = new HTuple();
            HTuple hv_Max_COPY_INP_TMP = new HTuple(hv_Max);
            HTuple hv_Min_COPY_INP_TMP = new HTuple(hv_Min);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_ImageSelected);
            HOperatorSet.GenEmptyObj(out ho_SelectedChannel);
            HOperatorSet.GenEmptyObj(out ho_LowerRegion);
            HOperatorSet.GenEmptyObj(out ho_UpperRegion);
            HOperatorSet.GenEmptyObj(out ho_ImageSelectedScaled);
            try
            {
                //Convenience procedure to scale the gray values of the
                //input image Image from the interval [Min,Max]
                //to the interval [0,255] (default).
                //Gray values < 0 or > 255 (after scaling) are clipped.
                //
                //If the image shall be scaled to an interval different from [0,255],
                //this can be achieved by passing tuples with 2 values [From, To]
                //as Min and Max.
                //Example:
                //scale_image_range(Image:ImageScaled:[100,50],[200,250])
                //maps the gray values of Image from the interval [100,200] to [50,250].
                //All other gray values will be clipped.
                //
                //input parameters:
                //Image: the input image
                //Min: the minimum gray value which will be mapped to 0
                //     If a tuple with two values is given, the first value will
                //     be mapped to the second value.
                //Max: The maximum gray value which will be mapped to 255
                //     If a tuple with two values is given, the first value will
                //     be mapped to the second value.
                //
                //Output parameter:
                //ImageScale: the resulting scaled image.
                //
                if ((int)(new HTuple((new HTuple(hv_Min_COPY_INP_TMP.TupleLength())).TupleEqual(
                    2))) != 0)
                {
                    hv_LowerLimit.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_LowerLimit = hv_Min_COPY_INP_TMP.TupleSelect(
                            1);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Min = hv_Min_COPY_INP_TMP.TupleSelect(
                                0);
                            hv_Min_COPY_INP_TMP.Dispose();
                            hv_Min_COPY_INP_TMP = ExpTmpLocalVar_Min;
                        }
                    }
                }
                else
                {
                    hv_LowerLimit.Dispose();
                    hv_LowerLimit = 0.0;
                }
                if ((int)(new HTuple((new HTuple(hv_Max_COPY_INP_TMP.TupleLength())).TupleEqual(
                    2))) != 0)
                {
                    hv_UpperLimit.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_UpperLimit = hv_Max_COPY_INP_TMP.TupleSelect(
                            1);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Max = hv_Max_COPY_INP_TMP.TupleSelect(
                                0);
                            hv_Max_COPY_INP_TMP.Dispose();
                            hv_Max_COPY_INP_TMP = ExpTmpLocalVar_Max;
                        }
                    }
                }
                else
                {
                    hv_UpperLimit.Dispose();
                    hv_UpperLimit = 255.0;
                }
                //
                //Calculate scaling parameters.
                hv_Mult.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Mult = (((hv_UpperLimit - hv_LowerLimit)).TupleReal()
                        ) / (hv_Max_COPY_INP_TMP - hv_Min_COPY_INP_TMP);
                }
                hv_Add.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Add = ((-hv_Mult) * hv_Min_COPY_INP_TMP) + hv_LowerLimit;
                }
                //
                //Scale image.
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ScaleImage(ho_Image_COPY_INP_TMP, out ExpTmpOutVar_0, hv_Mult,
                        hv_Add);
                    ho_Image_COPY_INP_TMP.Dispose();
                    ho_Image_COPY_INP_TMP = ExpTmpOutVar_0;
                }
                //
                //Clip gray values if necessary.
                //This must be done for each image and channel separately.
                ho_ImageScaled.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ImageScaled);
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_Image_COPY_INP_TMP, out hv_NumImages);
                HTuple end_val49 = hv_NumImages;
                HTuple step_val49 = 1;
                for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val49, step_val49); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val49))
                {
                    ho_ImageSelected.Dispose();
                    HOperatorSet.SelectObj(ho_Image_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                    hv_Channels.Dispose();
                    HOperatorSet.CountChannels(ho_ImageSelected, out hv_Channels);
                    HTuple end_val52 = hv_Channels;
                    HTuple step_val52 = 1;
                    for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val52, step_val52); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val52))
                    {
                        ho_SelectedChannel.Dispose();
                        HOperatorSet.AccessChannel(ho_ImageSelected, out ho_SelectedChannel, hv_ChannelIndex);
                        hv_MinGray.Dispose(); hv_MaxGray.Dispose(); hv_Range.Dispose();
                        HOperatorSet.MinMaxGray(ho_SelectedChannel, ho_SelectedChannel, 0, out hv_MinGray,
                            out hv_MaxGray, out hv_Range);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_LowerRegion.Dispose();
                            HOperatorSet.Threshold(ho_SelectedChannel, out ho_LowerRegion, ((hv_MinGray.TupleConcat(
                                hv_LowerLimit))).TupleMin(), hv_LowerLimit);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_UpperRegion.Dispose();
                            HOperatorSet.Threshold(ho_SelectedChannel, out ho_UpperRegion, hv_UpperLimit,
                                ((hv_UpperLimit.TupleConcat(hv_MaxGray))).TupleMax());
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.PaintRegion(ho_LowerRegion, ho_SelectedChannel, out ExpTmpOutVar_0,
                                hv_LowerLimit, "fill");
                            ho_SelectedChannel.Dispose();
                            ho_SelectedChannel = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.PaintRegion(ho_UpperRegion, ho_SelectedChannel, out ExpTmpOutVar_0,
                                hv_UpperLimit, "fill");
                            ho_SelectedChannel.Dispose();
                            ho_SelectedChannel = ExpTmpOutVar_0;
                        }
                        if ((int)(new HTuple(hv_ChannelIndex.TupleEqual(1))) != 0)
                        {
                            ho_ImageSelectedScaled.Dispose();
                            HOperatorSet.CopyObj(ho_SelectedChannel, out ho_ImageSelectedScaled,
                                1, 1);
                        }
                        else
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.AppendChannel(ho_ImageSelectedScaled, ho_SelectedChannel,
                                    out ExpTmpOutVar_0);
                                ho_ImageSelectedScaled.Dispose();
                                ho_ImageSelectedScaled = ExpTmpOutVar_0;
                            }
                        }
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_ImageScaled, ho_ImageSelectedScaled, out ExpTmpOutVar_0
                            );
                        ho_ImageScaled.Dispose();
                        ho_ImageScaled = ExpTmpOutVar_0;
                    }
                }
                ho_Image_COPY_INP_TMP.Dispose();
                ho_ImageSelected.Dispose();
                ho_SelectedChannel.Dispose();
                ho_LowerRegion.Dispose();
                ho_UpperRegion.Dispose();
                ho_ImageSelectedScaled.Dispose();

                hv_Max_COPY_INP_TMP.Dispose();
                hv_Min_COPY_INP_TMP.Dispose();
                hv_LowerLimit.Dispose();
                hv_UpperLimit.Dispose();
                hv_Mult.Dispose();
                hv_Add.Dispose();
                hv_NumImages.Dispose();
                hv_ImageIndex.Dispose();
                hv_Channels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_MinGray.Dispose();
                hv_MaxGray.Dispose();
                hv_Range.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image_COPY_INP_TMP.Dispose();
                ho_ImageSelected.Dispose();
                ho_SelectedChannel.Dispose();
                ho_LowerRegion.Dispose();
                ho_UpperRegion.Dispose();
                ho_ImageSelectedScaled.Dispose();

                hv_Max_COPY_INP_TMP.Dispose();
                hv_Min_COPY_INP_TMP.Dispose();
                hv_LowerLimit.Dispose();
                hv_UpperLimit.Dispose();
                hv_Mult.Dispose();
                hv_Add.Dispose();
                hv_NumImages.Dispose();
                hv_ImageIndex.Dispose();
                hv_Channels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_MinGray.Dispose();
                hv_MaxGray.Dispose();
                hv_Range.Dispose();

                throw HDevExpDefaultException;
            }
        }

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

        // Chapter: Tuple / Element Order
        // Short Description: Sort the elements of a tuple randomly. 
        public void tuple_shuffle(HTuple hv_Tuple, out HTuple hv_Shuffled)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ShuffleIndices = new HTuple();
            // Initialize local and output iconic variables 
            hv_Shuffled = new HTuple();
            try
            {
                //This procedure sorts the input tuple randomly.
                //
                if ((int)(new HTuple((new HTuple(hv_Tuple.TupleLength())).TupleGreater(0))) != 0)
                {
                    //Create a tuple of random numbers,
                    //sort this tuple, and return the indices
                    //of this sorted tuple.
                    hv_ShuffleIndices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ShuffleIndices = (HTuple.TupleRand(
                            new HTuple(hv_Tuple.TupleLength()))).TupleSortIndex();
                    }
                    //Assign the elements of Tuple
                    //to these random positions.
                    hv_Shuffled.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Shuffled = hv_Tuple.TupleSelect(
                            hv_ShuffleIndices);
                    }
                }
                else
                {
                    //If the input tuple is empty,
                    //an empty tuple should be returned.
                    hv_Shuffled.Dispose();
                    hv_Shuffled = new HTuple();
                }

                hv_ShuffleIndices.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_ShuffleIndices.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Window
        // Short Description: This procedure sets and returns meta information to display images correctly. 
        public void update_window_meta_information(HTuple hv_WindowHandle, HTuple hv_WidthImage,
            HTuple hv_HeightImage, HTuple hv_WindowRow1, HTuple hv_WindowColumn1, HTuple hv_MapColorBarWidth,
            HTuple hv_MarginBottom, out HTuple hv_WindowImageRatioHeight, out HTuple hv_WindowImageRatioWidth,
            out HTuple hv_SetPartRow2, out HTuple hv_SetPartColumn2, out HTuple hv_PrevWindowCoordinatesOut)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv__ = new HTuple(), hv_WindowWidth = new HTuple();
            HTuple hv_WindowHeight = new HTuple(), hv_WindowRow2 = new HTuple();
            HTuple hv_WindowColumn2 = new HTuple(), hv_WindowRatio = new HTuple();
            HTuple hv_ImageRow2 = new HTuple(), hv_ImageColumn2 = new HTuple();
            HTuple hv_ImageRatio = new HTuple(), hv_ImageWindowRatioHeight = new HTuple();
            HTuple hv_ImageRow2InWindow = new HTuple(), hv_ImageCol2InWindow = new HTuple();
            // Initialize local and output iconic variables 
            hv_WindowImageRatioHeight = new HTuple();
            hv_WindowImageRatioWidth = new HTuple();
            hv_SetPartRow2 = new HTuple();
            hv_SetPartColumn2 = new HTuple();
            hv_PrevWindowCoordinatesOut = new HTuple();
            try
            {
                //
                //This procedure sets and returns meta information to display images correctly.
                //
                //Set part for the image to be displayed later and adapt window size (+ MarginBottom + MapColorBarWidth).
                hv__.Dispose(); hv__.Dispose(); hv_WindowWidth.Dispose(); hv_WindowHeight.Dispose();
                HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv__, out hv__, out hv_WindowWidth,
                    out hv_WindowHeight);
                hv_WindowImageRatioHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowImageRatioHeight = hv_WindowHeight / (hv_HeightImage * 1.0);
                }
                hv_WindowImageRatioWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowImageRatioWidth = hv_WindowWidth / (hv_WidthImage * 1.0);
                }
                //
                //Set window part such that image is displayed undistorted.
                hv_WindowRow2.Dispose();
                hv_WindowRow2 = new HTuple(hv_WindowHeight);
                hv_WindowColumn2.Dispose();
                hv_WindowColumn2 = new HTuple(hv_WindowWidth);
                hv_WindowRatio.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowRatio = hv_WindowColumn2 / (hv_WindowRow2 * 1.0);
                }
                //
                hv_ImageRow2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageRow2 = hv_HeightImage + (hv_MarginBottom / hv_WindowImageRatioHeight);
                }
                hv_ImageColumn2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageColumn2 = hv_WidthImage + (hv_MapColorBarWidth / hv_WindowImageRatioWidth);
                }
                hv_ImageRatio.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageRatio = hv_ImageColumn2 / (hv_ImageRow2 * 1.0);
                }
                if ((int)(new HTuple(hv_ImageRatio.TupleGreater(hv_WindowRatio))) != 0)
                {
                    //
                    //Extend image until right window border.
                    hv_SetPartColumn2.Dispose();
                    hv_SetPartColumn2 = new HTuple(hv_ImageColumn2);
                    hv_ImageWindowRatioHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageWindowRatioHeight = hv_ImageColumn2 / (hv_WindowColumn2 * 1.0);
                    }
                    hv_ImageRow2InWindow.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageRow2InWindow = hv_ImageRow2 / hv_ImageWindowRatioHeight;
                    }
                    hv_SetPartRow2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SetPartRow2 = hv_ImageRow2 + ((hv_WindowRow2 - hv_ImageRow2InWindow) / hv_WindowImageRatioWidth);
                    }
                }
                else
                {
                    //
                    //Extend image until bottom of window.
                    hv_SetPartRow2.Dispose();
                    hv_SetPartRow2 = new HTuple(hv_ImageRow2);
                    hv_ImageWindowRatioHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageWindowRatioHeight = hv_ImageRow2 / (hv_WindowRow2 * 1.0);
                    }
                    hv_ImageCol2InWindow.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageCol2InWindow = hv_ImageColumn2 / hv_ImageWindowRatioHeight;
                    }
                    hv_SetPartColumn2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SetPartColumn2 = hv_ImageColumn2 + ((hv_WindowColumn2 - hv_ImageCol2InWindow) / hv_WindowImageRatioHeight);
                    }
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetPart(HDevWindowStack.GetActive(), 0, 0, hv_SetPartRow2, hv_SetPartColumn2);
                }
                //
                //Return the coordinates of the new window.
                hv_PrevWindowCoordinatesOut.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PrevWindowCoordinatesOut = new HTuple();
                    hv_PrevWindowCoordinatesOut = hv_PrevWindowCoordinatesOut.TupleConcat(hv_WindowRow1, hv_WindowColumn1, hv_WindowWidth, hv_WindowHeight);
                }
                //

                hv__.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();
                hv_WindowRow2.Dispose();
                hv_WindowColumn2.Dispose();
                hv_WindowRatio.Dispose();
                hv_ImageRow2.Dispose();
                hv_ImageColumn2.Dispose();
                hv_ImageRatio.Dispose();
                hv_ImageWindowRatioHeight.Dispose();
                hv_ImageRow2InWindow.Dispose();
                hv_ImageCol2InWindow.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv__.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();
                hv_WindowRow2.Dispose();
                hv_WindowColumn2.Dispose();
                hv_WindowRatio.Dispose();
                hv_ImageRow2.Dispose();
                hv_ImageColumn2.Dispose();
                hv_ImageRatio.Dispose();
                hv_ImageWindowRatioHeight.Dispose();
                hv_ImageRow2InWindow.Dispose();
                hv_ImageCol2InWindow.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Local procedures 
        public void check_data_availability(HTuple hv_ExampleDataDir, HTuple hv_PreprocessParamFileName, HTuple hv_TrainedModelFileName, HTuple hv_UsePretrainedModel)      
        {



            // Local control variables 

            HTuple hv_FileExists = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure checks if all necessary files are available.
                //
                hv_FileExists.Dispose();
                HOperatorSet.FileExists(hv_ExampleDataDir, out hv_FileExists);
                if ((int)(hv_FileExists.TupleNot()) != 0)
                {
                    throw new HalconException(hv_ExampleDataDir + " does not exist. Please run part 1 and 2 of example series.");
                }

                hv_FileExists.Dispose();
                HOperatorSet.FileExists(hv_PreprocessParamFileName, out hv_FileExists);
                if ((int)(hv_FileExists.TupleNot()) != 0)
                {
                    throw new HalconException(hv_PreprocessParamFileName + " does not exist. Please run part 1 of example series.");
                }
                //
                hv_FileExists.Dispose();
                HOperatorSet.FileExists(hv_TrainedModelFileName, out hv_FileExists);
                if ((int)(hv_FileExists.TupleNot()) != 0)
                {
                    if ((int)(hv_UsePretrainedModel) != 0)
                    {
                        throw new HalconException(hv_TrainedModelFileName + " does not exist. Please run the HALCON Deep Learning installer.");
                    }
                    else
                    {
                        throw new HalconException(hv_TrainedModelFileName + " does not exist. Please run part 2 of example series.");
                    }
                }
                //

                hv_FileExists.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_FileExists.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_close_example_image_window(HTuple hv_ExampleInternals)
        {



            // Local control variables 

            HTuple hv_WindowHandleImages = new HTuple();
            HTuple hv_Exception = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure closes the image window.

                try
                {
                    hv_WindowHandleImages.Dispose();
                    HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images", out hv_WindowHandleImages);
                    HDevWindowStack.SetActive(hv_WindowHandleImages);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.CloseWindow(HDevWindowStack.Pop());
                    }
                    //Delete key.
                    HOperatorSet.RemoveDictKey(hv_ExampleInternals, "window_images");
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }


                hv_WindowHandleImages.Dispose();
                hv_Exception.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_WindowHandleImages.Dispose();
                hv_Exception.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_close_example_legend_window(HTuple hv_ExampleInternals)
        {



            // Local control variables 

            HTuple hv_WindowHandleLegend = new HTuple();
            HTuple hv_Exception = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure closes the legend window.

                try
                {
                    hv_WindowHandleLegend.Dispose();
                    HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_legend", out hv_WindowHandleLegend);
                    HDevWindowStack.SetActive(hv_WindowHandleLegend);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.CloseWindow(HDevWindowStack.Pop());
                    }
                    //Delete key.
                    HOperatorSet.RemoveDictKey(hv_ExampleInternals, "window_legend");
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }


                hv_WindowHandleLegend.Dispose();
                hv_Exception.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_WindowHandleLegend.Dispose();
                hv_Exception.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_close_example_text_window(HTuple hv_ExampleInternals)
        {



            // Local control variables 

            HTuple hv_WindowHandleImages = new HTuple();
            HTuple hv_Exception = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure closes the text window.

                try
                {
                    hv_WindowHandleImages.Dispose();
                    HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_text", out hv_WindowHandleImages);
                    HDevWindowStack.SetActive(hv_WindowHandleImages);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.CloseWindow(HDevWindowStack.Pop());
                    }
                    //Delete key.
                    HOperatorSet.RemoveDictKey(hv_ExampleInternals, "window_text");
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }

                hv_WindowHandleImages.Dispose();
                hv_Exception.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_WindowHandleImages.Dispose();
                hv_Exception.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_close_example_windows(HTuple hv_ExampleInternals)
        {



            // Local control variables 

            HTuple hv_ShowExampleScreens = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure closes all example windows.
                //
                hv_ShowExampleScreens.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "show_example_screens", out hv_ShowExampleScreens);
                if ((int)(hv_ShowExampleScreens.TupleNot()) != 0)
                {

                    hv_ShowExampleScreens.Dispose();

                    return;
                }
                //
                dev_close_example_text_window(hv_ExampleInternals);
                dev_close_example_image_window(hv_ExampleInternals);
                dev_close_example_legend_window(hv_ExampleInternals);
                //

                hv_ShowExampleScreens.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_ShowExampleScreens.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_example_reset_windows(HTuple hv_ExampleInternals)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_WindowHandlesToClose = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_I = new HTuple();
            HTuple hv_WindowHandleKeys = new HTuple(), hv_Index = new HTuple();
            HTuple hv_WindowImagesNeeded = new HTuple(), hv_WindowLegendNeeded = new HTuple();
            HTuple hv_WindowHandleImages = new HTuple(), hv_WindowHandleLegend = new HTuple();
            HTuple hv_WindowHandleText = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure resets the graphics windows.
                //
                //Close any windows that are listed in key 'window_handles_to_close'.
                try
                {
                    hv_WindowHandlesToClose.Dispose();
                    HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_handles_to_close",
                        out hv_WindowHandlesToClose);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_WindowHandlesToClose.Dispose();
                    hv_WindowHandlesToClose = new HTuple();
                }
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_WindowHandlesToClose.TupleLength()
                    )) - 1); hv_I = (int)hv_I + 1)
                {
                    HDevWindowStack.SetActive(hv_WindowHandlesToClose.TupleSelect(
                        hv_I));
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.CloseWindow(HDevWindowStack.Pop());
                    }
                }
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_handles_to_close", new HTuple());
                //
                //Open image window if needed.
                hv_WindowHandleKeys.Dispose();
                HOperatorSet.GetDictParam(hv_ExampleInternals, "keys", new HTuple(), out hv_WindowHandleKeys);
                hv_Index.Dispose();
                HOperatorSet.TupleFind(hv_WindowHandleKeys, "window_images", out hv_Index);
                hv_WindowImagesNeeded.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images_needed", out hv_WindowImagesNeeded);
                if ((int)(hv_WindowImagesNeeded.TupleAnd(new HTuple(hv_Index.TupleEqual(-1)))) != 0)
                {
                    //Open new window for images.
                    dev_open_example_image_window(hv_ExampleInternals);
                }
                else if ((int)((new HTuple(hv_WindowImagesNeeded.TupleNot())).TupleAnd(
                    new HTuple(hv_Index.TupleNotEqual(-1)))) != 0)
                {
                    //Window for images exists but is not needed -> close it.
                    dev_close_example_image_window(hv_ExampleInternals);
                }
                //
                //Open legend window if needed.
                hv_WindowHandleKeys.Dispose();
                HOperatorSet.GetDictParam(hv_ExampleInternals, "keys", new HTuple(), out hv_WindowHandleKeys);
                hv_Index.Dispose();
                HOperatorSet.TupleFind(hv_WindowHandleKeys, "window_legend", out hv_Index);
                hv_WindowLegendNeeded.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_legend_needed", out hv_WindowLegendNeeded);
                if ((int)(hv_WindowLegendNeeded.TupleAnd(new HTuple(hv_Index.TupleEqual(-1)))) != 0)
                {
                    //Open new window for legend.
                    dev_open_example_legend_window(hv_ExampleInternals, 280);
                }
                else if ((int)((new HTuple(hv_WindowLegendNeeded.TupleNot())).TupleAnd(
                    new HTuple(hv_Index.TupleNotEqual(-1)))) != 0)
                {
                    //Window for legend exists but is not needed -> close it.
                    dev_close_example_legend_window(hv_ExampleInternals);
                }
                //
                //Set the correct area (part) of the image window.
                try
                {
                    hv_WindowHandleImages.Dispose();
                    HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images", out hv_WindowHandleImages);
                    HDevWindowStack.SetActive(hv_WindowHandleImages);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
                    }
                    //Set default window extends
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetWindowExtents(HDevWindowStack.GetActive(), 360, 0, 500,
                            500);
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetPart(HDevWindowStack.GetActive(), 1, 1, -1, -1);
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }
                //
                //Set the correct area (part) of the legend window.
                try
                {
                    hv_WindowHandleLegend.Dispose();
                    HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_legend", out hv_WindowHandleLegend);
                    HDevWindowStack.SetActive(hv_WindowHandleLegend);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetPart(HDevWindowStack.GetActive(), 1, 1, -1, -1);
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }
                hv_WindowHandleText.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_text", out hv_WindowHandleText);
                HDevWindowStack.SetActive(hv_WindowHandleText);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
                }
                //

                hv_WindowHandlesToClose.Dispose();
                hv_Exception.Dispose();
                hv_I.Dispose();
                hv_WindowHandleKeys.Dispose();
                hv_Index.Dispose();
                hv_WindowImagesNeeded.Dispose();
                hv_WindowLegendNeeded.Dispose();
                hv_WindowHandleImages.Dispose();
                hv_WindowHandleLegend.Dispose();
                hv_WindowHandleText.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_WindowHandlesToClose.Dispose();
                hv_Exception.Dispose();
                hv_I.Dispose();
                hv_WindowHandleKeys.Dispose();
                hv_Index.Dispose();
                hv_WindowImagesNeeded.Dispose();
                hv_WindowLegendNeeded.Dispose();
                hv_WindowHandleImages.Dispose();
                hv_WindowHandleLegend.Dispose();
                hv_WindowHandleText.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_model_output_image(HTuple hv_ExampleInternals)
        {



            // Local iconic variables 

            HObject ho_Image;

            // Local control variables 

            HTuple hv_WindowHandleImages = new HTuple();
            HTuple hv_WindowHandleLegend = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_DLResult = new HTuple();
            HTuple hv_DLSample = new HTuple(), hv_DLDatasetInfo = new HTuple();
            HTuple hv_WindowHandleDict = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            try
            {
                //This procedure visualizes the apply step.
                //
                //Get windows and adapt size.
                hv_WindowHandleImages.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images", out hv_WindowHandleImages);
                hv_WindowHandleLegend.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_legend", out hv_WindowHandleLegend);
                hv_Row.Dispose(); hv_Column.Dispose(); hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetWindowExtents(hv_WindowHandleImages, out hv_Row, out hv_Column,
                    out hv_Width, out hv_Height);
                HDevWindowStack.SetActive(hv_WindowHandleImages);
                if (HDevWindowStack.IsOpen())
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetWindowExtents(HDevWindowStack.GetActive(), -1, -1, hv_Width,
                            hv_Height + 40.576);
                    }
                }
                hv_Row.Dispose(); hv_Column.Dispose(); hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetWindowExtents(hv_WindowHandleLegend, out hv_Row, out hv_Column,
                    out hv_Width, out hv_Height);
                HDevWindowStack.SetActive(hv_WindowHandleLegend);
                if (HDevWindowStack.IsOpen())
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetWindowExtents(HDevWindowStack.GetActive(), -1, -1, hv_Width,
                            hv_Height + 40.576);
                    }
                }
                //
                //Create DLResult dictionary.
                hv_DLResult.Dispose();
                HOperatorSet.CreateDict(out hv_DLResult);
                HOperatorSet.SetDictTuple(hv_DLResult, "classification_class_names", "contamination");
                HOperatorSet.SetDictTuple(hv_DLResult, "classification_class_ids", 0);
                HOperatorSet.SetDictTuple(hv_DLResult, "classification_confidences", 0.98);
                //
                //Create DLSample dictionary.
                hv_DLSample.Dispose();
                HOperatorSet.CreateDict(out hv_DLSample);
                ho_Image.Dispose();
                HOperatorSet.GetDictObject(out ho_Image, hv_ExampleInternals, "example_image_preprocessed");
                HOperatorSet.SetDictObject(ho_Image, hv_DLSample, "image");
                //
                //Create DLDatasetInfo.
                hv_DLDatasetInfo.Dispose();
                HOperatorSet.CreateDict(out hv_DLDatasetInfo);
                HOperatorSet.SetDictTuple(hv_DLDatasetInfo, "class_ids", ((new HTuple(0)).TupleConcat(
                    1)).TupleConcat(2));
                HOperatorSet.SetDictTuple(hv_DLDatasetInfo, "class_names", ((new HTuple("contamination")).TupleConcat(
                    "crack")).TupleConcat("good"));
                //
                //Display sample and result.
                hv_WindowHandleDict.Dispose();
                HOperatorSet.CreateDict(out hv_WindowHandleDict);
                hv_WindowHandleImages.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images", out hv_WindowHandleImages);
                hv_WindowHandleLegend.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_legend", out hv_WindowHandleLegend);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictTuple(hv_WindowHandleDict, "classification_result", hv_WindowHandleImages.TupleConcat(
                        hv_WindowHandleLegend));
                }
                dev_display_dl_data(hv_DLSample, hv_DLResult, hv_DLDatasetInfo, "classification_result",
                    new HTuple(), hv_WindowHandleDict);
                //
                HDevWindowStack.SetActive(hv_WindowHandleImages);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Output of 'apply_dl_model'",
                        "window", "top", "left", "black", new HTuple(), new HTuple());
                }
                //
                ho_Image.Dispose();

                hv_WindowHandleImages.Dispose();
                hv_WindowHandleLegend.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_DLResult.Dispose();
                hv_DLSample.Dispose();
                hv_DLDatasetInfo.Dispose();
                hv_WindowHandleDict.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();

                hv_WindowHandleImages.Dispose();
                hv_WindowHandleLegend.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_DLResult.Dispose();
                hv_DLSample.Dispose();
                hv_DLDatasetInfo.Dispose();
                hv_WindowHandleDict.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_pills_example_dataset_preview()
        {


            // Local iconic variables 

            HObject ho_Image = null, ho_TiledImage = null;
            HObject ho_ImageR = null, ho_ImageG = null, ho_ImageB = null;
            HObject ho_ImageRG = null, ho_ImageRGB = null;

            // Local control variables 

            HTuple hv_GinsengPath = new HTuple(), hv_MagnesiumPath = new HTuple();
            HTuple hv_MintPath = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_Width1 = new HTuple();
            HTuple hv_Height1 = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_ErrorAndAdviceText = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_TiledImage);
            HOperatorSet.GenEmptyObj(out ho_ImageR);
            HOperatorSet.GenEmptyObj(out ho_ImageG);
            HOperatorSet.GenEmptyObj(out ho_ImageB);
            HOperatorSet.GenEmptyObj(out ho_ImageRG);
            HOperatorSet.GenEmptyObj(out ho_ImageRGB);
            try
            {
                //This procedure displays a selection of pill images.
                //
                try
                {
                    //Read some example images.
                    hv_GinsengPath.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_GinsengPath = new HTuple("pill/ginseng/") + (
                            (new HTuple("good/pill_ginseng_good_001")).TupleConcat("contamination/pill_ginseng_contamination_004")).TupleConcat(
                            "crack/pill_ginseng_crack_001");
                    }
                    hv_MagnesiumPath.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MagnesiumPath = new HTuple("pill/magnesium/") + (
                            (new HTuple("good/pill_magnesium_good_001")).TupleConcat("contamination/pill_magnesium_contamination_001")).TupleConcat(
                            "crack/pill_magnesium_crack_001");
                    }
                    hv_MintPath.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MintPath = new HTuple("pill/mint/") + (
                            (new HTuple("good/pill_mint_good_001")).TupleConcat("contamination/pill_mint_contamination_001")).TupleConcat(
                            "crack/pill_mint_crack_009");
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Image.Dispose();
                        HOperatorSet.ReadImage(out ho_Image, ((hv_GinsengPath.TupleConcat(hv_MagnesiumPath))).TupleConcat(
                            hv_MintPath));
                    }
                    ho_TiledImage.Dispose();
                    HOperatorSet.TileImages(ho_Image, out ho_TiledImage, 3, "horizontal");
                    //Generate background image.
                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_TiledImage, out hv_Width, out hv_Height);
                    ho_ImageR.Dispose();
                    HOperatorSet.GenImageProto(ho_TiledImage, out ho_ImageR, 18);
                    ho_ImageG.Dispose();
                    HOperatorSet.GenImageProto(ho_TiledImage, out ho_ImageG, 22);
                    ho_ImageB.Dispose();
                    HOperatorSet.GenImageProto(ho_TiledImage, out ho_ImageB, 28);
                    ho_ImageRG.Dispose();
                    HOperatorSet.AppendChannel(ho_ImageR, ho_ImageG, out ho_ImageRG);
                    ho_ImageRGB.Dispose();
                    HOperatorSet.AppendChannel(ho_ImageRG, ho_ImageB, out ho_ImageRGB);
                    //Display the background and the images.
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetWindowExtents(HDevWindowStack.GetActive(), 360, 0, 800,
                            400);
                    }
                    hv_Width1.Dispose(); hv_Height1.Dispose();
                    HOperatorSet.GetImageSize(ho_ImageRGB, out hv_Width1, out hv_Height1);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetPart(HDevWindowStack.GetActive(), 0, 0, hv_Height1, hv_Width1);
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_ImageRGB, HDevWindowStack.GetActive());
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_TiledImage, HDevWindowStack.GetActive());
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    //If the example image files are not found, an error message is displayed.
                    hv_ErrorAndAdviceText.Dispose();
                    hv_ErrorAndAdviceText = "The images required for this example could not be found.";
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ErrorAndAdviceText = hv_ErrorAndAdviceText.TupleConcat(
                                "");
                            hv_ErrorAndAdviceText.Dispose();
                            hv_ErrorAndAdviceText = ExpTmpLocalVar_ErrorAndAdviceText;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ErrorAndAdviceText = hv_ErrorAndAdviceText.TupleConcat(
                                "These images are part of a separate installer. Please");
                            hv_ErrorAndAdviceText.Dispose();
                            hv_ErrorAndAdviceText = ExpTmpLocalVar_ErrorAndAdviceText;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ErrorAndAdviceText = hv_ErrorAndAdviceText.TupleConcat(
                                "refer to the Installation Guide for more information on");
                            hv_ErrorAndAdviceText.Dispose();
                            hv_ErrorAndAdviceText = ExpTmpLocalVar_ErrorAndAdviceText;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ErrorAndAdviceText = hv_ErrorAndAdviceText.TupleConcat(
                                "this topic!");
                            hv_ErrorAndAdviceText.Dispose();
                            hv_ErrorAndAdviceText = ExpTmpLocalVar_ErrorAndAdviceText;
                        }
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_ErrorAndAdviceText,
                            "window", "center", "left", "red", new HTuple(), new HTuple());
                    }
                }
                //
                ho_Image.Dispose();
                ho_TiledImage.Dispose();
                ho_ImageR.Dispose();
                ho_ImageG.Dispose();
                ho_ImageB.Dispose();
                ho_ImageRG.Dispose();
                ho_ImageRGB.Dispose();

                hv_GinsengPath.Dispose();
                hv_MagnesiumPath.Dispose();
                hv_MintPath.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Width1.Dispose();
                hv_Height1.Dispose();
                hv_Exception.Dispose();
                hv_ErrorAndAdviceText.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();
                ho_TiledImage.Dispose();
                ho_ImageR.Dispose();
                ho_ImageG.Dispose();
                ho_ImageB.Dispose();
                ho_ImageRG.Dispose();
                ho_ImageRGB.Dispose();

                hv_GinsengPath.Dispose();
                hv_MagnesiumPath.Dispose();
                hv_MintPath.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Width1.Dispose();
                hv_Height1.Dispose();
                hv_Exception.Dispose();
                hv_ErrorAndAdviceText.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_preprocessed_image(HTuple hv_ExampleInternals)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Image;

            // Local control variables 

            HTuple hv_PreprocessParamFileName = new HTuple();
            HTuple hv_DLPreprocessParam = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            try
            {
                //This procedure displays an example of a preprocessed image.
                //
                //Read image.
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, "pill/ginseng/contamination/pill_ginseng_contamination_007.png");
                //
                //Preprocess image.
                hv_PreprocessParamFileName.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "preprocess_param_file_name",
                    out hv_PreprocessParamFileName);
                hv_DLPreprocessParam.Dispose();
                HOperatorSet.ReadDict(hv_PreprocessParamFileName, new HTuple(), new HTuple(),
                    out hv_DLPreprocessParam);
                //
                {
                    HObject ExpTmpOutVar_0;
                    preprocess_dl_model_images(ho_Image, out ExpTmpOutVar_0, hv_DLPreprocessParam);
                    ho_Image.Dispose();
                    ho_Image = ExpTmpOutVar_0;
                }
                HOperatorSet.SetDictObject(ho_Image, hv_ExampleInternals, "preprocessed_image");
                //
                //Set preprocessed image to ExampleInternals.
                HOperatorSet.SetDictObject(ho_Image, hv_ExampleInternals, "example_image_preprocessed");
                //
                //Display preprocessed image.
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetPart(HDevWindowStack.GetActive(), 0, 0, hv_Height, hv_Width);
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Preprocessed image",
                        "window", "top", "left", "black", new HTuple(), new HTuple());
                }
                //
                ho_Image.Dispose();

                hv_PreprocessParamFileName.Dispose();
                hv_DLPreprocessParam.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();

                hv_PreprocessParamFileName.Dispose();
                hv_DLPreprocessParam.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_raw_image(HTuple hv_WindowHandleImages)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Image;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Width1 = new HTuple(), hv_Height1 = new HTuple();
            HTuple hv_ZoomFactor = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            try
            {
                //This procedure displays a raw image as inserted into the sample.
                //
                HDevWindowStack.SetActive(hv_WindowHandleImages);
                //Read image and fit the window handle.
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, "pill/ginseng/contamination/pill_ginseng_contamination_007.png");
                //
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                hv_Row.Dispose(); hv_Column.Dispose(); hv_Width1.Dispose(); hv_Height1.Dispose();
                HOperatorSet.GetWindowExtents(hv_WindowHandleImages, out hv_Row, out hv_Column,
                    out hv_Width1, out hv_Height1);
                //
                if ((int)(new HTuple(hv_Height.TupleLess(hv_Width))) != 0)
                {
                    hv_ZoomFactor.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomFactor = hv_Width1 / (hv_Width.TupleReal()
                            );
                    }
                }
                else
                {
                    hv_ZoomFactor.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomFactor = hv_Height1 / (hv_Height.TupleReal()
                            );
                    }
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ZoomImageFactor(ho_Image, out ExpTmpOutVar_0, hv_ZoomFactor, hv_ZoomFactor,
                        "bilinear");
                    ho_Image.Dispose();
                    ho_Image = ExpTmpOutVar_0;
                }
                //
                if (HDevWindowStack.IsOpen())
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetWindowExtents(HDevWindowStack.GetActive(), 360, 0, hv_ZoomFactor * hv_Width,
                            hv_ZoomFactor * hv_Height);
                    }
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetPart(HDevWindowStack.GetActive(), 1, 1, -1, -1);
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Raw image", "window",
                        "top", "left", "black", new HTuple(), new HTuple());
                }
                //
                ho_Image.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Width1.Dispose();
                hv_Height1.Dispose();
                hv_ZoomFactor.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Width1.Dispose();
                hv_Height1.Dispose();
                hv_ZoomFactor.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_screen_example_images(HTuple hv_ExampleInternals)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ShowExampleScreens = new HTuple();
            HTuple hv_ExampleDataDir = new HTuple(), hv_UsePretrainedModel = new HTuple();
            HTuple hv_PreprocessParamFileName = new HTuple(), hv_RetrainedModelFileName = new HTuple();
            HTuple hv_DataDirectory = new HTuple(), hv_PreprocessParamExists = new HTuple();
            HTuple hv_ModelExists = new HTuple(), hv_WindowImageNeeded = new HTuple();
            HTuple hv_WindowHandleText = new HTuple(), hv_Text = new HTuple();
            HTuple hv_WindowHandleImages = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure displays an overview on the different example parts.
                //
                hv_ShowExampleScreens.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "show_example_screens", out hv_ShowExampleScreens);
                if ((int)(hv_ShowExampleScreens.TupleNot()) != 0)
                {

                    hv_ShowExampleScreens.Dispose();
                    hv_ExampleDataDir.Dispose();
                    hv_UsePretrainedModel.Dispose();
                    hv_PreprocessParamFileName.Dispose();
                    hv_RetrainedModelFileName.Dispose();
                    hv_DataDirectory.Dispose();
                    hv_PreprocessParamExists.Dispose();
                    hv_ModelExists.Dispose();
                    hv_WindowImageNeeded.Dispose();
                    hv_WindowHandleText.Dispose();
                    hv_Text.Dispose();
                    hv_WindowHandleImages.Dispose();

                    return;
                }
                //
                //Example data folder containing the outputs of the previous example series.
                hv_ExampleDataDir.Dispose();
                hv_ExampleDataDir = "classify_pill_defects_data";
                //
                //Check if the trained model and preprocessing parameters are available.
                hv_UsePretrainedModel.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "use_pretrained_model", out hv_UsePretrainedModel);
                if ((int)(hv_UsePretrainedModel) != 0)
                {
                    //File name of dict containing parameters used for preprocessing.
                    hv_PreprocessParamFileName.Dispose();
                    hv_PreprocessParamFileName = "classify_pill_defects_preprocess_param.hdict";
                    //File name of dict containing parameters used for preprocessing.
                    hv_RetrainedModelFileName.Dispose();
                    hv_RetrainedModelFileName = "classify_pill_defects.hdl";
                }
                else
                {
                    //File name of dict containing parameters used for preprocessing.
                    hv_DataDirectory.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DataDirectory = hv_ExampleDataDir + "/dldataset_pill_300x300";
                    }
                    hv_PreprocessParamFileName.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_PreprocessParamFileName = hv_DataDirectory + "/dl_preprocess_param.hdict";
                    }
                    //File name of dict containing parameters used for preprocessing.
                    hv_RetrainedModelFileName.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RetrainedModelFileName = hv_ExampleDataDir + "/best_dl_model_classification.hdl";
                    }
                }
                hv_PreprocessParamExists.Dispose();
                HOperatorSet.FileExists(hv_PreprocessParamFileName, out hv_PreprocessParamExists);
                hv_ModelExists.Dispose();
                HOperatorSet.FileExists(hv_RetrainedModelFileName, out hv_ModelExists);
                //
                //Reset the open windows for a clean display.
                hv_WindowImageNeeded.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowImageNeeded = hv_PreprocessParamExists.TupleAnd(
                        hv_ModelExists);
                }
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_needed", hv_WindowImageNeeded);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_legend_needed", 0);
                dev_display_example_reset_windows(hv_ExampleInternals);
                //
                //Display explanatory text.
                hv_WindowHandleText.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_text", out hv_WindowHandleText);
                HDevWindowStack.SetActive(hv_WindowHandleText);
                //
                if ((int)((new HTuple(hv_PreprocessParamExists.TupleNot())).TupleOr(hv_ModelExists.TupleNot()
                    )) != 0)
                {
                    if ((int)(hv_UsePretrainedModel) != 0)
                    {
                        hv_Text.Dispose();
                        hv_Text = "The pretrained model and corresponding preprocessing";
                        if (hv_Text == null)
                            hv_Text = new HTuple();
                        hv_Text[new HTuple(hv_Text.TupleLength())] = "parameters could not be found.";
                        if (hv_Text == null)
                            hv_Text = new HTuple();
                        hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                        if (hv_Text == null)
                            hv_Text = new HTuple();
                        hv_Text[new HTuple(hv_Text.TupleLength())] = "These files are part of a separate installer. Please";
                        if (hv_Text == null)
                            hv_Text = new HTuple();
                        hv_Text[new HTuple(hv_Text.TupleLength())] = "refer to the Installation Guide for more information on";
                        if (hv_Text == null)
                            hv_Text = new HTuple();
                        hv_Text[new HTuple(hv_Text.TupleLength())] = "this topic!";
                    }
                    else
                    {
                        //
                        //Part 1 and/or part 2 should be run before continuing this example.
                        hv_Text.Dispose();
                        hv_Text = "To run this example you need the output of:";
                        if ((int)(hv_PreprocessParamExists.TupleNot()) != 0)
                        {
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "- 'classify_pill_defects_deep_learning_1_prepare.hdev'";
                        }
                        if ((int)(hv_ModelExists.TupleNot()) != 0)
                        {
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "- 'classify_pill_defects_deep_learning_2_train.hdev'";
                        }
                        if (hv_Text == null)
                            hv_Text = new HTuple();
                        hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                        if ((int)((new HTuple(hv_PreprocessParamExists.TupleNot())).TupleAnd(hv_ModelExists.TupleNot()
                            )) != 0)
                        {
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Please run these examples first.";
                        }
                        else
                        {
                            if (hv_Text == null)
                                hv_Text = new HTuple();
                            hv_Text[new HTuple(hv_Text.TupleLength())] = "Please run this example first.";
                        }
                    }
                    //
                    set_display_font(hv_WindowHandleText, 20, "mono", "true", "false");
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window", "top",
                            "left", "red", "box", "true");
                    }
                    set_display_font(hv_WindowHandleText, 16, "mono", "true", "false");
                }
                else
                {
                    //
                    //All parts have been run before, hence continue with the example text.
                    hv_Text.Dispose();
                    hv_Text = "We now have a trained DL classification model.";
                    if (hv_Text == null)
                        hv_Text = new HTuple();
                    hv_Text[new HTuple(hv_Text.TupleLength())] = "We are ready to apply it to new images.";
                    if (hv_Text == null)
                        hv_Text = new HTuple();
                    hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                    if (hv_Text == null)
                        hv_Text = new HTuple();
                    hv_Text[new HTuple(hv_Text.TupleLength())] = "These images are not part of the preprocessed dataset.";
                    if (hv_Text == null)
                        hv_Text = new HTuple();
                    hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                    if (hv_Text == null)
                        hv_Text = new HTuple();
                    hv_Text[new HTuple(hv_Text.TupleLength())] = new HTuple("The images have to be preprocessed in the same way as the DLDataset,");
                    if (hv_Text == null)
                        hv_Text = new HTuple();
                    hv_Text[new HTuple(hv_Text.TupleLength())] = "which was used for training.";
                    if (hv_Text == null)
                        hv_Text = new HTuple();
                    hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                    if (hv_Text == null)
                        hv_Text = new HTuple();
                    hv_Text[new HTuple(hv_Text.TupleLength())] = "Below you see a few example images.";
                    //
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window", "top",
                            "left", "black", "box", "true");
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispText(HDevWindowStack.GetActive(), "Press Run (F5) to continue",
                            "window", "bottom", "right", "black", new HTuple(), new HTuple());
                    }
                    //
                    //Add proprocessing parameters to ExampleInternals.
                    HOperatorSet.SetDictTuple(hv_ExampleInternals, "preprocess_param_file_name",
                        hv_PreprocessParamFileName);
                    //
                    //Display an example image
                    hv_WindowHandleImages.Dispose();
                    HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images", out hv_WindowHandleImages);
                    HDevWindowStack.SetActive(hv_WindowHandleImages);
                    dev_display_pills_example_dataset_preview();
                }
                //

                hv_ShowExampleScreens.Dispose();
                hv_ExampleDataDir.Dispose();
                hv_UsePretrainedModel.Dispose();
                hv_PreprocessParamFileName.Dispose();
                hv_RetrainedModelFileName.Dispose();
                hv_DataDirectory.Dispose();
                hv_PreprocessParamExists.Dispose();
                hv_ModelExists.Dispose();
                hv_WindowImageNeeded.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();
                hv_WindowHandleImages.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_ShowExampleScreens.Dispose();
                hv_ExampleDataDir.Dispose();
                hv_UsePretrainedModel.Dispose();
                hv_PreprocessParamFileName.Dispose();
                hv_RetrainedModelFileName.Dispose();
                hv_DataDirectory.Dispose();
                hv_PreprocessParamExists.Dispose();
                hv_ModelExists.Dispose();
                hv_WindowImageNeeded.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();
                hv_WindowHandleImages.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_screen_final(HTuple hv_ExampleInternals)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ShowExampleScreens = new HTuple();
            HTuple hv_WindowHandleText = new HTuple(), hv_Text = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure shows the final message of the example series.
                //
                hv_ShowExampleScreens.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "show_example_screens", out hv_ShowExampleScreens);
                if ((int)(hv_ShowExampleScreens.TupleNot()) != 0)
                {

                    hv_ShowExampleScreens.Dispose();
                    hv_WindowHandleText.Dispose();
                    hv_Text.Dispose();

                    return;
                }
                //
                dev_open_example_text_window(hv_ExampleInternals);
                //
                //Reset the open windows for a clean display.
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_needed", 0);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_legend_needed", 0);
                dev_display_example_reset_windows(hv_ExampleInternals);
                //
                hv_WindowHandleText.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_text", out hv_WindowHandleText);
                HDevWindowStack.SetActive(hv_WindowHandleText);
                //
                //Display instruction text.
                hv_Text.Dispose();
                hv_Text = "Congratulations!";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "You have finished the series of examples for DL classification.";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "You can now train a classification model on your own data.";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                //
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window", "top",
                        "left", "black", "box", "true");
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "End of program.", "window",
                        "bottom", "right", "black", "box", "true");
                }
                //

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_screen_inference_step_1(HTuple hv_ExampleInternals)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ShowExampleScreens = new HTuple();
            HTuple hv_WindowHandleText = new HTuple(), hv_Text = new HTuple();
            HTuple hv_WindowHandleImages = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure displays the second explanatory part of the inference.
                //
                hv_ShowExampleScreens.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "show_example_screens", out hv_ShowExampleScreens);
                if ((int)(hv_ShowExampleScreens.TupleNot()) != 0)
                {

                    hv_ShowExampleScreens.Dispose();
                    hv_WindowHandleText.Dispose();
                    hv_Text.Dispose();
                    hv_WindowHandleImages.Dispose();

                    return;
                }
                //
                //Reset the open windows for a clean display.
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_needed", 1);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_legend_needed", 0);
                dev_display_example_reset_windows(hv_ExampleInternals);
                //
                //Display explanatory text.
                hv_WindowHandleText.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_text", out hv_WindowHandleText);
                HDevWindowStack.SetActive(hv_WindowHandleText);
                //
                hv_Text.Dispose();
                hv_Text = "Inference steps for one image:";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "1. Generate a DLSample for the image using";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "   'gen_dl_samples_from_images'.";
                //
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window", "top",
                        "left", "black", "box", "true");
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Press Run (F5) to continue",
                        "window", "bottom", "right", "black", new HTuple(), new HTuple());
                }
                //
                //Display raw image.
                hv_WindowHandleImages.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images", out hv_WindowHandleImages);
                dev_display_raw_image(hv_WindowHandleImages);
                //

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();
                hv_WindowHandleImages.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();
                hv_WindowHandleImages.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_screen_inference_step_2(HTuple hv_ExampleInternals)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ShowExampleScreens = new HTuple();
            HTuple hv_WindowHandleText = new HTuple(), hv_Text = new HTuple();
            HTuple hv_WindowHandleImages = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure displays the second explanatory part of the inference.
                //
                hv_ShowExampleScreens.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "show_example_screens", out hv_ShowExampleScreens);
                if ((int)(hv_ShowExampleScreens.TupleNot()) != 0)
                {

                    hv_ShowExampleScreens.Dispose();
                    hv_WindowHandleText.Dispose();
                    hv_Text.Dispose();
                    hv_WindowHandleImages.Dispose();

                    return;
                }
                //
                //Reset the open windows for a clean display.
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_needed", 1);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_legend_needed", 0);
                dev_display_example_reset_windows(hv_ExampleInternals);
                //
                //Display explanatory text.
                hv_WindowHandleText.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_text", out hv_WindowHandleText);
                HDevWindowStack.SetActive(hv_WindowHandleText);
                //
                hv_Text.Dispose();
                hv_Text = "Inference steps for one image:";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "1. Generate a DLSample for the image using";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "   'gen_dl_samples_from_images'.";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "2. Preprocess the image to suit the trained model";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "   using 'preprocess_dl_samples'.";
                //
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window", "top",
                        "left", "black", "box", "true");
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Press Run (F5) to continue",
                        "window", "bottom", "right", "black", new HTuple(), new HTuple());
                }
                //
                //Display example images.
                hv_WindowHandleImages.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images", out hv_WindowHandleImages);
                HDevWindowStack.SetActive(hv_WindowHandleImages);
                dev_display_preprocessed_image(hv_ExampleInternals);
                //

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();
                hv_WindowHandleImages.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();
                hv_WindowHandleImages.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_screen_inference_step_3(HTuple hv_ExampleInternals)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ShowExampleScreens = new HTuple();
            HTuple hv_WindowHandleText = new HTuple(), hv_Text = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure displays the third explanatory part of the inference.
                //
                hv_ShowExampleScreens.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "show_example_screens", out hv_ShowExampleScreens);
                if ((int)(hv_ShowExampleScreens.TupleNot()) != 0)
                {

                    hv_ShowExampleScreens.Dispose();
                    hv_WindowHandleText.Dispose();
                    hv_Text.Dispose();

                    return;
                }
                //
                //Reset the open windows for a clean display.
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_needed", 1);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_legend_needed", 1);
                dev_display_example_reset_windows(hv_ExampleInternals);
                //
                //Display explanatory text.
                hv_WindowHandleText.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_text", out hv_WindowHandleText);
                HDevWindowStack.SetActive(hv_WindowHandleText);
                //
                hv_Text.Dispose();
                hv_Text = "Inference steps for one image:";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "1. Generate a DLSample for the image using";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "   'gen_dl_samples_from_images'.";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "2. Preprocess the image to fit the trained model";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "   using 'preprocess_dl_samples'.";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "3. Apply the model using 'apply_dl_model'.";
                //
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window", "top",
                        "left", "black", "box", "true");
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Press Run (F5) to continue",
                        "window", "bottom", "right", "black", new HTuple(), new HTuple());
                }
                //
                //Display example images,
                dev_display_model_output_image(hv_ExampleInternals);
                //

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_screen_introduction(HTuple hv_ExampleInternals)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ShowExampleScreens = new HTuple();
            HTuple hv_WindowHandleText = new HTuple(), hv_Text = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure displays an overview on the different example parts.
                //
                hv_ShowExampleScreens.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "show_example_screens", out hv_ShowExampleScreens);
                if ((int)(hv_ShowExampleScreens.TupleNot()) != 0)
                {

                    hv_ShowExampleScreens.Dispose();
                    hv_WindowHandleText.Dispose();
                    hv_Text.Dispose();

                    return;
                }
                //
                //Reset the open windows for a clean display.
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_needed", 0);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_legend_needed", 0);
                dev_display_example_reset_windows(hv_ExampleInternals);
                //
                //Display explanatory text.
                hv_WindowHandleText.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_text", out hv_WindowHandleText);
                HDevWindowStack.SetActive(hv_WindowHandleText);
                //
                hv_Text.Dispose();
                hv_Text = new HTuple("This example is part of a series of examples, which summarize ");
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "the workflow for DL classification. It uses the MVTec pill dataset.";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "The four parts are: ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "1. Dataset preprocessing.";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "2. Training of the model.";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "3. Evaluation of the trained model.";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "4. Inference on new images.";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "This example covers part 4: 'Inference on new images'.";
                //
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window", "top",
                        "left", "black", "box", "true");
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Press Run (F5) to continue",
                        "window", "bottom", "right", "black", new HTuple(), new HTuple());
                }
                //

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_display_screen_run_program(HTuple hv_ExampleInternals)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ShowExampleScreens = new HTuple();
            HTuple hv_WindowHandleText = new HTuple(), hv_Text = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure shows a final text before running the program.
                //
                hv_ShowExampleScreens.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "show_example_screens", out hv_ShowExampleScreens);
                if ((int)(hv_ShowExampleScreens.TupleNot()) != 0)
                {

                    hv_ShowExampleScreens.Dispose();
                    hv_WindowHandleText.Dispose();
                    hv_Text.Dispose();

                    return;
                }
                //
                //Reset the open windows for a clean display.
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_needed", 0);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_legend_needed", 0);
                dev_display_example_reset_windows(hv_ExampleInternals);
                //
                //Display explanatory text.
                hv_WindowHandleText.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_text", out hv_WindowHandleText);
                HDevWindowStack.SetActive(hv_WindowHandleText);
                //
                hv_Text.Dispose();
                hv_Text = "We will now apply the trained model from example part 2";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "'classify_pill_defects_deep_learning_2_train.hdev'";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "to some new images using 'apply_dl_model'.";
                //
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window", "top",
                        "left", "black", "box", "true");
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Press Run (F5) to continue",
                        "window", "bottom", "right", "black", new HTuple(), new HTuple());
                }
                //

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_ShowExampleScreens.Dispose();
                hv_WindowHandleText.Dispose();
                hv_Text.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_example_init(HTuple hv_ShowExampleScreens, HTuple hv_UsePretrainedModel,
            out HTuple hv_ExampleInternals)
        {


            // Initialize local and output iconic variables 
            hv_ExampleInternals = new HTuple();
            //This procedure initializes the graphic windows that are used for explanations during the example.
            //
            //A dict that will be used/adapted by other example procedures.
            hv_ExampleInternals.Dispose();
            HOperatorSet.CreateDict(out hv_ExampleInternals);
            HOperatorSet.SetDictTuple(hv_ExampleInternals, "show_example_screens", hv_ShowExampleScreens);
            HOperatorSet.SetDictTuple(hv_ExampleInternals, "use_pretrained_model", hv_UsePretrainedModel);
            if ((int)(hv_ShowExampleScreens.TupleNot()) != 0)
            {


                return;
            }
            //
            if (HDevWindowStack.IsOpen())
            {
                HOperatorSet.CloseWindow(HDevWindowStack.Pop());
            }
            dev_open_example_text_window(hv_ExampleInternals);
            //
            HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_needed", 0);
            HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_legend_needed", 0);
            //


            return;
        }

        public void dev_open_example_image_window(HTuple hv_ExampleInternals)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_WindowHeightText = new HTuple();
            HTuple hv_WindowWidthImage = new HTuple(), hv_WindowHeightImages = new HTuple();
            HTuple hv_WindowBGColor = new HTuple(), hv_WindowYImages = new HTuple();
            HTuple hv_WindowXImages = new HTuple(), hv_WindowHandleImages = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure initializes the graphic windows that are used to display example images.
                //
                hv_WindowHeightText.Dispose();
                hv_WindowHeightText = 300;
                hv_WindowWidthImage.Dispose();
                hv_WindowWidthImage = 500;
                hv_WindowHeightImages.Dispose();
                hv_WindowHeightImages = 500;
                hv_WindowBGColor.Dispose();
                hv_WindowBGColor = "black";
                //
                hv_WindowYImages.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowYImages = hv_WindowHeightText + 60;
                }
                hv_WindowXImages.Dispose();
                hv_WindowXImages = 0;
                HOperatorSet.SetWindowAttr("background_color", hv_WindowBGColor);
                HOperatorSet.OpenWindow(hv_WindowYImages, hv_WindowXImages, hv_WindowWidthImage, hv_WindowHeightImages, 0, "visible", "", out hv_WindowHandleImages);
                HDevWindowStack.Push(hv_WindowHandleImages);
                set_display_font(hv_WindowHandleImages, 16, "mono", "true", "false");
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images", hv_WindowHandleImages);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_width", hv_WindowWidthImage);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_height", hv_WindowHeightImages);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_x", hv_WindowXImages);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_images_y", hv_WindowYImages);
                //

                hv_WindowHeightText.Dispose();
                hv_WindowWidthImage.Dispose();
                hv_WindowHeightImages.Dispose();
                hv_WindowBGColor.Dispose();
                hv_WindowYImages.Dispose();
                hv_WindowXImages.Dispose();
                hv_WindowHandleImages.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_WindowHeightText.Dispose();
                hv_WindowWidthImage.Dispose();
                hv_WindowHeightImages.Dispose();
                hv_WindowBGColor.Dispose();
                hv_WindowYImages.Dispose();
                hv_WindowXImages.Dispose();
                hv_WindowHandleImages.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_open_example_legend_window(HTuple hv_ExampleInternals, HTuple hv_WindowWidth)
        {



            // Local control variables 

            HTuple hv_WindowImagesHeight = new HTuple();
            HTuple hv_WindowImagesWidth = new HTuple(), hv_WindowImagesX = new HTuple();
            HTuple hv_WindowImagesY = new HTuple(), hv_WindowHandleLegend = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure initializes the graphic windows that are used to display a legend.

                hv_WindowImagesHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images_height", out hv_WindowImagesHeight);
                hv_WindowImagesWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images_width", out hv_WindowImagesWidth);
                hv_WindowImagesX.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images_x", out hv_WindowImagesX);
                hv_WindowImagesY.Dispose();
                HOperatorSet.GetDictTuple(hv_ExampleInternals, "window_images_y", out hv_WindowImagesY);
                HOperatorSet.SetWindowAttr("background_color", "black");
                HOperatorSet.OpenWindow(hv_WindowImagesY, (hv_WindowImagesX + hv_WindowImagesWidth) + 5, hv_WindowWidth, hv_WindowImagesHeight, 0, "visible", "", out hv_WindowHandleLegend);
                HDevWindowStack.Push(hv_WindowHandleLegend);
                set_display_font(hv_WindowHandleLegend, 14, "mono", "true", "false");
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_legend", hv_WindowHandleLegend);

                hv_WindowImagesHeight.Dispose();
                hv_WindowImagesWidth.Dispose();
                hv_WindowImagesX.Dispose();
                hv_WindowImagesY.Dispose();
                hv_WindowHandleLegend.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_WindowImagesHeight.Dispose();
                hv_WindowImagesWidth.Dispose();
                hv_WindowImagesX.Dispose();
                hv_WindowImagesY.Dispose();
                hv_WindowHandleLegend.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void dev_open_example_text_window(HTuple hv_ExampleInternals)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_WindowWidthText = new HTuple(), hv_WindowHeightText = new HTuple();
            HTuple hv_WindowBGColor = new HTuple(), hv_WindowHandleText = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                hv_WindowWidthText.Dispose();
                hv_WindowWidthText = 800;
                hv_WindowHeightText.Dispose();
                hv_WindowHeightText = 300;
                hv_WindowBGColor.Dispose();
                hv_WindowBGColor = "gray";
                HOperatorSet.SetWindowAttr("background_color", hv_WindowBGColor);
                HOperatorSet.OpenWindow(0, 0, hv_WindowWidthText, hv_WindowHeightText, 0, "visible", "", out hv_WindowHandleText);
                HDevWindowStack.Push(hv_WindowHandleText);
                set_display_font(hv_WindowHandleText, 16, "mono", "true", "false");
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_text", hv_WindowHandleText);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_text_width", hv_WindowWidthText);
                HOperatorSet.SetDictTuple(hv_ExampleInternals, "window_text_height", hv_WindowHeightText);
                //

                hv_WindowWidthText.Dispose();
                hv_WindowHeightText.Dispose();
                hv_WindowBGColor.Dispose();
                hv_WindowHandleText.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_WindowWidthText.Dispose();
                hv_WindowHeightText.Dispose();
                hv_WindowBGColor.Dispose();
                hv_WindowHandleText.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void get_example_inference_images(HTuple hv_ImageDir, out HTuple hv_ImageFiles)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_NumSamples = new HTuple(), hv_ImageFilesAll = new HTuple();
            // Initialize local and output iconic variables 
            hv_ImageFiles = new HTuple();
            try
            {
                //This procedure creates a list of images used for this example.
                //
                hv_NumSamples.Dispose();
                hv_NumSamples = 10;
                hv_ImageFilesAll.Dispose();
                list_image_files(hv_ImageDir, "default", "recursive", out hv_ImageFilesAll);
                {
                    HTuple ExpTmpOutVar_0;
                    tuple_shuffle(hv_ImageFilesAll, out ExpTmpOutVar_0);
                    hv_ImageFilesAll.Dispose();
                    hv_ImageFilesAll = ExpTmpOutVar_0;
                }
                hv_ImageFiles.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageFiles = hv_ImageFilesAll.TupleSelectRange(
                        0, hv_NumSamples - 1);
                }
                //

                hv_NumSamples.Dispose();
                hv_ImageFilesAll.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_NumSamples.Dispose();
                hv_ImageFilesAll.Dispose();

                throw HDevExpDefaultException;
            }
        }

        #region DispImageAdaptively(自适应显示图像,不会改变图像长宽比)
        /// <summary>
        /// 自适应显示图像,不会改变图像长宽比
        /// </summary>
        public void DispImageAdaptively(ref HWindowControl hWindowControl, HObject ho_Image)
        {

            HOperatorSet.ClearWindow(hWindowControl.HalconWindow);

            int mW = hWindowControl.Width;

            int mH = hWindowControl.Height;

            HTuple hv_width = new HTuple(); HTuple hv_height = new HTuple();

            HOperatorSet.GetImageSize(ho_Image, out hv_width, out hv_height);

            int imageW = Convert.ToInt32(hv_width.ToString());

            int imageH = Convert.ToInt32(hv_height.ToString());

            if (mW > 0 && mH > 0)
            {

                double mScale_Window = Convert.ToDouble(mW) / Convert.ToDouble(mH);

                double mScale_Image = Convert.ToDouble(imageW) / Convert.ToDouble(imageH);

                double row1, column1, row2, column2;

                int mH_1 = Convert.ToInt32(mW / mScale_Image);

                System.Drawing.Rectangle rect = hWindowControl.ImagePart;



                if (mH_1 > mH)
                {

                    row1 = 0;

                    row2 = imageH;

                    double mImage_w = imageH * mScale_Window - imageW;

                    double mD_Image_W = Math.Abs(mImage_w / 2.0);

                    column1 = mD_Image_W;

                    column2 = imageW + mD_Image_W;



                    rect.X = -(int)Math.Round(mD_Image_W);

                    rect.Y = 0;

                    rect.Height = imageH;

                    rect.Width = (int)Math.Round(imageH * mScale_Window);

                }

                else
                {

                    column1 = 0;

                    column2 = imageW;

                    double mImage_h = Convert.ToDouble(imageW) / mScale_Window - imageH;

                    double mD_Image_H = Math.Abs(mImage_h / 2.0);

                    row1 = mD_Image_H;

                    row2 = imageH + mD_Image_H;



                    rect.X = 0;

                    rect.Y = -(int)Math.Round(mD_Image_H);

                    rect.Height = (int)Math.Round(Convert.ToDouble(imageW) / mScale_Window);

                    rect.Width = imageW;

                }

                hWindowControl.ImagePart = rect;

            }

            HOperatorSet.DispObj(ho_Image, hWindowControl.HalconWindow);

        }
        #endregion
    }
}
