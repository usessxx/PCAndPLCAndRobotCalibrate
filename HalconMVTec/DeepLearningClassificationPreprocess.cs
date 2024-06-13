using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace HalconMVTec
{
    public class DeepLearningClassificationPreprocess
    {
        // Local control variables
        HTuple hv_PreprocessResultDirectoryPath = new HTuple();//存储预处理结果-sample文件夹 及 hdict文件的上一级文件夹路径(包含文件夹名)
        HTuple hv_PreprocessResultFileName = new HTuple();//预处理结果文件名(以".hdict"为后缀,例如:dl_preprocess_dataset.hdict)
        HTuple hv_PreprocessResultParameterFileName = new HTuple();//预处理结果-参数的文件名(以".hdict"为后缀,例如:dl_preprocess_param.hdict)

        HTuple hv_LableDataSetFilePath = new HTuple();//读取的标注文件全路径(包含此文件名以".hdict"为后缀)

        HTuple hv_TrainingPercent = new HTuple();//训练集百分比
        HTuple hv_ValidationPercent = new HTuple();//验证集百分比
        HTuple hv_ImageWidth = new HTuple();//图片宽度(像素值)
        HTuple hv_ImageHeight = new HTuple();//图片高度(像素值)
        HTuple hv_ImageNumChannels = new HTuple();//图片通道数
        HTuple hv_RandomSeed = new HTuple();//随机种子
        HTuple hv_NormalizationType = new HTuple();
        HTuple hv_DomainHandling = new HTuple();
        
        HTuple hv_DictHandle = new HTuple();
        //HTuple hv_FileExists = new HTuple();
        HTuple hv_DLPreprocessParam = new HTuple();
        HTuple hv_GenParam = new HTuple();
        HTuple hv_DLDatasetFileName = new HTuple();
        //HTuple hv_DatasetSamples = new HTuple();
        //HTuple hv_SampleIndices = new HTuple();
        //HTuple hv_ShuffledIndices = new HTuple();
        //HTuple hv_DLSampleBatchDisplay = new HTuple();
        //HTuple hv_WindowHandleDict = new HTuple();
        //HTuple hv_Index = new HTuple();
        //HTuple hv_Text = new HTuple();

        //ADD FOR DISPLAY
        public HWindowControl hv_HWindowControl = new HWindowControl();//显示用窗口控件
        int loopCount = 0;//当前处理图片计数
        int imageCount = 0;//需要预处理的图片计数
        HTuple hv_Progress_ForDisplay = new HTuple();//存储要显示的信息

        #region DeepLearningPreprocessClassification(构造函数里对预处理参数初始化)
        //构造函数里对预处理参数初始化
        public DeepLearningClassificationPreprocess()
        {
            //In order to get a reproducible split we set a random seed.
            //This means that re-running the script results in the same split of DLDataset.
            hv_RandomSeed = 42;

            //Percentages for splitting the dataset.
            hv_TrainingPercent = 70;
            hv_ValidationPercent = 15;

            //Image dimensions the images are rescaled to during preprocessing.
            hv_ImageWidth = 580;
            hv_ImageHeight = 384;
            hv_ImageNumChannels = 1;

            //Further parameters for image preprocessing.
            hv_NormalizationType = "none";
            hv_DomainHandling = "full_domain";
        }
        #endregion

        #region PreprocessAction(预处理开始)
        public void PreprocessAction(string lableDataSetFilePath, string preprocessResultDirectoryPath, string preprocessResultFileName, string preprocessResultParameterFileName, int imageWidth, int imageHeight, int imageNumChannels, int trainingPercent, int validationPercent, int RandomSeed)          
        {
            hv_LableDataSetFilePath = lableDataSetFilePath;
            hv_PreprocessResultDirectoryPath = preprocessResultDirectoryPath;
            hv_PreprocessResultParameterFileName = preprocessResultParameterFileName;
            hv_PreprocessResultFileName = preprocessResultFileName;
            hv_ImageWidth = imageWidth;
            hv_ImageHeight = imageHeight;
            hv_ImageNumChannels = imageNumChannels;
            hv_TrainingPercent = trainingPercent;
            hv_ValidationPercent = validationPercent;
            hv_RandomSeed = RandomSeed;

            //读取标注数据集并分割为训练集,验证集,测试集 
            LabelDataSetSplit();

            //创建预处理参数
            create_dl_preprocess_param("classification", hv_ImageWidth, hv_ImageHeight, hv_ImageNumChannels, -127, 128, hv_NormalizationType, hv_DomainHandling, new HTuple(), new HTuple(), new HTuple(), new HTuple(), out hv_DLPreprocessParam);           

            //Preprocess the dataset. This might take a few seconds.
            HOperatorSet.CreateDict(out hv_GenParam);
            HOperatorSet.SetDictTuple(hv_GenParam, "overwrite_files", 1);

            //开始预处理
            preprocess_dl_dataset(hv_DictHandle, hv_PreprocessResultDirectoryPath, hv_DLPreprocessParam, hv_GenParam, out hv_DLDatasetFileName);
      
            //Store preprocess params separately in order to use it e.g. during inference.
            //存储预处理结果-参数,以便在推理过程中使用它
            HOperatorSet.WriteDict(hv_DLPreprocessParam, hv_PreprocessResultDirectoryPath + @"\" + hv_PreprocessResultParameterFileName, new HTuple(), new HTuple());          
        }
        #endregion

        #region LabelDataSetSplit(读取标注数据集并分割为训练集,验证集,测试集)
        //Read the labeled data and split it into train, validation and test
        //读取标注数据集并分割为训练集,验证集,测试集 
        public void LabelDataSetSplit()
        {
            //Set the random seed.
            HOperatorSet.SetSystem("seed_rand", hv_RandomSeed);

            //Read the dataset with the procedure read_dl_dataset_classification.
            //Alternatively, you can read a DLDataset dictionary
            //as created by e.g., the MVTec Deep Learning Tool using read_dict().
            HOperatorSet.ReadDict(hv_LableDataSetFilePath, new HTuple(), new HTuple(), out hv_DictHandle);

            //Generate the split.
            split_dl_dataset(hv_DictHandle, hv_TrainingPercent, hv_ValidationPercent, new HTuple());
        }
        #endregion

        #region split_dl_dataset(分割数据集)
        // Chapter: Deep Learning / Model
        // Short Description: Split the samples into training, validation, and test subsets. 
        public void split_dl_dataset(HTuple hv_DLDataset, HTuple hv_TrainingPercent, HTuple hv_ValidationPercent,
            HTuple hv_GenParam)
        {
            // Local iconic variables 

            HObject ho_SegmImage = null;

            // Local control variables 

            HTuple hv_OverwriteSplit = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_SplitNames = new HTuple(), hv_GenParamName = new HTuple();
            HTuple hv_GenParamIndex = new HTuple(), hv_Type = new HTuple();
            HTuple hv_DLSamples = new HTuple(), hv_DLSample = new HTuple();
            HTuple hv_AnomalyDetectionLabelExists = new HTuple(), hv_BBoxLabelIdExists = new HTuple();
            HTuple hv_ImageLabelIdExists = new HTuple(), hv_SegmFileExists = new HTuple();
            HTuple hv_ClassIDs = new HTuple(), hv_ClassNames = new HTuple();
            HTuple hv_ClassIDToClassIndex = new HTuple(), hv_TrainingRatio = new HTuple();
            HTuple hv_ValidationRatio = new HTuple(), hv_SplitRatios = new HTuple();
            HTuple hv_SplitRatiosInvSortIndices = new HTuple(), hv_SplitKeys = new HTuple();
            HTuple hv_IndexSample = new HTuple(), hv_SplitExists = new HTuple();
            HTuple hv_SampleSplit = new HTuple(), hv_NotYetSplit = new HTuple();
            HTuple hv_ImageIDsPerClass = new HTuple(), hv_ClassIndex = new HTuple();
            HTuple hv_NumImagesPerClass = new HTuple(), hv_ImageIDList = new HTuple();
            HTuple hv_ImageID = new HTuple(), hv_AnomalyLabel = new HTuple();
            HTuple hv_Labels = new HTuple(), hv_ImageLabelID = new HTuple();
            HTuple hv_BboxLabels = new HTuple(), hv_SegmDir = new HTuple();
            HTuple hv_SegmFileName = new HTuple(), hv_AbsoluteHisto = new HTuple();
            HTuple hv_LabelIndices = new HTuple(), hv_Index = new HTuple();
            HTuple hv_ImgIDsClass = new HTuple(), hv_SplitImageIDs = new HTuple();
            HTuple hv_AssignedImageIDs = new HTuple(), hv_ClassSortIndices = new HTuple();
            HTuple hv_ImageIDsClass = new HTuple(), hv_ImageIDsClassToBeAssigned = new HTuple();
            HTuple hv_SplitIndex = new HTuple(), hv_NumToBeAssignedToThisSplit = new HTuple();
            HTuple hv_AssignedImageIDsToThisSplit = new HTuple(), hv_NumAlreadyAssignedToThisSplit = new HTuple();
            HTuple hv_NumStillToBeAssigned = new HTuple(), hv_ImageIndex = new HTuple();
            HTuple hv_Rand = new HTuple(), hv_RatioIndex = new HTuple();
            HTuple hv_CurrentSplitIndex = new HTuple(), hv_CurrentSplitRatio = new HTuple();
            HTuple hv_ImageIDsWithoutLabel = new HTuple(), hv_NumImageIDsWithoutLabel = new HTuple();
            HTuple hv_NumToBeAssigned = new HTuple(), hv_MaxRatioIndex = new HTuple();
            HTuple hv_SplitNameIndex = new HTuple(), hv_SplitName = new HTuple();
            HTuple hv_SplitIDs = new HTuple(), hv_SplitIDIndex = new HTuple();
            HTuple hv_SampleSplitIDs = new HTuple(), hv_DLSampleIndex = new HTuple();
            HTuple hv_ErrorDict = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SegmImage);
            try
            {
                //
                //This procedure divides the samples in DLDataset
                //into three disjoint subsets: train, validation, and test.
                //The number of samples in each subset is defined
                //by the given percentages TrainingPercent and ValidationPercent.
                //As a result, every sample has a new key named 'split'
                //with an associated value 'train', 'validation', or 'test'.
                //Thereby the classes of every image are taken
                //into consideration, in order to avoid accidental predominance
                //of certain classes in one of the subsets. In the case of a
                //dataset used for anomaly detection, all images of type 'nok'
                //are sorted into the 'test' split.
                //
                //Check input data.
                if ((int)(new HTuple(hv_TrainingPercent.TupleLess(0))) != 0)
                {
                    throw new HalconException("TrainingPercent must not be smaller than zero.");
                }
                if ((int)(new HTuple(hv_ValidationPercent.TupleLess(0))) != 0)
                {
                    throw new HalconException("ValidationPercent must not be smaller than zero.");
                }
                if ((int)(new HTuple(((hv_TrainingPercent + hv_ValidationPercent)).TupleGreater(
                    100))) != 0)
                {
                    throw new HalconException("The sum of TrainingPercent and ValidationPercent must not be greater than 100.");
                }
                //
                //** Set the default values ***
                //
                //Overwrite an existing split?
                hv_OverwriteSplit.Dispose();
                hv_OverwriteSplit = 0;
                //Initialize model_type of the DLDataset.
                hv_ModelType.Dispose();
                hv_ModelType = "";
                //Names for split subsets.
                hv_SplitNames.Dispose();
                hv_SplitNames = new HTuple();
                hv_SplitNames[0] = "train";
                hv_SplitNames[1] = "validation";
                hv_SplitNames[2] = "test";
                //
                //Get input for generic parameters.
                if ((int)(new HTuple(hv_GenParam.TupleNotEqual(new HTuple()))) != 0)
                {
                    hv_GenParamName.Dispose();
                    HOperatorSet.GetDictParam(hv_GenParam, "keys", new HTuple(), out hv_GenParamName);
                    for (hv_GenParamIndex = 0; (int)hv_GenParamIndex <= (int)((new HTuple(hv_GenParamName.TupleLength()
                        )) - 1); hv_GenParamIndex = (int)hv_GenParamIndex + 1)
                    {
                        if ((int)(new HTuple(((hv_GenParamName.TupleSelect(hv_GenParamIndex))).TupleEqual(
                            "overwrite_split"))) != 0)
                        {
                            hv_OverwriteSplit.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, "overwrite_split", out hv_OverwriteSplit);
                            hv_Type.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Type = hv_OverwriteSplit.TupleType()
                                    ;
                            }
                            if ((int)((new HTuple(hv_Type.TupleEqual(4))).TupleAnd(new HTuple(hv_OverwriteSplit.TupleEqual(
                                "true")))) != 0)
                            {
                                hv_OverwriteSplit.Dispose();
                                hv_OverwriteSplit = 1;
                            }
                            if ((int)((new HTuple(hv_Type.TupleEqual(4))).TupleAnd(new HTuple(hv_OverwriteSplit.TupleEqual(
                                "false")))) != 0)
                            {
                                hv_OverwriteSplit.Dispose();
                                hv_OverwriteSplit = 0;
                            }
                        }
                        else if ((int)(new HTuple(((hv_GenParamName.TupleSelect(hv_GenParamIndex))).TupleEqual(
                            "model_type"))) != 0)
                        {
                            hv_ModelType.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, "model_type", out hv_ModelType);
                        }
                        else
                        {
                            throw new HalconException("Unknown GenParam entry: " + (hv_GenParamName.TupleSelect(
                                hv_GenParamIndex)));
                        }
                    }
                }
                //
                //Try to guess the ModelType if not set by GenParam.
                if ((int)(new HTuple(hv_ModelType.TupleEqual(""))) != 0)
                {
                    hv_DLSamples.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLDataset, "samples", out hv_DLSamples);
                    hv_DLSample.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DLSample = hv_DLSamples.TupleSelect(
                            0);
                    }
                    //Check for relevant keys.
                    hv_AnomalyDetectionLabelExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "anomaly_label", out hv_AnomalyDetectionLabelExists);
                    hv_BBoxLabelIdExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "bbox_label_id", out hv_BBoxLabelIdExists);
                    hv_ImageLabelIdExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "image_label_id", out hv_ImageLabelIdExists);
                    hv_SegmFileExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "segmentation_file_name",
                        out hv_SegmFileExists);
                    //
                    if ((int)(hv_AnomalyDetectionLabelExists) != 0)
                    {
                        hv_ModelType.Dispose();
                        hv_ModelType = "anomaly_detection";
                    }
                    else if ((int)(hv_ImageLabelIdExists) != 0)
                    {
                        hv_ModelType.Dispose();
                        hv_ModelType = "classification";
                    }
                    else if ((int)(hv_BBoxLabelIdExists) != 0)
                    {
                        hv_ModelType.Dispose();
                        hv_ModelType = "detection";
                    }
                    else if ((int)(hv_SegmFileExists) != 0)
                    {
                        hv_ModelType.Dispose();
                        hv_ModelType = "segmentation";
                    }
                    else
                    {
                        throw new HalconException("Parameter 'model_type' cannot be determined.");
                    }
                }
                //
                //Get data from DLDataset.
                if ((int)(new HTuple(hv_ModelType.TupleNotEqual("anomaly_detection"))) != 0)
                {
                    hv_ClassIDs.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLDataset, "class_ids", out hv_ClassIDs);
                    hv_ClassNames.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLDataset, "class_names", out hv_ClassNames);
                }
                else
                {
                    hv_ClassIDs.Dispose();
                    hv_ClassIDs = new HTuple();
                    hv_ClassIDs[0] = 0;
                    hv_ClassIDs[1] = 1;
                    hv_ClassNames.Dispose();
                    hv_ClassNames = new HTuple();
                    hv_ClassNames[0] = "ok";
                    hv_ClassNames[1] = "nok";
                }
                hv_ClassIDToClassIndex.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ClassIDToClassIndex = HTuple.TupleGenConst(
                        (hv_ClassIDs.TupleMax()) + 1, -1);
                }
                if (hv_ClassIDToClassIndex == null)
                    hv_ClassIDToClassIndex = new HTuple();
                hv_ClassIDToClassIndex[hv_ClassIDs] = HTuple.TupleGenSequence(0, (new HTuple(hv_ClassIDs.TupleLength()
                    )) - 1, 1);
                //
                //Calculate ratios of training and validation datasets.
                hv_TrainingRatio.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TrainingRatio = hv_TrainingPercent * 0.01;
                }
                hv_ValidationRatio.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ValidationRatio = hv_ValidationPercent * 0.01;
                }
                hv_SplitRatios.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SplitRatios = new HTuple();
                    hv_SplitRatios = hv_SplitRatios.TupleConcat(hv_TrainingRatio, hv_ValidationRatio);
                    hv_SplitRatios = hv_SplitRatios.TupleConcat((1.0 - hv_TrainingRatio) - hv_ValidationRatio);
                }
                hv_SplitRatiosInvSortIndices.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SplitRatiosInvSortIndices = (new HTuple(hv_SplitRatios.TupleSortIndex()
                        )).TupleInverse();
                }
                //
                //Test whether the dataset is already split.
                hv_SplitKeys.Dispose();
                hv_SplitKeys = new HTuple();
                for (hv_IndexSample = 0; (int)hv_IndexSample <= (int)((new HTuple(hv_DLSamples.TupleLength()
                    )) - 1); hv_IndexSample = (int)hv_IndexSample + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SplitExists.Dispose();
                        HOperatorSet.GetDictParam(hv_DLSamples.TupleSelect(hv_IndexSample), "key_exists",
                            "split", out hv_SplitExists);
                    }
                    if ((int)(hv_SplitExists) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_SampleSplit.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLSamples.TupleSelect(hv_IndexSample), "split",
                                out hv_SampleSplit);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_SplitKeys = hv_SplitKeys.TupleConcat(
                                    hv_SampleSplit);
                                hv_SplitKeys.Dispose();
                                hv_SplitKeys = ExpTmpLocalVar_SplitKeys;
                            }
                        }
                    }
                }
                hv_NotYetSplit.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_NotYetSplit = new HTuple(hv_SplitKeys.TupleEqual(
                        new HTuple()));
                }
                //
                //Split the dataset if no split is present
                //or split should be overwritten.
                if ((int)(hv_NotYetSplit.TupleOr(hv_OverwriteSplit)) != 0)
                {
                    //
                    //Initialize a dictionary to collect the IDs
                    //of images that contain the class.
                    hv_ImageIDsPerClass.Dispose();
                    HOperatorSet.CreateDict(out hv_ImageIDsPerClass);
                    for (hv_ClassIndex = 0; (int)hv_ClassIndex <= (int)((new HTuple(hv_ClassNames.TupleLength()
                        )) - 1); hv_ClassIndex = (int)hv_ClassIndex + 1)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_ImageIDsPerClass, hv_ClassNames.TupleSelect(
                                hv_ClassIndex), new HTuple());
                        }
                    }
                    //
                    //Get labels of every sample image
                    //and count how many images per class there are.
                    //
                    hv_NumImagesPerClass.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumImagesPerClass = HTuple.TupleGenConst(
                            new HTuple(hv_ClassIDs.TupleLength()), 0);
                    }
                    hv_ImageIDList.Dispose();
                    hv_ImageIDList = new HTuple();
                    for (hv_IndexSample = 0; (int)hv_IndexSample <= (int)((new HTuple(hv_DLSamples.TupleLength()
                        )) - 1); hv_IndexSample = (int)hv_IndexSample + 1)
                    {
                        hv_DLSample.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_DLSample = hv_DLSamples.TupleSelect(
                                hv_IndexSample);
                        }
                        hv_ImageID.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageID);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_ImageIDList = hv_ImageIDList.TupleConcat(
                                    hv_ImageID);
                                hv_ImageIDList.Dispose();
                                hv_ImageIDList = ExpTmpLocalVar_ImageIDList;
                            }
                        }
                        if ((int)(new HTuple(hv_ModelType.TupleEqual("anomaly_detection"))) != 0)
                        {
                            //Get labels - anomaly detection.
                            hv_AnomalyLabel.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLSample, "anomaly_label", out hv_AnomalyLabel);
                            hv_Labels.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Labels = new HTuple(hv_AnomalyLabel.TupleNotEqual(
                                    "ok"));
                            }
                        }
                        else if ((int)(new HTuple(hv_ModelType.TupleEqual("classification"))) != 0)
                        {
                            //Get labels - classification.
                            hv_ImageLabelID.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLSample, "image_label_id", out hv_ImageLabelID);
                            hv_Labels.Dispose();
                            hv_Labels = new HTuple(hv_ImageLabelID);
                        }
                        else if ((int)(new HTuple(hv_ModelType.TupleEqual("detection"))) != 0)
                        {
                            //Get labels - object detection.
                            hv_BboxLabels.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLSample, "bbox_label_id", out hv_BboxLabels);
                            hv_Labels.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Labels = ((hv_BboxLabels.TupleSort()
                                    )).TupleUniq();
                            }
                        }
                        else if ((int)(new HTuple(hv_ModelType.TupleEqual("segmentation"))) != 0)
                        {
                            //Get labels - semantic segmentation.
                            hv_SegmDir.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLDataset, "segmentation_dir", out hv_SegmDir);
                            hv_SegmFileName.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLSample, "segmentation_file_name", out hv_SegmFileName);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_SegmImage.Dispose();
                                HOperatorSet.ReadImage(out ho_SegmImage, (hv_SegmDir + "/") + hv_SegmFileName);
                            }
                            //
                            hv_AbsoluteHisto.Dispose();
                            HOperatorSet.GrayHistoAbs(ho_SegmImage, ho_SegmImage, 1, out hv_AbsoluteHisto);
                            hv_Labels.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Labels = ((hv_AbsoluteHisto.TupleGreaterElem(
                                    0))).TupleFind(1);
                            }
                        }
                        //
                        //Add up images per class.
                        hv_LabelIndices.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LabelIndices = hv_ClassIDToClassIndex.TupleSelect(
                                hv_Labels);
                        }
                        if (hv_NumImagesPerClass == null)
                            hv_NumImagesPerClass = new HTuple();
                        hv_NumImagesPerClass[hv_LabelIndices] = (hv_NumImagesPerClass.TupleSelect(
                            hv_LabelIndices)) + 1;
                        for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Labels.TupleLength()
                            )) - 1); hv_Index = (int)hv_Index + 1)
                        {
                            //Add ImageIDs to ImageIDsPerClass.
                            hv_ClassIndex.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ClassIndex = hv_LabelIndices.TupleSelect(
                                    hv_Index);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ImgIDsClass.Dispose();
                                HOperatorSet.GetDictTuple(hv_ImageIDsPerClass, hv_ClassNames.TupleSelect(
                                    hv_ClassIndex), out hv_ImgIDsClass);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_ImgIDsClass = hv_ImgIDsClass.TupleConcat(
                                        hv_ImageID);
                                    hv_ImgIDsClass.Dispose();
                                    hv_ImgIDsClass = ExpTmpLocalVar_ImgIDsClass;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetDictTuple(hv_ImageIDsPerClass, hv_ClassNames.TupleSelect(
                                    hv_ClassIndex), hv_ImgIDsClass);
                            }
                        }
                    }
                    //
                    //** Start splitting. ***
                    //
                    //Create a dictionary where the ImageIDs
                    //for the three subsets are saved.
                    hv_SplitImageIDs.Dispose();
                    HOperatorSet.CreateDict(out hv_SplitImageIDs);
                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_SplitNames.TupleLength()
                        )) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_SplitImageIDs, hv_SplitNames.TupleSelect(hv_Index),
                                new HTuple());
                        }
                    }
                    //We split based on classes,
                    //starting with the smallest class.
                    hv_AssignedImageIDs.Dispose();
                    hv_AssignedImageIDs = new HTuple();
                    hv_ClassSortIndices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ClassSortIndices = hv_NumImagesPerClass.TupleSortIndex()
                            ;
                    }
                    for (hv_ClassIndex = 0; (int)hv_ClassIndex <= (int)((new HTuple(hv_NumImagesPerClass.TupleLength()
                        )) - 1); hv_ClassIndex = (int)hv_ClassIndex + 1)
                    {
                        //Get all ImageIDs where this class is present.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ImageIDsClass.Dispose();
                            HOperatorSet.GetDictTuple(hv_ImageIDsPerClass, hv_ClassNames.TupleSelect(
                                hv_ClassSortIndices.TupleSelect(hv_ClassIndex)), out hv_ImageIDsClass);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_ImageIDsClass = hv_ImageIDsClass.TupleUniq()
                                    ;
                                hv_ImageIDsClass.Dispose();
                                hv_ImageIDsClass = ExpTmpLocalVar_ImageIDsClass;
                            }
                        }
                        //Remove ImageIDs that have already been assigned.
                        hv_ImageIDsClassToBeAssigned.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ImageIDsClassToBeAssigned = hv_ImageIDsClass.TupleDifference(
                                hv_AssignedImageIDs);
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            tuple_shuffle(hv_ImageIDsClassToBeAssigned, out ExpTmpOutVar_0);
                            hv_ImageIDsClassToBeAssigned.Dispose();
                            hv_ImageIDsClassToBeAssigned = ExpTmpOutVar_0;
                        }
                        //
                        for (hv_SplitIndex = 0; (int)hv_SplitIndex <= (int)((new HTuple(hv_SplitNames.TupleLength()
                            )) - 1); hv_SplitIndex = (int)hv_SplitIndex + 1)
                        {
                            //Check how many of the IDs have already been assigned
                            //and how many should be assigned.
                            if ((int)((new HTuple(hv_ModelType.TupleEqual("anomaly_detection"))).TupleAnd(
                                new HTuple(((hv_ClassNames.TupleSelect(hv_ClassSortIndices.TupleSelect(
                                hv_ClassIndex)))).TupleEqual("nok")))) != 0)
                            {
                                //All 'nok' images for anomaly detection are sorted into the test set.
                                if ((int)(new HTuple(((hv_SplitNames.TupleSelect(hv_SplitIndex))).TupleEqual(
                                    "test"))) != 0)
                                {
                                    hv_NumToBeAssignedToThisSplit.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_NumToBeAssignedToThisSplit = new HTuple(hv_ImageIDsClass.TupleLength()
                                            );
                                    }
                                }
                                else
                                {
                                    hv_NumToBeAssignedToThisSplit.Dispose();
                                    hv_NumToBeAssignedToThisSplit = 0;
                                }
                            }
                            else
                            {
                                hv_NumToBeAssignedToThisSplit.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_NumToBeAssignedToThisSplit = (((((hv_SplitRatios.TupleSelect(
                                        hv_SplitIndex)) * (new HTuple(hv_ImageIDsClass.TupleLength())))).TupleFloor()
                                        )).TupleInt();
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_AssignedImageIDsToThisSplit.Dispose();
                                HOperatorSet.GetDictTuple(hv_SplitImageIDs, hv_SplitNames.TupleSelect(
                                    hv_SplitIndex), out hv_AssignedImageIDsToThisSplit);
                            }
                            hv_NumAlreadyAssignedToThisSplit.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_NumAlreadyAssignedToThisSplit = new HTuple(((hv_ImageIDsClass.TupleIntersection(
                                    hv_AssignedImageIDsToThisSplit))).TupleLength());
                            }
                            hv_NumStillToBeAssigned.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_NumStillToBeAssigned = hv_NumToBeAssignedToThisSplit - hv_NumAlreadyAssignedToThisSplit;
                            }
                            //
                            if ((int)(new HTuple(hv_NumStillToBeAssigned.TupleGreater(0))) != 0)
                            {
                                if ((int)(new HTuple(hv_NumStillToBeAssigned.TupleGreater(new HTuple(hv_ImageIDsClassToBeAssigned.TupleLength()
                                    )))) != 0)
                                {
                                    hv_NumStillToBeAssigned.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_NumStillToBeAssigned = new HTuple(hv_ImageIDsClassToBeAssigned.TupleLength()
                                            );
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_AssignedImageIDsToThisSplit = hv_AssignedImageIDsToThisSplit.TupleConcat(
                                            hv_ImageIDsClassToBeAssigned.TupleSelectRange(0, hv_NumStillToBeAssigned - 1));
                                        hv_AssignedImageIDsToThisSplit.Dispose();
                                        hv_AssignedImageIDsToThisSplit = ExpTmpLocalVar_AssignedImageIDsToThisSplit;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.SetDictTuple(hv_SplitImageIDs, hv_SplitNames.TupleSelect(
                                        hv_SplitIndex), hv_AssignedImageIDsToThisSplit);
                                }
                                //Update the remaining ImageIDs of this class.
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_ImageIDsClassToBeAssigned = hv_ImageIDsClassToBeAssigned.TupleSelectRange(
                                            hv_NumStillToBeAssigned, (new HTuple(hv_ImageIDsClassToBeAssigned.TupleLength()
                                            )) - 1);
                                        hv_ImageIDsClassToBeAssigned.Dispose();
                                        hv_ImageIDsClassToBeAssigned = ExpTmpLocalVar_ImageIDsClassToBeAssigned;
                                    }
                                }
                            }
                        }
                        //The remaining ImageIDs are assigned to random subsets
                        //according to the defined ratios.
                        if ((int)(new HTuple((new HTuple(hv_ImageIDsClassToBeAssigned.TupleLength()
                            )).TupleGreater(0))) != 0)
                        {
                            for (hv_ImageIndex = 0; (int)hv_ImageIndex <= (int)((new HTuple(hv_ImageIDsClassToBeAssigned.TupleLength()
                                )) - 1); hv_ImageIndex = (int)hv_ImageIndex + 1)
                            {
                                hv_Rand.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Rand = HTuple.TupleRand(
                                        1);
                                }
                                for (hv_RatioIndex = 0; (int)hv_RatioIndex <= (int)((new HTuple(hv_SplitRatios.TupleLength()
                                    )) - 1); hv_RatioIndex = (int)hv_RatioIndex + 1)
                                {
                                    hv_CurrentSplitIndex.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_CurrentSplitIndex = hv_SplitRatiosInvSortIndices.TupleSelect(
                                            hv_RatioIndex);
                                    }
                                    hv_CurrentSplitRatio.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_CurrentSplitRatio = hv_SplitRatios.TupleSelect(
                                            hv_CurrentSplitIndex);
                                    }
                                    if ((int)(new HTuple(hv_Rand.TupleLessEqual(hv_CurrentSplitRatio))) != 0)
                                    {
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_AssignedImageIDsToThisSplit.Dispose();
                                            HOperatorSet.GetDictTuple(hv_SplitImageIDs, hv_SplitNames.TupleSelect(
                                                hv_CurrentSplitIndex), out hv_AssignedImageIDsToThisSplit);
                                        }
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            {
                                                HTuple
                                                  ExpTmpLocalVar_AssignedImageIDsToThisSplit = hv_AssignedImageIDsToThisSplit.TupleConcat(
                                                    hv_ImageIDsClassToBeAssigned.TupleSelect(hv_ImageIndex));
                                                hv_AssignedImageIDsToThisSplit.Dispose();
                                                hv_AssignedImageIDsToThisSplit = ExpTmpLocalVar_AssignedImageIDsToThisSplit;
                                            }
                                        }
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            HOperatorSet.SetDictTuple(hv_SplitImageIDs, hv_SplitNames.TupleSelect(
                                                hv_CurrentSplitIndex), hv_AssignedImageIDsToThisSplit);
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            {
                                                HTuple
                                                  ExpTmpLocalVar_Rand = hv_Rand - hv_CurrentSplitRatio;
                                                hv_Rand.Dispose();
                                                hv_Rand = ExpTmpLocalVar_Rand;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_AssignedImageIDs = ((((hv_AssignedImageIDs.TupleConcat(
                                    hv_ImageIDsClass))).TupleUniq())).TupleSort();
                                hv_AssignedImageIDs.Dispose();
                                hv_AssignedImageIDs = ExpTmpLocalVar_AssignedImageIDs;
                            }
                        }
                    }
                    //
                    //There might be images not having any labels:
                    //Assign them based on the ratio.
                    hv_ImageIDsWithoutLabel.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageIDsWithoutLabel = hv_ImageIDList.TupleDifference(
                            hv_AssignedImageIDs);
                    }
                    hv_NumImageIDsWithoutLabel.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumImageIDsWithoutLabel = new HTuple(hv_ImageIDsWithoutLabel.TupleLength()
                            );
                    }
                    if ((int)(new HTuple(hv_NumImageIDsWithoutLabel.TupleGreater(0))) != 0)
                    {
                        {
                            HTuple ExpTmpOutVar_0;
                            tuple_shuffle(hv_ImageIDsWithoutLabel, out ExpTmpOutVar_0);
                            hv_ImageIDsWithoutLabel.Dispose();
                            hv_ImageIDsWithoutLabel = ExpTmpOutVar_0;
                        }
                        for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_SplitRatios.TupleLength()
                            )) - 1); hv_Index = (int)hv_Index + 1)
                        {
                            hv_NumToBeAssigned.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_NumToBeAssigned = (((hv_SplitRatios.TupleSelect(
                                    hv_Index)) * hv_NumImageIDsWithoutLabel)).TupleInt();
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_AssignedImageIDsToThisSplit.Dispose();
                                HOperatorSet.GetDictTuple(hv_SplitImageIDs, hv_SplitNames.TupleSelect(
                                    hv_Index), out hv_AssignedImageIDsToThisSplit);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_AssignedImageIDsToThisSplit = hv_AssignedImageIDsToThisSplit.TupleConcat(
                                        hv_ImageIDsWithoutLabel.TupleSelectRange(0, hv_NumToBeAssigned - 1));
                                    hv_AssignedImageIDsToThisSplit.Dispose();
                                    hv_AssignedImageIDsToThisSplit = ExpTmpLocalVar_AssignedImageIDsToThisSplit;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetDictTuple(hv_SplitImageIDs, hv_SplitNames.TupleSelect(
                                    hv_Index), hv_AssignedImageIDsToThisSplit.TupleSort());
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_ImageIDsWithoutLabel = hv_ImageIDsWithoutLabel.TupleSelectRange(
                                        hv_NumToBeAssigned, (new HTuple(hv_ImageIDsWithoutLabel.TupleLength()
                                        )) - 1);
                                    hv_ImageIDsWithoutLabel.Dispose();
                                    hv_ImageIDsWithoutLabel = ExpTmpLocalVar_ImageIDsWithoutLabel;
                                }
                            }
                        }
                        //If there are still ImageIDs, assign them to split with highest ratio.
                        hv_MaxRatioIndex.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_MaxRatioIndex = hv_SplitRatiosInvSortIndices.TupleFind(
                                0);
                        }
                        if ((int)(new HTuple((new HTuple(hv_ImageIDsWithoutLabel.TupleLength())).TupleGreater(
                            0))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_AssignedImageIDsToThisSplit.Dispose();
                                HOperatorSet.GetDictTuple(hv_SplitImageIDs, hv_SplitNames.TupleSelect(
                                    hv_MaxRatioIndex), out hv_AssignedImageIDsToThisSplit);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_AssignedImageIDsToThisSplit = hv_AssignedImageIDsToThisSplit.TupleConcat(
                                        hv_ImageIDsWithoutLabel);
                                    hv_AssignedImageIDsToThisSplit.Dispose();
                                    hv_AssignedImageIDsToThisSplit = ExpTmpLocalVar_AssignedImageIDsToThisSplit;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetDictTuple(hv_SplitImageIDs, hv_SplitNames.TupleSelect(
                                    hv_MaxRatioIndex), hv_AssignedImageIDsToThisSplit.TupleSort());
                            }
                        }
                    }
                    //
                    //Assign 'split' entries to samples.
                    for (hv_SplitNameIndex = 0; (int)hv_SplitNameIndex <= (int)((new HTuple(hv_SplitNames.TupleLength()
                        )) - 1); hv_SplitNameIndex = (int)hv_SplitNameIndex + 1)
                    {
                        hv_SplitName.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_SplitName = hv_SplitNames.TupleSelect(
                                hv_SplitNameIndex);
                        }
                        hv_SplitIDs.Dispose();
                        HOperatorSet.GetDictTuple(hv_SplitImageIDs, hv_SplitName, out hv_SplitIDs);
                        for (hv_SplitIDIndex = 0; (int)hv_SplitIDIndex <= (int)((new HTuple(hv_SplitIDs.TupleLength()
                            )) - 1); hv_SplitIDIndex = (int)hv_SplitIDIndex + 1)
                        {
                            hv_SampleSplitIDs.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_SampleSplitIDs = hv_SplitIDs.TupleSelect(
                                    hv_SplitIDIndex);
                            }
                            hv_DLSampleIndex.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_DLSampleIndex = hv_ImageIDList.TupleFind(
                                    hv_SampleSplitIDs);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetDictTuple(hv_DLSamples.TupleSelect(hv_DLSampleIndex),
                                    "split", hv_SplitName);
                            }
                        }
                    }
                }
                else
                {
                    hv_ErrorDict.Dispose();
                    hv_ErrorDict = "The dataset is already split. You can overwrite the existing split using the generic parameter 'overwrite_split'.";
                    throw new HalconException(hv_ErrorDict);
                }
                ho_SegmImage.Dispose();

                hv_OverwriteSplit.Dispose();
                hv_ModelType.Dispose();
                hv_SplitNames.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamIndex.Dispose();
                hv_Type.Dispose();
                hv_DLSamples.Dispose();
                hv_DLSample.Dispose();
                hv_AnomalyDetectionLabelExists.Dispose();
                hv_BBoxLabelIdExists.Dispose();
                hv_ImageLabelIdExists.Dispose();
                hv_SegmFileExists.Dispose();
                hv_ClassIDs.Dispose();
                hv_ClassNames.Dispose();
                hv_ClassIDToClassIndex.Dispose();
                hv_TrainingRatio.Dispose();
                hv_ValidationRatio.Dispose();
                hv_SplitRatios.Dispose();
                hv_SplitRatiosInvSortIndices.Dispose();
                hv_SplitKeys.Dispose();
                hv_IndexSample.Dispose();
                hv_SplitExists.Dispose();
                hv_SampleSplit.Dispose();
                hv_NotYetSplit.Dispose();
                hv_ImageIDsPerClass.Dispose();
                hv_ClassIndex.Dispose();
                hv_NumImagesPerClass.Dispose();
                hv_ImageIDList.Dispose();
                hv_ImageID.Dispose();
                hv_AnomalyLabel.Dispose();
                hv_Labels.Dispose();
                hv_ImageLabelID.Dispose();
                hv_BboxLabels.Dispose();
                hv_SegmDir.Dispose();
                hv_SegmFileName.Dispose();
                hv_AbsoluteHisto.Dispose();
                hv_LabelIndices.Dispose();
                hv_Index.Dispose();
                hv_ImgIDsClass.Dispose();
                hv_SplitImageIDs.Dispose();
                hv_AssignedImageIDs.Dispose();
                hv_ClassSortIndices.Dispose();
                hv_ImageIDsClass.Dispose();
                hv_ImageIDsClassToBeAssigned.Dispose();
                hv_SplitIndex.Dispose();
                hv_NumToBeAssignedToThisSplit.Dispose();
                hv_AssignedImageIDsToThisSplit.Dispose();
                hv_NumAlreadyAssignedToThisSplit.Dispose();
                hv_NumStillToBeAssigned.Dispose();
                hv_ImageIndex.Dispose();
                hv_Rand.Dispose();
                hv_RatioIndex.Dispose();
                hv_CurrentSplitIndex.Dispose();
                hv_CurrentSplitRatio.Dispose();
                hv_ImageIDsWithoutLabel.Dispose();
                hv_NumImageIDsWithoutLabel.Dispose();
                hv_NumToBeAssigned.Dispose();
                hv_MaxRatioIndex.Dispose();
                hv_SplitNameIndex.Dispose();
                hv_SplitName.Dispose();
                hv_SplitIDs.Dispose();
                hv_SplitIDIndex.Dispose();
                hv_SampleSplitIDs.Dispose();
                hv_DLSampleIndex.Dispose();
                hv_ErrorDict.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_SegmImage.Dispose();

                hv_OverwriteSplit.Dispose();
                hv_ModelType.Dispose();
                hv_SplitNames.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamIndex.Dispose();
                hv_Type.Dispose();
                hv_DLSamples.Dispose();
                hv_DLSample.Dispose();
                hv_AnomalyDetectionLabelExists.Dispose();
                hv_BBoxLabelIdExists.Dispose();
                hv_ImageLabelIdExists.Dispose();
                hv_SegmFileExists.Dispose();
                hv_ClassIDs.Dispose();
                hv_ClassNames.Dispose();
                hv_ClassIDToClassIndex.Dispose();
                hv_TrainingRatio.Dispose();
                hv_ValidationRatio.Dispose();
                hv_SplitRatios.Dispose();
                hv_SplitRatiosInvSortIndices.Dispose();
                hv_SplitKeys.Dispose();
                hv_IndexSample.Dispose();
                hv_SplitExists.Dispose();
                hv_SampleSplit.Dispose();
                hv_NotYetSplit.Dispose();
                hv_ImageIDsPerClass.Dispose();
                hv_ClassIndex.Dispose();
                hv_NumImagesPerClass.Dispose();
                hv_ImageIDList.Dispose();
                hv_ImageID.Dispose();
                hv_AnomalyLabel.Dispose();
                hv_Labels.Dispose();
                hv_ImageLabelID.Dispose();
                hv_BboxLabels.Dispose();
                hv_SegmDir.Dispose();
                hv_SegmFileName.Dispose();
                hv_AbsoluteHisto.Dispose();
                hv_LabelIndices.Dispose();
                hv_Index.Dispose();
                hv_ImgIDsClass.Dispose();
                hv_SplitImageIDs.Dispose();
                hv_AssignedImageIDs.Dispose();
                hv_ClassSortIndices.Dispose();
                hv_ImageIDsClass.Dispose();
                hv_ImageIDsClassToBeAssigned.Dispose();
                hv_SplitIndex.Dispose();
                hv_NumToBeAssignedToThisSplit.Dispose();
                hv_AssignedImageIDsToThisSplit.Dispose();
                hv_NumAlreadyAssignedToThisSplit.Dispose();
                hv_NumStillToBeAssigned.Dispose();
                hv_ImageIndex.Dispose();
                hv_Rand.Dispose();
                hv_RatioIndex.Dispose();
                hv_CurrentSplitIndex.Dispose();
                hv_CurrentSplitRatio.Dispose();
                hv_ImageIDsWithoutLabel.Dispose();
                hv_NumImageIDsWithoutLabel.Dispose();
                hv_NumToBeAssigned.Dispose();
                hv_MaxRatioIndex.Dispose();
                hv_SplitNameIndex.Dispose();
                hv_SplitName.Dispose();
                hv_SplitIDs.Dispose();
                hv_SplitIDIndex.Dispose();
                hv_SampleSplitIDs.Dispose();
                hv_DLSampleIndex.Dispose();
                hv_ErrorDict.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region preprocess_dl_dataset
        // Chapter: Deep Learning / Model
        // Short Description: Standard preprocessing on the entire dataset declared in DLDataset. 
        public void preprocess_dl_dataset(HTuple hv_DLDataset, HTuple hv_DataDirectory, HTuple hv_DLPreprocessParam, HTuple hv_GenParam, out HTuple hv_DLDatasetFileName)
        {
            // Local iconic variables 

            // Local control variables 

            HTuple hv_OverwriteFiles = new HTuple(), hv_ShowProgress = new HTuple();
            HTuple hv_ClassWeightsSegmentation = new HTuple(), hv_MaxWeight = new HTuple();
            HTuple hv_DLModelType = new HTuple(), hv_GenParamGenDLSamples = new HTuple();
            HTuple hv_ClassIDsDataset = new HTuple(), hv_SetBackgroundID = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_KeyExists = new HTuple();
            HTuple hv_InstanceType = new HTuple(), hv_GenParamName = new HTuple();
            HTuple hv_GenParamIndex = new HTuple(), hv_FileExists = new HTuple();
            HTuple hv_DLSampleDir = new HTuple(), hv_DLDatasetSamples = new HTuple();
            HTuple hv_Progress = new HTuple(), hv_SecondsStart = new HTuple();
            HTuple hv_SampleIndex = new HTuple(), hv_DLSampleBatch = new HTuple();
            HTuple hv_SecondsElapsed = new HTuple(), hv_SecondsRemaining = new HTuple();
            HTuple hv_ProgressPercent = new HTuple(), hv_ProgressPerSecond = new HTuple();
            HTuple hv_TimeElapsedString = new HTuple(), hv_TimeRemainingString = new HTuple();
            HTuple hv_IgnoreClassIDs = new HTuple();
            HTuple hv_DataDirectory_COPY_INP_TMP = new HTuple(hv_DataDirectory);

            // Initialize local and output iconic variables 
            hv_DLDatasetFileName = new HTuple();
            try
            {
                //
                //This procedure preprocesses the samples in the dictionary DLDataset.
                //
                //** Parameters values: ***
                //
                //Set the default values.
                //Overwrite existing DLDataset file and DLSample directory.
                hv_OverwriteFiles.Dispose();
                hv_OverwriteFiles = 0;
                //By default we show the progress of preprocessing.
                hv_ShowProgress.Dispose();
                hv_ShowProgress = 1;
                //Class weights specified by user (needed for segmentation)
                hv_ClassWeightsSegmentation.Dispose();
                hv_ClassWeightsSegmentation = new HTuple();
                //Set max weight. Parameter for calculating the weights (needed for segmentation).
                hv_MaxWeight.Dispose();
                hv_MaxWeight = 1000;
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //Get the model type.
                hv_DLModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_DLModelType);
                //
                //Initialize the generic parameters for gen_dl_samples.
                hv_GenParamGenDLSamples.Dispose();
                hv_GenParamGenDLSamples = new HTuple();
                //Check if the background class ID is part of the DLDataset class IDs.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                {
                    hv_ClassIDsDataset.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLDataset, "class_ids", out hv_ClassIDsDataset);
                    hv_SetBackgroundID.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "set_background_id", out hv_SetBackgroundID);
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_Indices.Dispose();
                        HOperatorSet.TupleFind(hv_ClassIDsDataset, hv_SetBackgroundID, out hv_Indices);
                        if ((int)(new HTuple(hv_Indices.TupleEqual(-1))) != 0)
                        {
                            throw new HalconException(("The 'set_background_id':'" + hv_SetBackgroundID) + "' needs to be part of the DLDataset 'class_ids' tuple.");
                        }
                    }
                }
                else if ((int)(new HTuple(hv_DLModelType.TupleEqual("detection"))) != 0)
                {
                    hv_GenParamGenDLSamples.Dispose();
                    HOperatorSet.CreateDict(out hv_GenParamGenDLSamples);
                    hv_KeyExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "instance_type",
                        out hv_KeyExists);
                    if ((int)(hv_KeyExists) != 0)
                    {
                        hv_InstanceType.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "instance_type", out hv_InstanceType);
                        HOperatorSet.SetDictTuple(hv_GenParamGenDLSamples, "instance_type", hv_InstanceType);
                    }
                    else
                    {
                        HOperatorSet.SetDictTuple(hv_GenParamGenDLSamples, "instance_type", "rectangle1");
                    }
                }
                //
                //Set the parameters for preprocess_dl_samples.
                HOperatorSet.SetDictTuple(hv_DLDataset, "preprocess_param", hv_DLPreprocessParam);
                //
                //Transfer generic parameters.
                if ((int)(new HTuple(hv_GenParam.TupleNotEqual(new HTuple()))) != 0)
                {
                    hv_GenParamName.Dispose();
                    HOperatorSet.GetDictParam(hv_GenParam, "keys", new HTuple(), out hv_GenParamName);
                    for (hv_GenParamIndex = 0; (int)hv_GenParamIndex <= (int)((new HTuple(hv_GenParamName.TupleLength()
                        )) - 1); hv_GenParamIndex = (int)hv_GenParamIndex + 1)
                    {
                        if ((int)(new HTuple(((hv_GenParamName.TupleSelect(hv_GenParamIndex))).TupleEqual(
                            "overwrite_files"))) != 0)
                        {
                            hv_OverwriteFiles.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, "overwrite_files", out hv_OverwriteFiles);
                        }
                        else if ((int)(new HTuple(((hv_GenParamName.TupleSelect(hv_GenParamIndex))).TupleEqual(
                            "show_progress"))) != 0)
                        {
                            hv_ShowProgress.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, "show_progress", out hv_ShowProgress);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_ShowProgress = (new HTuple(hv_ShowProgress.TupleEqual(
                                        "true"))).TupleOr(new HTuple(hv_ShowProgress.TupleEqual(1)));
                                    hv_ShowProgress.Dispose();
                                    hv_ShowProgress = ExpTmpLocalVar_ShowProgress;
                                }
                            }
                        }
                        else if ((int)(new HTuple(((hv_GenParamName.TupleSelect(hv_GenParamIndex))).TupleEqual(
                            "max_weight"))) != 0)
                        {
                            hv_MaxWeight.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, "max_weight", out hv_MaxWeight);
                            if ((int)(new HTuple(hv_DLModelType.TupleNotEqual("segmentation"))) != 0)
                            {
                                throw new HalconException("The preprocessing parameter 'max_weight' only applies for segmentation models.");
                            }
                        }
                        else if ((int)(new HTuple(((hv_GenParamName.TupleSelect(hv_GenParamIndex))).TupleEqual(
                            "class_weights"))) != 0)
                        {
                            hv_ClassWeightsSegmentation.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, "class_weights", out hv_ClassWeightsSegmentation);
                            if ((int)(new HTuple(hv_DLModelType.TupleNotEqual("segmentation"))) != 0)
                            {
                                throw new HalconException("The preprocessing parameter 'class_weights' only applies for segmentation models.");
                            }
                        }
                        else
                        {
                            throw new HalconException(("Unknown generic parameter: '" + (hv_GenParamName.TupleSelect(
                                hv_GenParamIndex))) + "'");
                        }
                    }
                }
                //
                //** Clean/Create data directory: ***
                {
                    HTuple ExpTmpOutVar_0;
                    HOperatorSet.TupleRegexpReplace(hv_DataDirectory_COPY_INP_TMP, "/+$", "", out ExpTmpOutVar_0);
                    hv_DataDirectory_COPY_INP_TMP.Dispose();
                    hv_DataDirectory_COPY_INP_TMP = ExpTmpOutVar_0;
                }
                hv_FileExists.Dispose();
                HOperatorSet.FileExists(hv_DataDirectory_COPY_INP_TMP, out hv_FileExists);
                if ((int)(hv_FileExists.TupleAnd(hv_OverwriteFiles.TupleNot())) != 0)
                {
                    throw new HalconException(("The folder " + hv_DataDirectory_COPY_INP_TMP) + " already exists. Either give a different directory or force overwriting using the parameter 'overwrite_files'.");
                }
                if ((int)(hv_FileExists) != 0)
                {
                    remove_dir_recursively(hv_DataDirectory_COPY_INP_TMP);
                }
                HOperatorSet.MakeDir(hv_DataDirectory_COPY_INP_TMP);
                //
                //Create the directory for the DLSamples, if it does not exist.
                //
                //Sample directory name.
                hv_DLSampleDir.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DLSampleDir = hv_DataDirectory_COPY_INP_TMP + "/samples";
                }
                HOperatorSet.MakeDir(hv_DLSampleDir);
                //
                //Set the output path.
                HOperatorSet.SetDictTuple(hv_DLDataset, "dlsample_dir", hv_DLSampleDir);
                //
                //** Preprocess all images in the dataset: ***
                //During training/validation and testing those preprocessed images
                //will be used for performance reasons.
                //
                //Get the samples to be preprocessed.
                hv_DLDatasetSamples.Dispose();
                HOperatorSet.GetDictTuple(hv_DLDataset, "samples", out hv_DLDatasetSamples);
                //
                //Initialize progress variables.
                if ((int)(hv_ShowProgress) != 0)
                {
                    hv_Progress.Dispose();
                    hv_Progress = new HTuple();
                    hv_Progress[0] = "Procedure: preprocess_dl_dataset";
                    hv_Progress[1] = "";
                    hv_Progress[2] = "";
                    hv_Progress[3] = "";
                    if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Progress = hv_Progress.TupleConcat(
                                    "Task: 1/2: image preprocessing");
                                hv_Progress.Dispose();
                                hv_Progress = ExpTmpLocalVar_Progress;
                            }
                        }
                    }
                    hv_SecondsStart.Dispose();
                    HOperatorSet.CountSeconds(out hv_SecondsStart);
                    // dev_inspect_ctrl(...); only in hdevelop
                }
                //
                //Loop over all samples.
                for (hv_SampleIndex = 0; (int)hv_SampleIndex <= (int)((new HTuple(hv_DLDatasetSamples.TupleLength()
                    )) - 1); hv_SampleIndex = (int)hv_SampleIndex + 1)
                {
                    //
                    //Generate the dictionary DLSample.
                    hv_DLSampleBatch.Dispose();

                    //ADD FOR DISPLAY
                    loopCount = hv_SampleIndex + 1;
                    imageCount = new HTuple(hv_DLDatasetSamples.TupleLength());

                    gen_dl_samples(hv_DLDataset, hv_SampleIndex, hv_DLModelType, hv_GenParamGenDLSamples,
                        out hv_DLSampleBatch);
                    //
                    //Preprocess the DLSample.
                    preprocess_dl_samples(hv_DLSampleBatch, hv_DLPreprocessParam);
                    //
                    //Write the preprocessed images.
                    write_dl_samples(hv_DLDataset, hv_SampleIndex, hv_DLSampleBatch, new HTuple(),
                        new HTuple());
                    //
                    //Provide progress information.
                    if ((int)(hv_ShowProgress.TupleAnd((new HTuple(((hv_SampleIndex % 10)).TupleEqual(
                        1))).TupleOr(new HTuple(hv_SampleIndex.TupleEqual((new HTuple(hv_DLDatasetSamples.TupleLength()
                        )) - 1))))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_SecondsElapsed.Dispose(); hv_SecondsRemaining.Dispose(); hv_ProgressPercent.Dispose(); hv_ProgressPerSecond.Dispose();
                            estimate_progress(hv_SecondsStart, 0, hv_SampleIndex, (new HTuple(hv_DLDatasetSamples.TupleLength()
                                )) - 1, out hv_SecondsElapsed, out hv_SecondsRemaining, out hv_ProgressPercent,
                                out hv_ProgressPerSecond);
                        }
                        hv_TimeElapsedString.Dispose();
                        timespan_string(hv_SecondsElapsed, "auto", out hv_TimeElapsedString);
                        hv_TimeRemainingString.Dispose();
                        timespan_string(hv_SecondsRemaining, "top2", out hv_TimeRemainingString);
                        if (hv_Progress == null)
                            hv_Progress = new HTuple();
                        hv_Progress[1] = ("Preprocess progress: " + (hv_ProgressPercent.TupleRound())) + " %";
                        if (hv_Progress == null)
                            hv_Progress = new HTuple();
                        hv_Progress[2] = "Time elapsed: " + hv_TimeElapsedString;
                        if (hv_Progress == null)
                            hv_Progress = new HTuple();
                        hv_Progress[3] = "Time left: " + hv_TimeRemainingString;

                        //ADD FOR DISPLAY
                        hv_Progress_ForDisplay[0] = hv_Progress[1];
                        hv_Progress_ForDisplay[1] = hv_Progress[2];
                        hv_Progress_ForDisplay[2] = hv_Progress[3];
                    }
                }
                //
                //If the model is of type segmentation, generate weight images.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                {
                    //
                    if ((int)(hv_ShowProgress) != 0)
                    {
                        hv_Progress.Dispose();
                        hv_Progress = new HTuple();
                        hv_Progress[0] = "Procedure: preprocess_dl_dataset";
                        hv_Progress[1] = "";
                        hv_Progress[2] = "";
                        if (hv_Progress == null)
                            hv_Progress = new HTuple();
                        hv_Progress[1] = "Please wait...";
                        if (hv_Progress == null)
                            hv_Progress = new HTuple();
                        hv_Progress[2] = "Task: 2/2: calculating class weights";
                    }
                    if ((int)(new HTuple((new HTuple(hv_ClassWeightsSegmentation.TupleLength()
                        )).TupleEqual(0))) != 0)
                    {
                        //Calculate the class weights for segmentation.
                        hv_IgnoreClassIDs.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", out hv_IgnoreClassIDs);
                        hv_ClassWeightsSegmentation.Dispose();
                        calculate_dl_segmentation_class_weights(hv_DLDataset, hv_MaxWeight, hv_IgnoreClassIDs,
                            out hv_ClassWeightsSegmentation);
                    }
                    //
                    //Generate the weight images.
                    gen_dl_segmentation_weight_images(hv_DLDataset, hv_DLPreprocessParam, hv_ClassWeightsSegmentation,
                        new HTuple());
                }
                if ((int)(hv_ShowProgress.TupleNot()) != 0)
                {
                    hv_Progress.Dispose();
                    hv_Progress = "Done.";
                    // dev_close_inspect_ctrl(...); only in hdevelop
                }
                //
                //Write the DLDataset dict.
                hv_DLDatasetFileName.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    //MODIFY
                    hv_DLDatasetFileName = hv_DataDirectory_COPY_INP_TMP + "\\" + hv_PreprocessResultFileName;
                }
                HOperatorSet.WriteDict(hv_DLDataset, hv_DLDatasetFileName, new HTuple(), new HTuple());

                hv_DataDirectory_COPY_INP_TMP.Dispose();
                hv_OverwriteFiles.Dispose();
                hv_ShowProgress.Dispose();
                hv_ClassWeightsSegmentation.Dispose();
                hv_MaxWeight.Dispose();
                hv_DLModelType.Dispose();
                hv_GenParamGenDLSamples.Dispose();
                hv_ClassIDsDataset.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_Indices.Dispose();
                hv_KeyExists.Dispose();
                hv_InstanceType.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamIndex.Dispose();
                hv_FileExists.Dispose();
                hv_DLSampleDir.Dispose();
                hv_DLDatasetSamples.Dispose();
                hv_Progress.Dispose();
                hv_SecondsStart.Dispose();
                hv_SampleIndex.Dispose();
                hv_DLSampleBatch.Dispose();
                hv_SecondsElapsed.Dispose();
                hv_SecondsRemaining.Dispose();
                hv_ProgressPercent.Dispose();
                hv_ProgressPerSecond.Dispose();
                hv_TimeElapsedString.Dispose();
                hv_TimeRemainingString.Dispose();
                hv_IgnoreClassIDs.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_DataDirectory_COPY_INP_TMP.Dispose();
                hv_OverwriteFiles.Dispose();
                hv_ShowProgress.Dispose();
                hv_ClassWeightsSegmentation.Dispose();
                hv_MaxWeight.Dispose();
                hv_DLModelType.Dispose();
                hv_GenParamGenDLSamples.Dispose();
                hv_ClassIDsDataset.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_Indices.Dispose();
                hv_KeyExists.Dispose();
                hv_InstanceType.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamIndex.Dispose();
                hv_FileExists.Dispose();
                hv_DLSampleDir.Dispose();
                hv_DLDatasetSamples.Dispose();
                hv_Progress.Dispose();
                hv_SecondsStart.Dispose();
                hv_SampleIndex.Dispose();
                hv_DLSampleBatch.Dispose();
                hv_SecondsElapsed.Dispose();
                hv_SecondsRemaining.Dispose();
                hv_ProgressPercent.Dispose();
                hv_ProgressPerSecond.Dispose();
                hv_TimeElapsedString.Dispose();
                hv_TimeRemainingString.Dispose();
                hv_IgnoreClassIDs.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region create_dl_preprocess_param
        // Chapter: Deep Learning / Model
        // Short Description: Creates a dictionary with preprocessing parameters. 
        public void create_dl_preprocess_param(HTuple hv_DLModelType, HTuple hv_ImageWidth, HTuple hv_ImageHeight, HTuple hv_ImageNumChannels, HTuple hv_ImageRangeMin, HTuple hv_ImageRangeMax, HTuple hv_NormalizationType, HTuple hv_DomainHandling, HTuple hv_IgnoreClassIDs, HTuple hv_SetBackgroundID, HTuple hv_ClassIDsBackground, HTuple hv_GenParam, out HTuple hv_DLPreprocessParam)
        {

            // Local control variables 

            HTuple hv_GenParamNames = new HTuple(), hv_GenParamIndex = new HTuple();
            HTuple hv_GenParamValue = new HTuple(), hv_KeysExist = new HTuple();
            HTuple hv_InstanceType = new HTuple();
            // Initialize local and output iconic variables 
            hv_DLPreprocessParam = new HTuple();
            try
            {
                //
                //This procedure creates a dictionary with all parameters needed for preprocessing.
                //
                hv_DLPreprocessParam.Dispose();
                HOperatorSet.CreateDict(out hv_DLPreprocessParam);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "model_type", hv_DLModelType);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_width", hv_ImageWidth);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_height", hv_ImageHeight);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_num_channels", hv_ImageNumChannels);
                if ((int)(new HTuple(hv_ImageRangeMin.TupleEqual(new HTuple()))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_min", -127);
                }
                else
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_min", hv_ImageRangeMin);
                }
                if ((int)(new HTuple(hv_ImageRangeMax.TupleEqual(new HTuple()))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_max", 128);
                }
                else
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_max", hv_ImageRangeMax);
                }
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type", hv_NormalizationType);
                //Replace possible legacy parameters.
                replace_legacy_preprocessing_parameters(hv_DLPreprocessParam);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "domain_handling", hv_DomainHandling);
                //
                //Set segmentation specific parameters.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", hv_IgnoreClassIDs);
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "set_background_id", hv_SetBackgroundID);
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "class_ids_background", hv_ClassIDsBackground);
                }
                //
                //Set generic parameters.
                if ((int)(new HTuple(hv_GenParam.TupleNotEqual(new HTuple()))) != 0)
                {
                    hv_GenParamNames.Dispose();
                    HOperatorSet.GetDictParam(hv_GenParam, "keys", new HTuple(), out hv_GenParamNames);
                    for (hv_GenParamIndex = 0; (int)hv_GenParamIndex <= (int)((new HTuple(hv_GenParamNames.TupleLength()
                        )) - 1); hv_GenParamIndex = (int)hv_GenParamIndex + 1)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_GenParamValue.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, hv_GenParamNames.TupleSelect(hv_GenParamIndex),
                                out hv_GenParamValue);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, hv_GenParamNames.TupleSelect(
                                hv_GenParamIndex), hv_GenParamValue);
                        }
                    }
                }
                //
                //Set necessary default values.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("detection"))) != 0)
                {
                    hv_KeysExist.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", (new HTuple("instance_type")).TupleConcat(
                        "ignore_direction"), out hv_KeysExist);
                    if ((int)(((hv_KeysExist.TupleSelect(0))).TupleNot()) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "instance_type", "rectangle1");
                    }
                    //Set default for 'ignore_direction' only if instance_type is 'rectangle2'.
                    hv_InstanceType.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "instance_type", out hv_InstanceType);
                    if ((int)((new HTuple(hv_InstanceType.TupleEqual("rectangle2"))).TupleAnd(
                        ((hv_KeysExist.TupleSelect(1))).TupleNot())) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "ignore_direction", 0);
                    }
                }
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //

                hv_GenParamNames.Dispose();
                hv_GenParamIndex.Dispose();
                hv_GenParamValue.Dispose();
                hv_KeysExist.Dispose();
                hv_InstanceType.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_GenParamNames.Dispose();
                hv_GenParamIndex.Dispose();
                hv_GenParamValue.Dispose();
                hv_KeysExist.Dispose();
                hv_InstanceType.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region check_dl_preprocess_param
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
        #endregion

        #region replace_legacy_preprocessing_parameters
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
        #endregion

        #region tuple_shuffle
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
        #endregion

        #region remove_dir_recursively
        // Chapter: File / Misc
        // Short Description: This procedure removes a directory recursively. 
        public void remove_dir_recursively(HTuple hv_DirName)
        {



            // Local control variables 

            HTuple hv_Dirs = new HTuple(), hv_I = new HTuple();
            HTuple hv_Files = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //Recursively delete all subdirectories.
                hv_Dirs.Dispose();
                HOperatorSet.ListFiles(hv_DirName, "directories", out hv_Dirs);
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Dirs.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        remove_dir_recursively(hv_Dirs.TupleSelect(hv_I));
                    }
                }
                //Delete all files.
                hv_Files.Dispose();
                HOperatorSet.ListFiles(hv_DirName, "files", out hv_Files);
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Files.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.DeleteFile(hv_Files.TupleSelect(hv_I));
                    }
                }
                //Remove empty directory.
                HOperatorSet.RemoveDir(hv_DirName);

                hv_Dirs.Dispose();
                hv_I.Dispose();
                hv_Files.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Dirs.Dispose();
                hv_I.Dispose();
                hv_Files.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region gen_dl_samples(The procedure returns DLSample dicts for given sample indices of a DLDataset)
        // Chapter: Deep Learning / Model
        // Short Description: The procedure returns DLSample dicts for given sample indices of a DLDataset. 
        public void gen_dl_samples(HTuple hv_DLDataset, HTuple hv_SampleIndices, HTuple hv_RestrictKeysDLSample,
            HTuple hv_GenParam, out HTuple hv_DLSampleBatch)
        {



            // Local iconic variables 

            HObject ho_ImageRaw = null, ho_ImageAnomaly = null;
            HObject ho_RegionAnomaly = null, ho_ImageSegmentation = null;

            // Local control variables 

            HTuple hv_ImageDir = new HTuple(), hv_DLSamples = new HTuple();
            HTuple hv_MinIndex = new HTuple(), hv_MaxIndex = new HTuple();
            HTuple hv_InstanceType = new HTuple(), hv_IgnoreMissing = new HTuple();
            HTuple hv_GenParamName = new HTuple(), hv_IndexGenParam = new HTuple();
            HTuple hv_DLSamplesProc = new HTuple(), hv_BboxKeyList = new HTuple();
            HTuple hv_ImageIndex = new HTuple(), hv_DLSample = new HTuple();
            HTuple hv_ImageID = new HTuple(), hv_ImageName = new HTuple();
            HTuple hv_FileName = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_AnomalyLabelExists = new HTuple(), hv_AnomalyLabel = new HTuple();
            HTuple hv_AnomalyFileNameExists = new HTuple(), hv_AnomalyDir = new HTuple();
            HTuple hv_AnomalyFileName = new HTuple(), hv_ExceptionImageAnomaly = new HTuple();
            HTuple hv_ExceptionRegionAnomaly = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_ImageLabelIDExists = new HTuple();
            HTuple hv_ImageLabelID = new HTuple(), hv_BboxExists = new HTuple();
            HTuple hv_BboxLabels = new HTuple(), hv_KeysExist = new HTuple();
            HTuple hv_MissingKeyIndices = new HTuple(), hv_IndexParam = new HTuple();
            HTuple hv_BboxCoord = new HTuple(), hv_SegKeyExists = new HTuple();
            HTuple hv_SegmentationDir = new HTuple(), hv_SegmentationName = new HTuple();
            HTuple hv_ExceptionSegmentation = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageRaw);
            HOperatorSet.GenEmptyObj(out ho_ImageAnomaly);
            HOperatorSet.GenEmptyObj(out ho_RegionAnomaly);
            HOperatorSet.GenEmptyObj(out ho_ImageSegmentation);
            hv_DLSampleBatch = new HTuple();
            try
            {
                //
                //This procedure creates DLSampleBatch, a tuple of DLSample dictionaries, with
                //the image data for each DLDataset sample, that was selected through SampleIndices.
                //The keys to be transferred can be restricted using RestrictKeysDLSample,
                //which is switched off ('off') by default.
                //The procedure returns all generated DLSample dictionaries in the tuple
                //DLSampleBatch.
                //Setting the GenParam 'ignore_missing_labels' controls whether an error is thrown,
                //if no ground truth annotation information is available for a given image.
                //
                //Get the image directory.
                hv_ImageDir.Dispose();
                HOperatorSet.GetDictTuple(hv_DLDataset, "image_dir", out hv_ImageDir);
                //
                //Get the samples from the DLDataset.
                hv_DLSamples.Dispose();
                HOperatorSet.GetDictTuple(hv_DLDataset, "samples", out hv_DLSamples);
                //
                //Check the input values.
                //
                //Check that the given indices are valid.
                hv_MinIndex.Dispose();
                HOperatorSet.TupleMin(hv_SampleIndices, out hv_MinIndex);
                hv_MaxIndex.Dispose();
                HOperatorSet.TupleMax(hv_SampleIndices, out hv_MaxIndex);
                if ((int)((new HTuple(hv_MinIndex.TupleLess(0))).TupleOr(new HTuple(hv_MaxIndex.TupleGreater(
                    (new HTuple(hv_DLSamples.TupleLength())) - 1)))) != 0)
                {
                    throw new HalconException("The given SampleIndices are not within the range of available samples in DLDataset.");
                }
                //
                //Check if the given method is valid.
                if ((int)(new HTuple((new HTuple(hv_RestrictKeysDLSample.TupleLength())).TupleEqual(
                    1))) != 0)
                {
                    if ((int)(new HTuple((new HTuple((new HTuple(((((((new HTuple("anomaly_detection")).TupleConcat(
                        "detection")).TupleConcat("segmentation")).TupleConcat("classification")).TupleConcat(
                        "image_only")).TupleConcat("off")).TupleFind(hv_RestrictKeysDLSample))).TupleMax()
                        )).TupleEqual(-1))) != 0)
                    {
                        throw new HalconException("Unknown RestrictKeysDLSample : " + hv_RestrictKeysDLSample);
                    }
                }
                else
                {
                    throw new HalconException("RestrictKeysDLSample must be specified by one string.");
                }
                //
                //Generic Parameters.
                //Set default values.
                hv_InstanceType.Dispose();
                hv_InstanceType = "rectangle1";
                //For missing labels an error is thrown.
                if ((int)(new HTuple(hv_RestrictKeysDLSample.TupleEqual("off"))) != 0)
                {
                    hv_IgnoreMissing.Dispose();
                    hv_IgnoreMissing = 1;
                }
                else
                {
                    hv_IgnoreMissing.Dispose();
                    hv_IgnoreMissing = 0;
                }
                //
                //Transfer generic parameters.
                if ((int)(new HTuple(hv_GenParam.TupleNotEqual(new HTuple()))) != 0)
                {
                    hv_GenParamName.Dispose();
                    HOperatorSet.GetDictParam(hv_GenParam, "keys", new HTuple(), out hv_GenParamName);
                    for (hv_IndexGenParam = 0; (int)hv_IndexGenParam <= (int)((new HTuple(hv_GenParamName.TupleLength()
                        )) - 1); hv_IndexGenParam = (int)hv_IndexGenParam + 1)
                    {
                        if ((int)(new HTuple(((hv_GenParamName.TupleSelect(hv_IndexGenParam))).TupleEqual(
                            "ignore_missing_labels"))) != 0)
                        {
                            hv_IgnoreMissing.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, "ignore_missing_labels", out hv_IgnoreMissing);
                            if ((int)((new HTuple((new HTuple(hv_IgnoreMissing.TupleEqual(1))).TupleOr(
                                new HTuple(hv_IgnoreMissing.TupleEqual(0))))).TupleNot()) != 0)
                            {
                                throw new HalconException("The GenParam ignore_missing_labels must be true or false.");
                            }
                        }
                        else if ((int)(new HTuple(((hv_GenParamName.TupleSelect(hv_IndexGenParam))).TupleEqual(
                            "instance_type"))) != 0)
                        {
                            if ((int)(new HTuple((new HTuple((new HTuple(((new HTuple("detection")).TupleConcat(
                                "off")).TupleFind(hv_RestrictKeysDLSample))).TupleMax())).TupleEqual(
                                -1))) != 0)
                            {
                                throw new HalconException("The GenParam instance_type can only be set for RestrictKeysDLSample detection or off.");
                            }
                            hv_InstanceType.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, "instance_type", out hv_InstanceType);
                            if ((int)(new HTuple((new HTuple((new HTuple(((new HTuple("rectangle1")).TupleConcat(
                                "rectangle2")).TupleFind(hv_InstanceType))).TupleMax())).TupleEqual(
                                -1))) != 0)
                            {
                                throw new HalconException("The GenParam instance_type must be either 'rectangle1' or 'rectangle2'.");
                            }
                        }
                        else
                        {
                            throw new HalconException("Unknown GenParam key : " + (hv_GenParamName.TupleSelect(
                                hv_IndexGenParam)));
                        }
                    }
                }
                //
                //Get the samples to be processed.
                hv_DLSamplesProc.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DLSamplesProc = hv_DLSamples.TupleSelect(
                        hv_SampleIndices);
                }
                //
                //Initialize the tuple for collection the DLSample dictionaries.
                hv_DLSampleBatch.Dispose();
                hv_DLSampleBatch = new HTuple();
                //
                //Set the BboxKeyList according to the InstanceType.
                if ((int)(new HTuple((new HTuple((new HTuple(((new HTuple("detection")).TupleConcat(
                    "off")).TupleFind(hv_RestrictKeysDLSample))).TupleMax())).TupleNotEqual(
                    -1))) != 0)
                {
                    hv_BboxKeyList.Dispose();
                    hv_BboxKeyList = new HTuple();
                    hv_BboxKeyList[0] = "bbox_col1";
                    hv_BboxKeyList[1] = "bbox_row1";
                    hv_BboxKeyList[2] = "bbox_col2";
                    hv_BboxKeyList[3] = "bbox_row2";
                    if ((int)(new HTuple(hv_InstanceType.TupleEqual("rectangle2"))) != 0)
                    {
                        hv_BboxKeyList.Dispose();
                        hv_BboxKeyList = new HTuple();
                        hv_BboxKeyList[0] = "bbox_row";
                        hv_BboxKeyList[1] = "bbox_col";
                        hv_BboxKeyList[2] = "bbox_length1";
                        hv_BboxKeyList[3] = "bbox_length2";
                        hv_BboxKeyList[4] = "bbox_phi";
                    }
                }
                //Loop over all selected samples and create a DLSample dictionary
                //for each dictionary in the DLDataset samples.
                for (hv_ImageIndex = 0; (int)hv_ImageIndex <= (int)((new HTuple(hv_SampleIndices.TupleLength())) - 1); hv_ImageIndex = (int)hv_ImageIndex + 1)            
                {
                    //
                    //Create the DLSample dictionary
                    hv_DLSample.Dispose();
                    HOperatorSet.CreateDict(out hv_DLSample);
                    //
                    //Set the image key.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageID.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSamplesProc.TupleSelect(hv_ImageIndex), "image_id",
                            out hv_ImageID);
                    }
                    HOperatorSet.SetDictTuple(hv_DLSample, "image_id", hv_ImageID);
                    //
                    //Read image.
                    //The relative file path of the image is specified in image_name.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageName.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSamplesProc.TupleSelect(hv_ImageIndex), "image_file_name",
                            out hv_ImageName);
                    }
                    //
                    if ((int)(new HTuple(((hv_ImageDir.TupleStrlen())).TupleEqual(0))) != 0)
                    {
                        hv_FileName.Dispose();
                        hv_FileName = new HTuple(hv_ImageName);
                    }
                    else
                    {
                        hv_FileName.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_FileName = (hv_ImageDir + "/") + hv_ImageName;
                        }
                    }
                    try
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_ImageRaw.Dispose();
                            HOperatorSet.ReadImage(out ho_ImageRaw, hv_FileName);
                            //Insert image into dictionary.
                            HOperatorSet.SetDictObject(ho_ImageRaw, hv_DLSample, "image");

                            //ADD FOR DISPLAY
                            DispImageAdaptively(ref hv_HWindowControl, ho_ImageRaw);
                            if (hv_Progress_ForDisplay.Length != 0)
                            {
                                hv_Progress_ForDisplay[3] = "Images Count:" + loopCount + "/" + imageCount;
                                HOperatorSet.DispText(hv_HWindowControl.HalconWindow, hv_Progress_ForDisplay, "window", "top", "left", "red", "box", "true");
                            }
                        }
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        throw new HalconException((((("Error for reading/setting image " + hv_FileName) + " with ID ") + hv_ImageID) + " : Error code ") + (hv_Exception.TupleSelect(
                            0)));
                    }
                    //
                    //Read specific data.
                    //
                    if ((int)(new HTuple(hv_RestrictKeysDLSample.TupleNotEqual("image_only"))) != 0)
                    {
                        //
                        //Transfer anomaly detection relevant data.
                        if ((int)(new HTuple((new HTuple((new HTuple(((new HTuple("anomaly_detection")).TupleConcat(
                            "off")).TupleFind(hv_RestrictKeysDLSample))).TupleMax())).TupleNotEqual(
                            -1))) != 0)
                        {
                            //Check the existence of the label key.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_AnomalyLabelExists.Dispose();
                                HOperatorSet.GetDictParam(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                    "key_exists", "anomaly_label", out hv_AnomalyLabelExists);
                            }
                            if ((int)(hv_AnomalyLabelExists) != 0)
                            {
                                //Get the image label.
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_AnomalyLabel.Dispose();
                                    HOperatorSet.GetDictTuple(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                        "anomaly_label", out hv_AnomalyLabel);
                                }
                                //Check the existence of the anomaly file name key. If not found it is just ignored.
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_AnomalyFileNameExists.Dispose();
                                    HOperatorSet.GetDictParam(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                        "key_exists", "anomaly_file_name", out hv_AnomalyFileNameExists);
                                }
                                if ((int)(hv_AnomalyFileNameExists) != 0)
                                {
                                    //Get the ground truth anomaly directory.
                                    hv_AnomalyDir.Dispose();
                                    HOperatorSet.GetDictTuple(hv_DLDataset, "anomaly_dir", out hv_AnomalyDir);
                                    //Get the image file name.
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_AnomalyFileName.Dispose();
                                        HOperatorSet.GetDictTuple(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                            "anomaly_file_name", out hv_AnomalyFileName);
                                    }
                                    //Read the ground truth anomaly image.
                                    try
                                    {
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            ho_ImageAnomaly.Dispose();
                                            HOperatorSet.ReadImage(out ho_ImageAnomaly, (hv_AnomalyDir + "/") + hv_AnomalyFileName);
                                        }
                                    }
                                    // catch (ExceptionImageAnomaly) 
                                    catch (HalconException HDevExpDefaultException1)
                                    {
                                        HDevExpDefaultException1.ToHTuple(out hv_ExceptionImageAnomaly);
                                        //If the file is not an image, try to read the ground truth anomaly region.
                                        //Then, convert this region to a ground truth anomaly image.
                                        try
                                        {
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                ho_RegionAnomaly.Dispose();
                                                HOperatorSet.ReadRegion(out ho_RegionAnomaly, (hv_AnomalyDir + "/") + hv_AnomalyFileName);
                                            }
                                        }
                                        // catch (ExceptionRegionAnomaly) 
                                        catch (HalconException HDevExpDefaultException2)
                                        {
                                            HDevExpDefaultException2.ToHTuple(out hv_ExceptionRegionAnomaly);
                                            throw new HalconException((("Error: Could not read the anomaly ground truth information of image_id " + hv_ImageID) + " : Error code ") + (hv_ExceptionImageAnomaly.TupleSelect(
                                                0)));
                                        }
                                        hv_Width.Dispose(); hv_Height.Dispose();
                                        HOperatorSet.GetImageSize(ho_ImageRaw, out hv_Width, out hv_Height);
                                        ho_ImageAnomaly.Dispose();
                                        HOperatorSet.GenImageConst(out ho_ImageAnomaly, "byte", hv_Width,
                                            hv_Height);
                                        HOperatorSet.OverpaintRegion(ho_ImageAnomaly, ho_ImageAnomaly,
                                            0, "fill");
                                        HOperatorSet.OverpaintRegion(ho_ImageAnomaly, ho_RegionAnomaly,
                                            1, "fill");
                                    }
                                    //Insert anomaly image into DLSample dictionary.
                                    HOperatorSet.SetDictObject(ho_ImageAnomaly, hv_DLSample, "anomaly_ground_truth");
                                }
                                //
                                //Insert anomaly label into DLSample dictionary.
                                HOperatorSet.SetDictTuple(hv_DLSample, "anomaly_label", hv_AnomalyLabel);
                                //Insert anomaly label id into DLSample dictionary.
                                if ((int)(new HTuple(hv_AnomalyLabel.TupleEqual("nok"))) != 0)
                                {
                                    HOperatorSet.SetDictTuple(hv_DLSample, "anomaly_label_id", 1);
                                }
                                else
                                {
                                    HOperatorSet.SetDictTuple(hv_DLSample, "anomaly_label_id", 0);
                                }
                            }
                            else if ((int)((new HTuple(hv_AnomalyLabelExists.TupleNot()
                                )).TupleAnd(hv_IgnoreMissing.TupleNot())) != 0)
                            {
                                throw new HalconException(("For image_id " + hv_ImageID) + " the key 'anomaly_label' is missing. Missing keys can be ignored using the GenParam ignore_missing_labels.");
                            }
                        }
                        //
                        //Transfer classification relevant data.
                        if ((int)(new HTuple((new HTuple((new HTuple(((new HTuple("classification")).TupleConcat(
                            "off")).TupleFind(hv_RestrictKeysDLSample))).TupleMax())).TupleNotEqual(
                            -1))) != 0)
                        {
                            //Check the existence of the required key.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ImageLabelIDExists.Dispose();
                                HOperatorSet.GetDictParam(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                    "key_exists", "image_label_id", out hv_ImageLabelIDExists);
                            }
                            if ((int)(hv_ImageLabelIDExists) != 0)
                            {
                                //Transfer the image label.
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_ImageLabelID.Dispose();
                                    HOperatorSet.GetDictTuple(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                        "image_label_id", out hv_ImageLabelID);
                                }
                                HOperatorSet.SetDictTuple(hv_DLSample, "image_label_id", hv_ImageLabelID);
                            }
                            else if ((int)((new HTuple(hv_ImageLabelIDExists.TupleNot()
                                )).TupleAnd(hv_IgnoreMissing.TupleNot())) != 0)
                            {
                                throw new HalconException(("For image_id " + hv_ImageID) + " the key 'image_label_id' is missing. Missing keys can be ignored using the GenParam ignore_missing_labels.");
                            }
                        }
                        //
                        //Transfer detection relevant data.
                        if ((int)(new HTuple((new HTuple((new HTuple(((new HTuple("detection")).TupleConcat(
                            "off")).TupleFind(hv_RestrictKeysDLSample))).TupleMax())).TupleNotEqual(
                            -1))) != 0)
                        {
                            //Check the existence of the required key.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_BboxExists.Dispose();
                                HOperatorSet.GetDictParam(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                    "key_exists", "bbox_label_id", out hv_BboxExists);
                            }
                            if ((int)(hv_BboxExists) != 0)
                            {
                                //Transfer the bounding box labels.
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_BboxLabels.Dispose();
                                    HOperatorSet.GetDictTuple(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                        "bbox_label_id", out hv_BboxLabels);
                                }
                                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_label_id", hv_BboxLabels);
                                //Transfer the bounding box coordinates.
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_KeysExist.Dispose();
                                    HOperatorSet.GetDictParam(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                        "key_exists", hv_BboxKeyList, out hv_KeysExist);
                                }
                                if ((int)((new HTuple(((hv_KeysExist.TupleSum())).TupleNotEqual(new HTuple(hv_KeysExist.TupleLength()
                                    )))).TupleAnd(hv_IgnoreMissing.TupleNot())) != 0)
                                {
                                    hv_MissingKeyIndices.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_MissingKeyIndices = ((hv_KeysExist.TupleEqualElem(
                                            0))).TupleFind(1);
                                    }
                                    throw new HalconException((("For image_id " + hv_ImageID) + new HTuple(", an error has occurred when transferring the key ")) + (hv_BboxKeyList.TupleSelect(
                                        hv_MissingKeyIndices)));
                                }
                                for (hv_IndexParam = 0; (int)hv_IndexParam <= (int)((new HTuple(hv_BboxKeyList.TupleLength()
                                    )) - 1); hv_IndexParam = (int)hv_IndexParam + 1)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_BboxCoord.Dispose();
                                        HOperatorSet.GetDictTuple(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                            hv_BboxKeyList.TupleSelect(hv_IndexParam), out hv_BboxCoord);
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.SetDictTuple(hv_DLSample, hv_BboxKeyList.TupleSelect(
                                            hv_IndexParam), hv_BboxCoord);
                                    }
                                }
                            }
                            else if ((int)(hv_IgnoreMissing.TupleNot()) != 0)
                            {
                                throw new HalconException(("For image_id " + hv_ImageID) + " there is no key bbox_label_id. Missing keys can be ignored using the GenParam ignore_missing_labels.");
                            }
                        }
                        //
                        //Transfer segmentation relevant data.
                        if ((int)(new HTuple((new HTuple((new HTuple(((new HTuple("segmentation")).TupleConcat(
                            "off")).TupleFind(hv_RestrictKeysDLSample))).TupleMax())).TupleNotEqual(
                            -1))) != 0)
                        {
                            //Check the existence of the required keys.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_SegKeyExists.Dispose();
                                HOperatorSet.GetDictParam(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                    "key_exists", "segmentation_file_name", out hv_SegKeyExists);
                            }
                            if ((int)(hv_SegKeyExists) != 0)
                            {
                                //Get the ground truth segmentation directory.
                                hv_SegmentationDir.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLDataset, "segmentation_dir", out hv_SegmentationDir);
                                //Get the image file name.
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_SegmentationName.Dispose();
                                    HOperatorSet.GetDictTuple(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                        "segmentation_file_name", out hv_SegmentationName);
                                }
                                //Read the ground truth segmentation image.
                                try
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        ho_ImageSegmentation.Dispose();
                                        HOperatorSet.ReadImage(out ho_ImageSegmentation, (hv_SegmentationDir + "/") + hv_SegmentationName);
                                    }
                                }
                                // catch (ExceptionSegmentation) 
                                catch (HalconException HDevExpDefaultException1)
                                {
                                    HDevExpDefaultException1.ToHTuple(out hv_ExceptionSegmentation);
                                    throw new HalconException((("Error for reading segmentation file of image_id " + hv_ImageID) + " : Error code ") + (hv_ExceptionSegmentation.TupleSelect(
                                        0)));
                                }
                                //Insert image into DLSample dictionary.
                                HOperatorSet.SetDictObject(ho_ImageSegmentation, hv_DLSample, "segmentation_image");
                            }
                            else if ((int)(hv_IgnoreMissing.TupleNot()) != 0)
                            {
                                throw new HalconException(("For image_id " + hv_ImageID) + " there is no key segmentation_file_name. Missing keys can be ignored using the GenParam ignore_missing_labels.");
                            }
                        }
                    }
                    //
                    //Collect all data dictionaries of all processed indices.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_DLSampleBatch = hv_DLSampleBatch.TupleConcat(
                                hv_DLSample);
                            hv_DLSampleBatch.Dispose();
                            hv_DLSampleBatch = ExpTmpLocalVar_DLSampleBatch;
                        }
                    }
                }





                ho_ImageRaw.Dispose();
                ho_ImageAnomaly.Dispose();
                ho_RegionAnomaly.Dispose();
                ho_ImageSegmentation.Dispose();

                hv_ImageDir.Dispose();
                hv_DLSamples.Dispose();
                hv_MinIndex.Dispose();
                hv_MaxIndex.Dispose();
                hv_InstanceType.Dispose();
                hv_IgnoreMissing.Dispose();
                hv_GenParamName.Dispose();
                hv_IndexGenParam.Dispose();
                hv_DLSamplesProc.Dispose();
                hv_BboxKeyList.Dispose();
                hv_ImageIndex.Dispose();
                hv_DLSample.Dispose();
                hv_ImageID.Dispose();
                hv_ImageName.Dispose();
                hv_FileName.Dispose();
                hv_Exception.Dispose();
                hv_AnomalyLabelExists.Dispose();
                hv_AnomalyLabel.Dispose();
                hv_AnomalyFileNameExists.Dispose();
                hv_AnomalyDir.Dispose();
                hv_AnomalyFileName.Dispose();
                hv_ExceptionImageAnomaly.Dispose();
                hv_ExceptionRegionAnomaly.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_ImageLabelIDExists.Dispose();
                hv_ImageLabelID.Dispose();
                hv_BboxExists.Dispose();
                hv_BboxLabels.Dispose();
                hv_KeysExist.Dispose();
                hv_MissingKeyIndices.Dispose();
                hv_IndexParam.Dispose();
                hv_BboxCoord.Dispose();
                hv_SegKeyExists.Dispose();
                hv_SegmentationDir.Dispose();
                hv_SegmentationName.Dispose();
                hv_ExceptionSegmentation.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageRaw.Dispose();
                ho_ImageAnomaly.Dispose();
                ho_RegionAnomaly.Dispose();
                ho_ImageSegmentation.Dispose();

                hv_ImageDir.Dispose();
                hv_DLSamples.Dispose();
                hv_MinIndex.Dispose();
                hv_MaxIndex.Dispose();
                hv_InstanceType.Dispose();
                hv_IgnoreMissing.Dispose();
                hv_GenParamName.Dispose();
                hv_IndexGenParam.Dispose();
                hv_DLSamplesProc.Dispose();
                hv_BboxKeyList.Dispose();
                hv_ImageIndex.Dispose();
                hv_DLSample.Dispose();
                hv_ImageID.Dispose();
                hv_ImageName.Dispose();
                hv_FileName.Dispose();
                hv_Exception.Dispose();
                hv_AnomalyLabelExists.Dispose();
                hv_AnomalyLabel.Dispose();
                hv_AnomalyFileNameExists.Dispose();
                hv_AnomalyDir.Dispose();
                hv_AnomalyFileName.Dispose();
                hv_ExceptionImageAnomaly.Dispose();
                hv_ExceptionRegionAnomaly.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_ImageLabelIDExists.Dispose();
                hv_ImageLabelID.Dispose();
                hv_BboxExists.Dispose();
                hv_BboxLabels.Dispose();
                hv_KeysExist.Dispose();
                hv_MissingKeyIndices.Dispose();
                hv_IndexParam.Dispose();
                hv_BboxCoord.Dispose();
                hv_SegKeyExists.Dispose();
                hv_SegmentationDir.Dispose();
                hv_SegmentationName.Dispose();
                hv_ExceptionSegmentation.Dispose();

                throw HDevExpDefaultException;
            }
        }

        #endregion

        #region preprocess_dl_samples
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
        #endregion

        #region write_dl_samples
        // Chapter: Deep Learning / Model
        // Short Description: Write the dictionaries of the samples in DLSampleBatch to hdict files and store the paths in DLDataset. 
        public void write_dl_samples(HTuple hv_DLDataset, HTuple hv_SampleIndices, HTuple hv_DLSampleBatch, HTuple hv_GenParamName, HTuple hv_GenParamValue)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_RaiseErrorWriteDict = new HTuple();
            HTuple hv_IndexGenParam = new HTuple(), hv_DLSampleDirExists = new HTuple();
            HTuple hv_OutDir = new HTuple(), hv_DatasetSamples = new HTuple();
            HTuple hv_I = new HTuple(), hv_DLDatasetIndex = new HTuple();
            HTuple hv_DatasetSample = new HTuple(), hv_DLSample = new HTuple();
            HTuple hv_DatasetImageID = new HTuple(), hv_SampleImageID = new HTuple();
            HTuple hv_FileNameOut = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //
                //This procedure writes all given DLSamples in DLSampleBatch to hdict files
                //and stores the file paths in the respective samples of the DLDataset.
                //The directory needs to be given in dlsample_dir, before calling this procedure.
                //
                //The output filename is created in the following way: image_id + '_dlsample.hdict'
                //
                //Set the default values.
                //Raise error when writing dictionary.
                hv_RaiseErrorWriteDict.Dispose();
                hv_RaiseErrorWriteDict = "true";
                //
                //Transfer generic parameters.
                if ((int)(new HTuple((new HTuple(hv_GenParamName.TupleLength())).TupleNotEqual(
                    new HTuple(hv_GenParamValue.TupleLength())))) != 0)
                {
                    throw new HalconException("GenParamName and GenParamValue have to be of equal length.");
                }
                //
                if ((int)(new HTuple(hv_GenParamName.TupleNotEqual(new HTuple()))) != 0)
                {
                    for (hv_IndexGenParam = 0; (int)hv_IndexGenParam <= (int)((new HTuple(hv_GenParamName.TupleLength()
                        )) - 1); hv_IndexGenParam = (int)hv_IndexGenParam + 1)
                    {
                        if ((int)(new HTuple(((hv_GenParamName.TupleSelect(hv_IndexGenParam))).TupleEqual(
                            "raise_error_if_content_not_serializable"))) != 0)
                        {
                            //Set 'raise_error_if_content_not_serializable' for writing write_dict.
                            hv_RaiseErrorWriteDict.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_RaiseErrorWriteDict = hv_GenParamValue.TupleSelect(
                                    hv_IndexGenParam);
                            }
                        }
                        else
                        {
                            throw new HalconException("Unknown GenParam key : " + (hv_GenParamName.TupleSelect(
                                hv_IndexGenParam)));
                        }
                    }
                }
                //
                //Check the parameters.
                //Check that the base path is available in the DLDataset.
                hv_DLSampleDirExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLDataset, "key_exists", "dlsample_dir", out hv_DLSampleDirExists);
                if ((int)(hv_DLSampleDirExists.TupleNot()) != 0)
                {
                    throw new HalconException("The dataset needs to include the key 'dlsample_dir'.");

                    hv_RaiseErrorWriteDict.Dispose();
                    hv_IndexGenParam.Dispose();
                    hv_DLSampleDirExists.Dispose();
                    hv_OutDir.Dispose();
                    hv_DatasetSamples.Dispose();
                    hv_I.Dispose();
                    hv_DLDatasetIndex.Dispose();
                    hv_DatasetSample.Dispose();
                    hv_DLSample.Dispose();
                    hv_DatasetImageID.Dispose();
                    hv_SampleImageID.Dispose();
                    hv_FileNameOut.Dispose();

                    return;
                }

                if ((int)(new HTuple((new HTuple(hv_DLSampleBatch.TupleLength())).TupleNotEqual(
                    new HTuple(hv_SampleIndices.TupleLength())))) != 0)
                {
                    throw new HalconException("The input tuples DLSampleBatch and SampleIndices need to match in length.");

                    hv_RaiseErrorWriteDict.Dispose();
                    hv_IndexGenParam.Dispose();
                    hv_DLSampleDirExists.Dispose();
                    hv_OutDir.Dispose();
                    hv_DatasetSamples.Dispose();
                    hv_I.Dispose();
                    hv_DLDatasetIndex.Dispose();
                    hv_DatasetSample.Dispose();
                    hv_DLSample.Dispose();
                    hv_DatasetImageID.Dispose();
                    hv_SampleImageID.Dispose();
                    hv_FileNameOut.Dispose();

                    return;
                }
                //
                //Write preprocessed data.
                //
                //Get the base path for the outputs.
                hv_OutDir.Dispose();
                HOperatorSet.GetDictTuple(hv_DLDataset, "dlsample_dir", out hv_OutDir);
                //
                //Get the samples.
                hv_DatasetSamples.Dispose();
                HOperatorSet.GetDictTuple(hv_DLDataset, "samples", out hv_DatasetSamples);
                //
                //Loop over all samples in the batch.
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_DLSampleBatch.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    //Get the sample dictionaries.
                    hv_DLDatasetIndex.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DLDatasetIndex = hv_SampleIndices.TupleSelect(
                            hv_I);
                    }
                    hv_DatasetSample.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DatasetSample = hv_DatasetSamples.TupleSelect(
                            hv_DLDatasetIndex);
                    }
                    hv_DLSample.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DLSample = hv_DLSampleBatch.TupleSelect(
                            hv_I);
                    }
                    //
                    //Check that image IDs match.
                    hv_DatasetImageID.Dispose();
                    HOperatorSet.GetDictTuple(hv_DatasetSample, "image_id", out hv_DatasetImageID);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SampleImageID.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSampleBatch.TupleSelect(hv_I), "image_id",
                            out hv_SampleImageID);
                    }
                    if ((int)(new HTuple(hv_DatasetImageID.TupleNotEqual(hv_SampleImageID))) != 0)
                    {
                        throw new HalconException("Image IDs do not match. Please use correct indexing in input argument SampleIndices.");

                        hv_RaiseErrorWriteDict.Dispose();
                        hv_IndexGenParam.Dispose();
                        hv_DLSampleDirExists.Dispose();
                        hv_OutDir.Dispose();
                        hv_DatasetSamples.Dispose();
                        hv_I.Dispose();
                        hv_DLDatasetIndex.Dispose();
                        hv_DatasetSample.Dispose();
                        hv_DLSample.Dispose();
                        hv_DatasetImageID.Dispose();
                        hv_SampleImageID.Dispose();
                        hv_FileNameOut.Dispose();

                        return;
                    }
                    //
                    //Generate the output file name.
                    hv_FileNameOut.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FileNameOut = hv_SampleImageID + "_dlsample.hdict";
                    }
                    //
                    //Write output dictionary.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.WriteDict(hv_DLSample, (hv_OutDir + "/") + hv_FileNameOut, "raise_error_if_content_not_serializable",
                            hv_RaiseErrorWriteDict);
                    }
                    //Add output path to DLDataset sample dictionary.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetDictTuple(hv_DatasetSamples.TupleSelect(hv_DLDatasetIndex),
                            "dlsample_file_name", hv_FileNameOut);
                    }
                    //
                }


                hv_RaiseErrorWriteDict.Dispose();
                hv_IndexGenParam.Dispose();
                hv_DLSampleDirExists.Dispose();
                hv_OutDir.Dispose();
                hv_DatasetSamples.Dispose();
                hv_I.Dispose();
                hv_DLDatasetIndex.Dispose();
                hv_DatasetSample.Dispose();
                hv_DLSample.Dispose();
                hv_DatasetImageID.Dispose();
                hv_SampleImageID.Dispose();
                hv_FileNameOut.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_RaiseErrorWriteDict.Dispose();
                hv_IndexGenParam.Dispose();
                hv_DLSampleDirExists.Dispose();
                hv_OutDir.Dispose();
                hv_DatasetSamples.Dispose();
                hv_I.Dispose();
                hv_DLDatasetIndex.Dispose();
                hv_DatasetSample.Dispose();
                hv_DLSample.Dispose();
                hv_DatasetImageID.Dispose();
                hv_SampleImageID.Dispose();
                hv_FileNameOut.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region estimate_progress
        // Chapter: Time
        // Short Description: Estimate the remaining time for a task given the current progress. 
        public void estimate_progress(HTuple hv_SecondsStart, HTuple hv_ProgressMin, HTuple hv_ProgressCurrent, HTuple hv_ProgressMax, out HTuple hv_SecondsElapsed, out HTuple hv_SecondsRemaining, out HTuple hv_ProgressPercent, out HTuple hv_ProgressPerSecond)
        {

            // Local iconic variables 

            // Local control variables 

            HTuple hv_SecondsNow = new HTuple(), hv_Epsilon = new HTuple();
            HTuple hv_ProgressRemaining = new HTuple();
            // Initialize local and output iconic variables 
            hv_SecondsElapsed = new HTuple();
            hv_SecondsRemaining = new HTuple();
            hv_ProgressPercent = new HTuple();
            hv_ProgressPerSecond = new HTuple();
            try
            {
                //
                //This procedure estimates the remaining time in seconds,
                //given a start time and a progress value.
                //
                //Get current time.
                hv_SecondsNow.Dispose();
                HOperatorSet.CountSeconds(out hv_SecondsNow);
                //
                //Get elapsed time span.
                hv_SecondsElapsed.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsElapsed = hv_SecondsNow - hv_SecondsStart;
                }
                //
                //A very small additive constant to avoid division by zero.
                hv_Epsilon.Dispose();
                hv_Epsilon = 1e-6;
                //
                //Estimate remaining time based on elapsed time.
                hv_ProgressRemaining.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ProgressRemaining = hv_ProgressMax - hv_ProgressCurrent;
                }
                hv_ProgressPerSecond.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ProgressPerSecond = (((hv_ProgressCurrent - hv_ProgressMin)).TupleReal()
                        ) / ((hv_SecondsElapsed.TupleReal()) + hv_Epsilon);
                }
                hv_SecondsRemaining.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsRemaining = hv_ProgressRemaining / (hv_ProgressPerSecond + hv_Epsilon);
                }
                //
                //Get current progress in percent.
                hv_ProgressPercent.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ProgressPercent = (100 * (((hv_ProgressCurrent - hv_ProgressMin)).TupleReal()
                        )) / ((((hv_ProgressMax - hv_ProgressMin)).TupleReal()) + hv_Epsilon);
                }

                hv_SecondsNow.Dispose();
                hv_Epsilon.Dispose();
                hv_ProgressRemaining.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_SecondsNow.Dispose();
                hv_Epsilon.Dispose();
                hv_ProgressRemaining.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region timespan_string
        // Chapter: Time
        // Short Description: Create a formatted string of a time span. 
        public void timespan_string(HTuple hv_TotalSeconds, HTuple hv_Format, out HTuple hv_TimeString)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Seconds = new HTuple(), hv_TotalMinutes = new HTuple();
            HTuple hv_Minutes = new HTuple(), hv_TotalHours = new HTuple();
            HTuple hv_Hours = new HTuple(), hv_Days = new HTuple();
            HTuple hv_TotalSeconds_COPY_INP_TMP = new HTuple(hv_TotalSeconds);

            // Initialize local and output iconic variables 
            hv_TimeString = new HTuple();
            try
            {
                //
                //This procedure creates a readable representation of a time span
                //given the elapsed time in seconds.
                //
                //Ensure that the input is an integer.
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_TotalSeconds = hv_TotalSeconds_COPY_INP_TMP.TupleInt()
                            ;
                        hv_TotalSeconds_COPY_INP_TMP.Dispose();
                        hv_TotalSeconds_COPY_INP_TMP = ExpTmpLocalVar_TotalSeconds;
                    }
                }
                //
                hv_Seconds.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Seconds = hv_TotalSeconds_COPY_INP_TMP % 60;
                }
                //
                hv_TotalMinutes.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TotalMinutes = hv_TotalSeconds_COPY_INP_TMP / 60;
                }
                hv_Minutes.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Minutes = hv_TotalMinutes % 60;
                }
                //
                hv_TotalHours.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TotalHours = hv_TotalSeconds_COPY_INP_TMP / 3600;
                }
                hv_Hours.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Hours = hv_TotalHours % 24;
                }
                //
                hv_Days.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Days = hv_TotalSeconds_COPY_INP_TMP / 86400;
                }
                //
                if ((int)(new HTuple(hv_Format.TupleEqual("auto"))) != 0)
                {
                    //Print the highest non-zero unit and all remaining sub-units.
                    if ((int)(new HTuple(hv_Days.TupleGreater(0))) != 0)
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (((((((hv_Days.TupleString(
                                "d")) + "d ") + (hv_Hours.TupleString("d"))) + "h ") + (hv_Minutes.TupleString(
                                "d"))) + "m ") + (hv_Seconds.TupleString("d"))) + "s";
                        }
                    }
                    else if ((int)(new HTuple(hv_Hours.TupleGreater(0))) != 0)
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (((((hv_Hours.TupleString(
                                "d")) + "h ") + (hv_Minutes.TupleString("d"))) + "m ") + (hv_Seconds.TupleString(
                                "d"))) + "s";
                        }
                    }
                    else if ((int)(new HTuple(hv_Minutes.TupleGreater(0))) != 0)
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (((hv_Minutes.TupleString(
                                "d")) + "m ") + (hv_Seconds.TupleString("d"))) + "s";
                        }
                    }
                    else
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (hv_Seconds.TupleString(
                                "d")) + "s";
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Format.TupleEqual("top1"))) != 0)
                {
                    //Print the highest non-zero unit.
                    if ((int)(new HTuple(hv_Days.TupleGreater(0))) != 0)
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (hv_Days.TupleString(
                                "d")) + "d";
                        }
                    }
                    else if ((int)(new HTuple(hv_Hours.TupleGreater(0))) != 0)
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (hv_Hours.TupleString(
                                "d")) + "h";
                        }
                    }
                    else if ((int)(new HTuple(hv_Minutes.TupleGreater(0))) != 0)
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (hv_Minutes.TupleString(
                                "d")) + "m";
                        }
                    }
                    else
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (hv_Seconds.TupleString(
                                "d")) + "s";
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Format.TupleEqual("top2"))) != 0)
                {
                    //Print the highest non-zero unit and the following sub-unit.
                    if ((int)(new HTuple(hv_Days.TupleGreater(0))) != 0)
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (((hv_Days.TupleString(
                                "d")) + "d ") + (hv_Hours.TupleString("d"))) + "h";
                        }
                    }
                    else if ((int)(new HTuple(hv_Hours.TupleGreater(0))) != 0)
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (((hv_Hours.TupleString(
                                "d")) + "h ") + (hv_Minutes.TupleString("d"))) + "m";
                        }
                    }
                    else if ((int)(new HTuple(hv_Minutes.TupleGreater(0))) != 0)
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (((hv_Minutes.TupleString(
                                "d")) + "m ") + (hv_Seconds.TupleString("d"))) + "s";
                        }
                    }
                    else
                    {
                        hv_TimeString.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TimeString = (hv_Seconds.TupleString(
                                "d")) + "s";
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Format.TupleEqual("dhms"))) != 0)
                {
                    //Print a Days-Hours-Minutes-Seconds string.
                    hv_TimeString.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_TimeString = (((((((hv_Days.TupleString(
                            "d")) + "d ") + (hv_Hours.TupleString("d"))) + "h ") + (hv_Minutes.TupleString(
                            "d"))) + "m ") + (hv_Seconds.TupleString("d"))) + "s";
                    }
                }
                else if ((int)(new HTuple(hv_Format.TupleEqual("hms"))) != 0)
                {
                    //Print a Hours-Minutes-Seconds string, where hours can be >= 24.
                    hv_TimeString.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_TimeString = (((((hv_TotalHours.TupleString(
                            "d")) + "h ") + (hv_Minutes.TupleString("d"))) + "m ") + (hv_Seconds.TupleString(
                            "d"))) + "s";
                    }
                }
                else
                {
                    throw new HalconException("Unknown format string.");
                }
                //

                hv_TotalSeconds_COPY_INP_TMP.Dispose();
                hv_Seconds.Dispose();
                hv_TotalMinutes.Dispose();
                hv_Minutes.Dispose();
                hv_TotalHours.Dispose();
                hv_Hours.Dispose();
                hv_Days.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_TotalSeconds_COPY_INP_TMP.Dispose();
                hv_Seconds.Dispose();
                hv_TotalMinutes.Dispose();
                hv_Minutes.Dispose();
                hv_TotalHours.Dispose();
                hv_Hours.Dispose();
                hv_Days.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region calculate_dl_segmentation_class_weights
        // Chapter: Deep Learning / Semantic Segmentation
        // Short Description: Calculate the class weights for a semantic segmentation dataset. 
        public void calculate_dl_segmentation_class_weights(HTuple hv_DLDataset, HTuple hv_MaxWeight, HTuple hv_IgnoreClassIDs, out HTuple hv_ClassWeights)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_SegmentationImage = null;

            // Local control variables 

            HTuple hv_KeysExists = new HTuple(), hv_DLSamples = new HTuple();
            HTuple hv_SampleIndices = new HTuple(), hv_ClassIDs = new HTuple();
            HTuple hv_NumClasses = new HTuple(), hv_ClassIDsToClassIdx = new HTuple();
            HTuple hv_ClassAreas = new HTuple(), hv_SampleIndex = new HTuple();
            HTuple hv_DLSample = new HTuple(), hv_ImageType = new HTuple();
            HTuple hv_AbsoluteHisto = new HTuple(), hv_TotalArea = new HTuple();
            HTuple hv_ValidClasses = new HTuple(), hv_ClassFreq = new HTuple();
            HTuple hv_IndicesToClip = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SegmentationImage);
            hv_ClassWeights = new HTuple();
            try
            {
                //
                //This procedure calculates a weight for each class that is present in the Dataset.
                //The class weights are calculated according to the inverse class frequencies
                //in the training dataset.
                //Therefore, the dataset has to be split before calling this procedure.
                //
                //Check if the input is correct.
                hv_KeysExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLDataset, "key_exists", (new HTuple("samples")).TupleConcat(
                    "class_ids"), out hv_KeysExists);
                if ((int)(((hv_KeysExists.TupleSelect(0))).TupleNot()) != 0)
                {
                    throw new HalconException("DLDataset must contain a key-value pair for 'samples'");
                }
                if ((int)(((hv_KeysExists.TupleSelect(1))).TupleNot()) != 0)
                {
                    throw new HalconException("DLDataset must contain a key-value pair for 'class_ids'");
                }
                if ((int)(new HTuple(hv_MaxWeight.TupleLessEqual(0))) != 0)
                {
                    throw new HalconException("MaxWeight must be greater than 0");
                }
                //
                //Get the samples of the dataset.
                hv_DLSamples.Dispose();
                HOperatorSet.GetDictTuple(hv_DLDataset, "samples", out hv_DLSamples);
                //Get the train samples.
                hv_SampleIndices.Dispose();
                find_dl_samples(hv_DLSamples, "split", "train", "match", out hv_SampleIndices);
                if ((int)(new HTuple((new HTuple(hv_SampleIndices.TupleLength())).TupleEqual(
                    0))) != 0)
                {
                    throw new HalconException("The DLDataset does not contain any samples with value 'train' for key 'split'");
                }
                //
                //Get the class IDs of the dataset.
                hv_ClassIDs.Dispose();
                HOperatorSet.GetDictTuple(hv_DLDataset, "class_ids", out hv_ClassIDs);
                hv_NumClasses.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_NumClasses = new HTuple(hv_ClassIDs.TupleLength()
                        );
                }
                //
                //Define mapping from class ID to class index.
                hv_ClassIDsToClassIdx.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ClassIDsToClassIdx = HTuple.TupleGenConst(
                        (hv_ClassIDs.TupleMax()) + 1, -1);
                }
                if (hv_ClassIDsToClassIdx == null)
                    hv_ClassIDsToClassIdx = new HTuple();
                hv_ClassIDsToClassIdx[hv_ClassIDs] = HTuple.TupleGenSequence(0, (new HTuple(hv_ClassIDs.TupleLength()
                    )) - 1, 1);
                //
                //We want to collect the number of pixels for each class.
                hv_ClassAreas.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ClassAreas = HTuple.TupleGenConst(
                        new HTuple(hv_ClassIDs.TupleLength()), 0);
                }
                //
                //Loop over the samples.
                for (hv_SampleIndex = 0; (int)hv_SampleIndex <= (int)((new HTuple(hv_SampleIndices.TupleLength()
                    )) - 1); hv_SampleIndex = (int)hv_SampleIndex + 1)
                {
                    //
                    //Read the sample.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DLSample.Dispose();
                        read_dl_samples(hv_DLDataset, hv_SampleIndices.TupleSelect(hv_SampleIndex),
                            out hv_DLSample);
                    }
                    //
                    //Get the segmentation image.
                    ho_SegmentationImage.Dispose();
                    HOperatorSet.GetDictObject(out ho_SegmentationImage, hv_DLSample, "segmentation_image");
                    //
                    //Convert the segmentation image if necessary.
                    hv_ImageType.Dispose();
                    HOperatorSet.GetImageType(ho_SegmentationImage, out hv_ImageType);
                    if ((int)(new HTuple((new HTuple(((((new HTuple("int1")).TupleConcat("int2")).TupleConcat(
                        "uint2")).TupleConcat("byte")).TupleFind(hv_ImageType))).TupleEqual(-1))) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConvertImageType(ho_SegmentationImage, out ExpTmpOutVar_0,
                                "uint2");
                            ho_SegmentationImage.Dispose();
                            ho_SegmentationImage = ExpTmpOutVar_0;
                        }
                    }
                    //
                    //Get the number of pixels for each class.
                    hv_AbsoluteHisto.Dispose();
                    HOperatorSet.GrayHistoAbs(ho_SegmentationImage, ho_SegmentationImage, 1,
                        out hv_AbsoluteHisto);
                    //
                    //Accumulate the areas.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ClassAreas = hv_ClassAreas + (hv_AbsoluteHisto.TupleSelect(
                                hv_ClassIDs));
                            hv_ClassAreas.Dispose();
                            hv_ClassAreas = ExpTmpLocalVar_ClassAreas;
                        }
                    }
                }
                //
                //Get the total number of pixels without the area of ignore classes.
                if (hv_ClassAreas == null)
                    hv_ClassAreas = new HTuple();
                hv_ClassAreas[hv_ClassIDsToClassIdx.TupleSelect(hv_IgnoreClassIDs)] = 0;
                hv_TotalArea.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TotalArea = hv_ClassAreas.TupleSum()
                        ;
                }
                //
                //Calculate the inverse class frequencies.
                hv_ClassWeights.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ClassWeights = HTuple.TupleGenConst(
                        new HTuple(hv_ClassIDs.TupleLength()), 0.0);
                }
                hv_ValidClasses.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ValidClasses = ((hv_ClassAreas.TupleNotEqualElem(
                        0))).TupleFind(1);
                }
                hv_ClassFreq.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ClassFreq = hv_ClassAreas / (hv_TotalArea.TupleReal()
                        );
                }
                if (hv_ClassWeights == null)
                    hv_ClassWeights = new HTuple();
                hv_ClassWeights[hv_ValidClasses] = 1.0 / ((hv_ClassFreq.TupleSelect(hv_ValidClasses)) + 0.0001);
                //
                //Scale the weights to obtain a final output of 1.0 for the most frequent class.
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_ClassWeights = hv_ClassWeights / (((hv_ClassWeights.TupleSelect(
                            hv_ValidClasses))).TupleMin());
                        hv_ClassWeights.Dispose();
                        hv_ClassWeights = ExpTmpLocalVar_ClassWeights;
                    }
                }
                //Clip the weights.
                hv_IndicesToClip.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_IndicesToClip = ((hv_ClassWeights.TupleGreaterElem(
                        hv_MaxWeight))).TupleFind(1);
                }
                if ((int)(new HTuple(hv_IndicesToClip.TupleNotEqual(-1))) != 0)
                {
                    if (hv_ClassWeights == null)
                        hv_ClassWeights = new HTuple();
                    hv_ClassWeights[hv_IndicesToClip] = hv_MaxWeight;
                }

                ho_SegmentationImage.Dispose();

                hv_KeysExists.Dispose();
                hv_DLSamples.Dispose();
                hv_SampleIndices.Dispose();
                hv_ClassIDs.Dispose();
                hv_NumClasses.Dispose();
                hv_ClassIDsToClassIdx.Dispose();
                hv_ClassAreas.Dispose();
                hv_SampleIndex.Dispose();
                hv_DLSample.Dispose();
                hv_ImageType.Dispose();
                hv_AbsoluteHisto.Dispose();
                hv_TotalArea.Dispose();
                hv_ValidClasses.Dispose();
                hv_ClassFreq.Dispose();
                hv_IndicesToClip.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_SegmentationImage.Dispose();

                hv_KeysExists.Dispose();
                hv_DLSamples.Dispose();
                hv_SampleIndices.Dispose();
                hv_ClassIDs.Dispose();
                hv_NumClasses.Dispose();
                hv_ClassIDsToClassIdx.Dispose();
                hv_ClassAreas.Dispose();
                hv_SampleIndex.Dispose();
                hv_DLSample.Dispose();
                hv_ImageType.Dispose();
                hv_AbsoluteHisto.Dispose();
                hv_TotalArea.Dispose();
                hv_ValidClasses.Dispose();
                hv_ClassFreq.Dispose();
                hv_IndicesToClip.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region gen_dl_segmentation_weight_images
        // Chapter: Deep Learning / Semantic Segmentation
        // Short Description: Generates weight images for the training dataset. 
        public void gen_dl_segmentation_weight_images(HTuple hv_DLDataset, HTuple hv_DLPreprocessParam,
            HTuple hv_ClassWeights, HTuple hv_GenParam)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_SegmentationImage = null, ho_WeightImage = null;
            HObject ho_IgnoreRegion = null, ho_IgnoreRegionTmp = null, ho_ClassRegion = null;

            // Local control variables 

            HTuple hv_KeyExists = new HTuple(), hv_ClassIDs = new HTuple();
            HTuple hv_OverwriteFiles = new HTuple(), hv_GenParamKeys = new HTuple();
            HTuple hv_GenParamIndex = new HTuple(), hv_IgnoreClassIDs = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_DLSamples = new HTuple();
            HTuple hv_SampleIndices = new HTuple(), hv_InitNewImage = new HTuple();
            HTuple hv_SampleIndex = new HTuple(), hv_DLSample = new HTuple();
            HTuple hv_WeightImageExists = new HTuple(), hv_SampleImageID = new HTuple();
            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_IgnoreIndex = new HTuple(), hv_IgnoreClassID = new HTuple();
            HTuple hv_ClassIndex = new HTuple(), hv_ClassID = new HTuple();
            HTuple hv_Weight = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SegmentationImage);
            HOperatorSet.GenEmptyObj(out ho_WeightImage);
            HOperatorSet.GenEmptyObj(out ho_IgnoreRegion);
            HOperatorSet.GenEmptyObj(out ho_IgnoreRegionTmp);
            HOperatorSet.GenEmptyObj(out ho_ClassRegion);
            try
            {
                //
                //This procedure generates for each training sample in DLDataset a weight image,
                //that is used as input to the loss in a segmentation model.
                //The dictionary DLDataset needs a key 'dlsample_dir', assigning a directory
                //in which for every sample a dictionary DLSample has to exist.
                //The procedure reads for each training sample the dictionary DLSample,
                //generates a weight image according to the specified ClassWeights
                //and overwrites the DLSample with the updated sample including the weight image.
                //
                //Check input data.
                hv_KeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLDataset, "key_exists", ((new HTuple("dlsample_dir")).TupleConcat(
                    "samples")).TupleConcat("class_ids"), out hv_KeyExists);
                if ((int)(((hv_KeyExists.TupleSelect(0))).TupleNot()) != 0)
                {
                    throw new HalconException("DLDataset needs a key-value pair for 'dlsample_dir'");
                }
                if ((int)(((hv_KeyExists.TupleSelect(1))).TupleNot()) != 0)
                {
                    throw new HalconException("DLDataset needs a key-value pair for 'samples'");
                }
                if ((int)(((hv_KeyExists.TupleSelect(2))).TupleNot()) != 0)
                {
                    throw new HalconException("DLDataset needs a key-value pair for 'class_ids'");
                }
                //
                hv_ClassIDs.Dispose();
                HOperatorSet.GetDictTuple(hv_DLDataset, "class_ids", out hv_ClassIDs);
                if ((int)(new HTuple(hv_ClassWeights.TupleEqual(new HTuple()))) != 0)
                {
                    throw new HalconException("ClassWeights is empty");
                }
                else if ((int)((new HTuple((new HTuple(hv_ClassWeights.TupleLength()
                    )).TupleNotEqual(new HTuple(hv_ClassIDs.TupleLength())))).TupleAnd(new HTuple((new HTuple(hv_ClassWeights.TupleLength()
                    )).TupleNotEqual(1)))) != 0)
                {
                    throw new HalconException("ClassWeights must be either a single value or of the same length as the DLDataset ClassIDs.");
                }
                //
                if ((int)(new HTuple(((((hv_ClassWeights.TupleLessElem(0))).TupleFind(1))).TupleGreater(
                    -1))) != 0)
                {
                    throw new HalconException("ClassWeights must be greater or equal zero.");
                }
                else if ((int)((new HTuple((new HTuple(hv_ClassWeights.TupleLength()
                    )).TupleEqual(1))).TupleAnd(new HTuple(hv_ClassWeights.TupleLessEqual(0)))) != 0)
                {
                    throw new HalconException(new HTuple("If only a single weight is given as ClassWeights, this must be greater than zero."));
                }
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Set defaults.
                hv_OverwriteFiles.Dispose();
                hv_OverwriteFiles = 0;
                //
                //Overwrite defaults specified in GenParam.
                if ((int)(new HTuple(hv_GenParam.TupleNotEqual(new HTuple()))) != 0)
                {
                    hv_GenParamKeys.Dispose();
                    HOperatorSet.GetDictParam(hv_GenParam, "keys", new HTuple(), out hv_GenParamKeys);
                    for (hv_GenParamIndex = 0; (int)hv_GenParamIndex <= (int)((new HTuple(hv_GenParamKeys.TupleLength()
                        )) - 1); hv_GenParamIndex = (int)hv_GenParamIndex + 1)
                    {
                        if ((int)(new HTuple(((hv_GenParamKeys.TupleSelect(hv_GenParamIndex))).TupleEqual(
                            "overwrite_files"))) != 0)
                        {
                            //Set parameter for overwriting files.
                            hv_OverwriteFiles.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, "overwrite_files", out hv_OverwriteFiles);
                            if ((int)((new HTuple(hv_OverwriteFiles.TupleNotEqual(0))).TupleAnd(new HTuple(hv_OverwriteFiles.TupleNotEqual(
                                1)))) != 0)
                            {
                                throw new HalconException("'overwrite_files' must be either true or false");
                            }
                        }
                        else
                        {
                            throw new HalconException(("Unknown parameter: '" + (hv_GenParamKeys.TupleSelect(
                                hv_GenParamIndex))) + "'");
                        }
                    }
                }
                //
                //Get the IDs of the classes to be ignored.
                try
                {
                    hv_IgnoreClassIDs.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", out hv_IgnoreClassIDs);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_IgnoreClassIDs.Dispose();
                    hv_IgnoreClassIDs = new HTuple();
                }
                //
                //Get the samples from the dataset.
                hv_DLSamples.Dispose();
                HOperatorSet.GetDictTuple(hv_DLDataset, "samples", out hv_DLSamples);
                //
                //Get the indices of the samples belonging to the defined split.
                hv_SampleIndices.Dispose();
                find_dl_samples(hv_DLSamples, "split", "train", "or", out hv_SampleIndices);
                //
                //Get system info on init_new_image.
                hv_InitNewImage.Dispose();
                HOperatorSet.GetSystem("init_new_image", out hv_InitNewImage);
                //
                //Loop over training samples.
                for (hv_SampleIndex = 0; (int)hv_SampleIndex <= (int)((new HTuple(hv_SampleIndices.TupleLength()
                    )) - 1); hv_SampleIndex = (int)hv_SampleIndex + 1)
                {
                    //
                    //Read the DLSample.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DLSample.Dispose();
                        read_dl_samples(hv_DLDataset, hv_SampleIndices.TupleSelect(hv_SampleIndex),
                            out hv_DLSample);
                    }
                    //
                    //Check if there is already a weight image in the DLSample.
                    hv_WeightImageExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "weight_image", out hv_WeightImageExists);
                    if ((int)(hv_WeightImageExists.TupleAnd(hv_OverwriteFiles.TupleNot())) != 0)
                    {
                        hv_SampleImageID.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_SampleImageID);
                        throw new HalconException(("The DLSample with image_id " + hv_SampleImageID) + " already contains a weight image. Force overwriting using the parameter 'overwrite_files' to true.");
                    }
                    //
                    //Get the segmentation image.
                    ho_SegmentationImage.Dispose();
                    HOperatorSet.GetDictObject(out ho_SegmentationImage, hv_DLSample, "segmentation_image");
                    //
                    //Generate the weight image.
                    //
                    //Initialize the weight image with 0.
                    hv_ImageWidth.Dispose(); hv_ImageHeight.Dispose();
                    HOperatorSet.GetImageSize(ho_SegmentationImage, out hv_ImageWidth, out hv_ImageHeight);
                    ho_WeightImage.Dispose();
                    HOperatorSet.GenImageConst(out ho_WeightImage, "real", hv_ImageWidth, hv_ImageHeight);
                    //Clear image.
                    if ((int)(new HTuple(hv_InitNewImage.TupleEqual("false"))) != 0)
                    {
                        HOperatorSet.OverpaintRegion(ho_WeightImage, ho_WeightImage, 0, "fill");
                    }
                    //
                    if ((int)(new HTuple((new HTuple(hv_ClassWeights.TupleLength())).TupleEqual(
                        1))) != 0)
                    {
                        //Constant class weight.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.OverpaintRegion(ho_WeightImage, ho_WeightImage, hv_ClassWeights.TupleSelect(
                                0), "fill");
                        }
                        //
                        if ((int)(new HTuple((new HTuple(hv_IgnoreClassIDs.TupleLength())).TupleGreater(
                            0))) != 0)
                        {
                            //Set ignore region to 0.
                            ho_IgnoreRegion.Dispose();
                            HOperatorSet.GenEmptyRegion(out ho_IgnoreRegion);
                            for (hv_IgnoreIndex = 0; (int)hv_IgnoreIndex <= (int)((new HTuple(hv_IgnoreClassIDs.TupleLength()
                                )) - 1); hv_IgnoreIndex = (int)hv_IgnoreIndex + 1)
                            {
                                hv_IgnoreClassID.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_IgnoreClassID = hv_IgnoreClassIDs.TupleSelect(
                                        hv_IgnoreIndex);
                                }
                                ho_IgnoreRegionTmp.Dispose();
                                HOperatorSet.Threshold(ho_SegmentationImage, out ho_IgnoreRegionTmp,
                                    hv_IgnoreClassID, hv_IgnoreClassID);
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.Union2(ho_IgnoreRegion, ho_IgnoreRegionTmp, out ExpTmpOutVar_0
                                        );
                                    ho_IgnoreRegion.Dispose();
                                    ho_IgnoreRegion = ExpTmpOutVar_0;
                                }
                            }
                            HOperatorSet.OverpaintRegion(ho_WeightImage, ho_IgnoreRegion, 0.0, "fill");
                        }
                    }
                    else
                    {
                        //Loop over model ClassIDs.
                        for (hv_ClassIndex = 0; (int)hv_ClassIndex <= (int)((new HTuple(hv_ClassIDs.TupleLength()
                            )) - 1); hv_ClassIndex = (int)hv_ClassIndex + 1)
                        {
                            if ((int)((new HTuple(hv_IgnoreClassIDs.TupleEqual(new HTuple()))).TupleOr(
                                new HTuple(((hv_IgnoreClassIDs.TupleFind(hv_ClassIDs.TupleSelect(
                                hv_ClassIndex)))).TupleEqual(-1)))) != 0)
                            {
                                //Set the pixel values of the weight image according to ClassWeights.
                                hv_ClassID.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_ClassID = hv_ClassIDs.TupleSelect(
                                        hv_ClassIndex);
                                }
                                hv_Weight.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Weight = hv_ClassWeights.TupleSelect(
                                        hv_ClassIndex);
                                }
                                ho_ClassRegion.Dispose();
                                HOperatorSet.Threshold(ho_SegmentationImage, out ho_ClassRegion, hv_ClassID,
                                    hv_ClassID);
                                HOperatorSet.OverpaintRegion(ho_WeightImage, ho_ClassRegion, hv_Weight,
                                    "fill");
                            }
                            else
                            {
                                //Ignore class has weight 0 which is already set.
                            }
                        }
                    }
                    //
                    //Add the weight image to DLSample.
                    HOperatorSet.SetDictObject(ho_WeightImage, hv_DLSample, "weight_image");
                    //
                    //Write the updated DLSample.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        write_dl_samples(hv_DLDataset, hv_SampleIndices.TupleSelect(hv_SampleIndex),
                            hv_DLSample, new HTuple(), new HTuple());
                    }
                }
                //
                //On success we store the class weights for later reference in the DLDataset.
                HOperatorSet.SetDictTuple(hv_DLDataset, "class_weights", hv_ClassWeights);
                //
                ho_SegmentationImage.Dispose();
                ho_WeightImage.Dispose();
                ho_IgnoreRegion.Dispose();
                ho_IgnoreRegionTmp.Dispose();
                ho_ClassRegion.Dispose();

                hv_KeyExists.Dispose();
                hv_ClassIDs.Dispose();
                hv_OverwriteFiles.Dispose();
                hv_GenParamKeys.Dispose();
                hv_GenParamIndex.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_Exception.Dispose();
                hv_DLSamples.Dispose();
                hv_SampleIndices.Dispose();
                hv_InitNewImage.Dispose();
                hv_SampleIndex.Dispose();
                hv_DLSample.Dispose();
                hv_WeightImageExists.Dispose();
                hv_SampleImageID.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_IgnoreIndex.Dispose();
                hv_IgnoreClassID.Dispose();
                hv_ClassIndex.Dispose();
                hv_ClassID.Dispose();
                hv_Weight.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_SegmentationImage.Dispose();
                ho_WeightImage.Dispose();
                ho_IgnoreRegion.Dispose();
                ho_IgnoreRegionTmp.Dispose();
                ho_ClassRegion.Dispose();

                hv_KeyExists.Dispose();
                hv_ClassIDs.Dispose();
                hv_OverwriteFiles.Dispose();
                hv_GenParamKeys.Dispose();
                hv_GenParamIndex.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_Exception.Dispose();
                hv_DLSamples.Dispose();
                hv_SampleIndices.Dispose();
                hv_InitNewImage.Dispose();
                hv_SampleIndex.Dispose();
                hv_DLSample.Dispose();
                hv_WeightImageExists.Dispose();
                hv_SampleImageID.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_IgnoreIndex.Dispose();
                hv_IgnoreClassID.Dispose();
                hv_ClassIndex.Dispose();
                hv_ClassID.Dispose();
                hv_Weight.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region preprocess_dl_model_images
        // Chapter: Deep Learning / Model
        // Short Description: Preprocess images for deep-learning-based training and inference. 
        public void preprocess_dl_model_images(HObject ho_Images, out HObject ho_ImagesPreprocessed, HTuple hv_DLPreprocessParam)
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
        #endregion

        #region preprocess_dl_model_anomaly
        // Chapter: Deep Learning / Model
        // Short Description: Preprocess anomaly images for evaluation and visualization of the deep-learning-based anomaly detection. 
        public void preprocess_dl_model_anomaly(HObject ho_AnomalyImages, out HObject ho_AnomalyImagesPreprocessed, HTuple hv_DLPreprocessParam)
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
        #endregion

        #region preprocess_dl_model_bbox_rect1
        // Chapter: Deep Learning / Object Detection
        // Short Description: This procedure preprocesses the bounding boxes of type 'rectangle1' for a given sample. 
        public void preprocess_dl_model_bbox_rect1(HObject ho_ImageRaw, HTuple hv_DLSample, HTuple hv_DLPreprocessParam)
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
        #endregion

        #region preprocess_dl_model_bbox_rect2
        // Chapter: Deep Learning / Object Detection
        // Short Description: This procedure preprocesses the bounding boxes of type 'rectangle2' for a given sample. 
        public void preprocess_dl_model_bbox_rect2(HObject ho_ImageRaw, HTuple hv_DLSample, HTuple hv_DLPreprocessParam)
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
        #endregion

        #region preprocess_dl_model_segmentations
        // Chapter: Deep Learning / Semantic Segmentation
        // Short Description: Preprocess segmentation and weight images for deep-learning-based segmentation training and inference. 
        public void preprocess_dl_model_segmentations(HObject ho_ImagesRaw, HObject ho_Segmentations, out HObject ho_SegmentationsPreprocessed, HTuple hv_DLPreprocessParam)
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
        #endregion

        #region find_dl_samples
        // Chapter: Deep Learning / Model
        // Short Description: Retrieve the indices of Samples that contain KeyName matching KeyValue according to the Mode set. 
        public void find_dl_samples(HTuple hv_Samples, HTuple hv_KeyName, HTuple hv_KeyValue, HTuple hv_Mode, out HTuple hv_SampleIndices)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_NumKeyValues = new HTuple(), hv_SampleIndex = new HTuple();
            HTuple hv_Sample = new HTuple(), hv_KeyExists = new HTuple();
            HTuple hv_Tuple = new HTuple(), hv_Hit = new HTuple();
            HTuple hv_ValueIndex = new HTuple(), hv_Value = new HTuple();
            // Initialize local and output iconic variables 
            hv_SampleIndices = new HTuple();
            try
            {
                //
                //This procedure gets the indices of the samples that contain the
                //requested KeyName matching the requested KeyValue according to the Mode.
                //If there is no match, an empty tuple [] will be returned.
                //
                //Check input parameters.
                if ((int)(new HTuple((new HTuple(hv_KeyName.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    throw new HalconException(new HTuple("Invalid KeyName size: ") + (new HTuple(hv_KeyName.TupleLength()
                        )));
                }
                if ((int)(new HTuple((new HTuple(hv_Mode.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    throw new HalconException(new HTuple("Invalid Mode size: ") + (new HTuple(hv_Mode.TupleLength()
                        )));
                }
                if ((int)((new HTuple((new HTuple(hv_Mode.TupleNotEqual("match"))).TupleAnd(
                    new HTuple(hv_Mode.TupleNotEqual("or"))))).TupleAnd(new HTuple(hv_Mode.TupleNotEqual(
                    "contain")))) != 0)
                {
                    throw new HalconException("Invalid Mode value: " + hv_Mode);
                }
                hv_NumKeyValues.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_NumKeyValues = new HTuple(hv_KeyValue.TupleLength()
                        );
                }
                if ((int)((new HTuple(hv_Mode.TupleEqual("contain"))).TupleAnd(new HTuple(hv_NumKeyValues.TupleLess(
                    1)))) != 0)
                {
                    throw new HalconException("Invalid KeyValue size for contain Mode: " + hv_NumKeyValues);
                }
                //
                //Find the indices.
                hv_SampleIndices.Dispose();
                hv_SampleIndices = new HTuple();
                for (hv_SampleIndex = 0; (int)hv_SampleIndex <= (int)((new HTuple(hv_Samples.TupleLength()
                    )) - 1); hv_SampleIndex = (int)hv_SampleIndex + 1)
                {
                    hv_Sample.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Sample = hv_Samples.TupleSelect(
                            hv_SampleIndex);
                    }
                    hv_KeyExists.Dispose();
                    HOperatorSet.GetDictParam(hv_Sample, "key_exists", hv_KeyName, out hv_KeyExists);
                    if ((int)(hv_KeyExists) != 0)
                    {
                        hv_Tuple.Dispose();
                        HOperatorSet.GetDictTuple(hv_Sample, hv_KeyName, out hv_Tuple);
                        if ((int)(new HTuple(hv_Mode.TupleEqual("match"))) != 0)
                        {
                            //Mode 'match': Tuple must be equal KeyValue.
                            hv_Hit.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Hit = new HTuple(hv_Tuple.TupleEqual(
                                    hv_KeyValue));
                            }
                        }
                        else if ((int)((new HTuple(hv_Mode.TupleEqual("or"))).TupleAnd(
                            new HTuple((new HTuple(hv_Tuple.TupleLength())).TupleEqual(1)))) != 0)
                        {
                            //Mode 'or': Tuple must have only 1 element and it has to be equal to any of KeyValues elements.
                            hv_Hit.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Hit = new HTuple(((hv_KeyValue.TupleFindFirst(
                                    hv_Tuple))).TupleGreaterEqual(0));
                            }
                        }
                        else if ((int)(new HTuple(hv_Mode.TupleEqual("contain"))) != 0)
                        {
                            //Mode 'contain': Tuple must contain any of the elements in KeyValue.
                            HTuple end_val35 = hv_NumKeyValues - 1;
                            HTuple step_val35 = 1;
                            for (hv_ValueIndex = 0; hv_ValueIndex.Continue(end_val35, step_val35); hv_ValueIndex = hv_ValueIndex.TupleAdd(step_val35))
                            {
                                hv_Value.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Value = hv_KeyValue.TupleSelect(
                                        hv_ValueIndex);
                                }
                                hv_Hit.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Hit = new HTuple(((hv_Tuple.TupleFindFirst(
                                        hv_Value))).TupleGreaterEqual(0));
                                }
                                if ((int)(hv_Hit) != 0)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //Unsupported configuration.
                            hv_Hit.Dispose();
                            hv_Hit = 0;
                        }
                        if ((int)(hv_Hit) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_SampleIndices = hv_SampleIndices.TupleConcat(
                                        hv_SampleIndex);
                                    hv_SampleIndices.Dispose();
                                    hv_SampleIndices = ExpTmpLocalVar_SampleIndices;
                                }
                            }
                        }
                    }
                }

                hv_NumKeyValues.Dispose();
                hv_SampleIndex.Dispose();
                hv_Sample.Dispose();
                hv_KeyExists.Dispose();
                hv_Tuple.Dispose();
                hv_Hit.Dispose();
                hv_ValueIndex.Dispose();
                hv_Value.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_NumKeyValues.Dispose();
                hv_SampleIndex.Dispose();
                hv_Sample.Dispose();
                hv_KeyExists.Dispose();
                hv_Tuple.Dispose();
                hv_Hit.Dispose();
                hv_ValueIndex.Dispose();
                hv_Value.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region read_dl_samples
        // Chapter: Deep Learning / Model
        // Short Description: Read the dictionaries DLSamples from files. 
        public void read_dl_samples(HTuple hv_DLDataset, HTuple hv_SampleIndices, out HTuple hv_DLSampleBatch)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_DatasetSamples = new HTuple(), hv_MinIndex = new HTuple();
            HTuple hv_MaxIndex = new HTuple(), hv_KeyDirExists = new HTuple();
            HTuple hv_DictDir = new HTuple(), hv_DLSamplesProc = new HTuple();
            HTuple hv_ImageIndex = new HTuple(), hv_KeyFileExists = new HTuple();
            HTuple hv_ImageID = new HTuple(), hv_FileNameRelative = new HTuple();
            HTuple hv_FileNameSample = new HTuple(), hv_FileExists = new HTuple();
            HTuple hv_DictPath = new HTuple(), hv_DLSample = new HTuple();
            HTuple hv_Exception = new HTuple();
            // Initialize local and output iconic variables 
            hv_DLSampleBatch = new HTuple();
            try
            {
                //
                //This procedure reads a batch of DLSample dictionaries from disk.
                //The wanted samples are selected from a DLDataset by their indices.
                //The indices of the wanted samples are handed over in SampleIndices.
                //It returns the tuple of read-in dictionaries in DLSampleBatch.
                //
                //Sanity checks of inputs.
                //
                if ((int)(new HTuple((new HTuple(hv_SampleIndices.TupleLength())).TupleLessEqual(
                    0))) != 0)
                {
                    //Check the length of selected indices.
                    throw new HalconException(new HTuple("Invalid length of SelectedIndices: ") + (new HTuple(hv_SampleIndices.TupleLength()
                        )));
                }
                else
                {
                    //Get the samples from the DLDataset.
                    hv_DatasetSamples.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLDataset, "samples", out hv_DatasetSamples);
                    //Get min and max value of given indices.
                    hv_MinIndex.Dispose();
                    HOperatorSet.TupleMin(hv_SampleIndices, out hv_MinIndex);
                    hv_MaxIndex.Dispose();
                    HOperatorSet.TupleMax(hv_SampleIndices, out hv_MaxIndex);
                    if ((int)((new HTuple(hv_MinIndex.TupleLess(0))).TupleOr(new HTuple(hv_MaxIndex.TupleGreater(
                        (new HTuple(hv_DatasetSamples.TupleLength())) - 1)))) != 0)
                    {
                        //Check the value range of the provided indices.
                        throw new HalconException("The given SampleIndices are not within the range of available samples in DLDataset.");
                    }
                }
                //
                //Check if the key dlsample_dir is given.
                hv_KeyDirExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLDataset, "key_exists", "dlsample_dir", out hv_KeyDirExists);
                //
                if ((int)(hv_KeyDirExists) != 0)
                {
                    //
                    //Get the dlsample_dir.
                    hv_DictDir.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLDataset, "dlsample_dir", out hv_DictDir);
                    //Get the samples to be processed.
                    hv_DLSamplesProc.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DLSamplesProc = hv_DatasetSamples.TupleSelect(
                            hv_SampleIndices);
                    }
                    //
                    //Initialize DLSampleBatch tuple.
                    hv_DLSampleBatch.Dispose();
                    hv_DLSampleBatch = new HTuple();
                    //
                    //Read in all DLSamples into the batch.
                    for (hv_ImageIndex = 0; (int)hv_ImageIndex <= (int)((new HTuple(hv_SampleIndices.TupleLength()
                        )) - 1); hv_ImageIndex = (int)hv_ImageIndex + 1)
                    {
                        //Check if dlsample key exist.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_KeyFileExists.Dispose();
                            HOperatorSet.GetDictParam(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                "key_exists", "dlsample_file_name", out hv_KeyFileExists);
                        }
                        //
                        if ((int)(hv_KeyFileExists.TupleNot()) != 0)
                        {
                            //
                            //If the key does not exist, check if a corresponding file exists.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ImageID.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                    "image_id", out hv_ImageID);
                            }
                            hv_FileNameRelative.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_FileNameRelative = hv_ImageID + "_dlsample.hdict";
                            }
                            hv_FileNameSample.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_FileNameSample = (hv_DictDir + "/") + hv_FileNameRelative;
                            }
                            //
                            hv_FileExists.Dispose();
                            HOperatorSet.FileExists(hv_FileNameSample, out hv_FileExists);
                            if ((int)(hv_FileExists) != 0)
                            {
                                //If it exists, create corresponding key.
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.SetDictTuple(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                        "dlsample_file_name", hv_FileNameRelative);
                                }
                            }
                            else
                            {
                                //If not, throw an error.
                                throw new HalconException("No 'dlsample_file_name' and hdict file available for image ID " + hv_ImageID);
                            }
                            //
                        }
                        //
                        //If dlsample dictionary is available for reading, read it.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_DictPath.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLSamplesProc.TupleSelect(hv_ImageIndex),
                                "dlsample_file_name", out hv_DictPath);
                        }
                        try
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_DLSample.Dispose();
                                HOperatorSet.ReadDict((hv_DictDir + "/") + hv_DictPath, new HTuple(), new HTuple(),
                                    out hv_DLSample);
                            }
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException1)
                        {
                            HDevExpDefaultException1.ToHTuple(out hv_Exception);
                            throw new HalconException((((("An error has occurred while reading " + hv_DictDir) + "/") + hv_DictPath) + new HTuple(" , HALCON error # ")) + (hv_Exception.TupleSelect(
                                0)));
                        }
                        //Add it to the DLSampleBatch.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_DLSampleBatch = hv_DLSampleBatch.TupleConcat(
                                    hv_DLSample);
                                hv_DLSampleBatch.Dispose();
                                hv_DLSampleBatch = ExpTmpLocalVar_DLSampleBatch;
                            }
                        }
                        //
                    }
                }
                else
                {
                    throw new HalconException("The dataset needs to include the key 'dlsample_dir' for reading a DLSample from file.");
                }


                hv_DatasetSamples.Dispose();
                hv_MinIndex.Dispose();
                hv_MaxIndex.Dispose();
                hv_KeyDirExists.Dispose();
                hv_DictDir.Dispose();
                hv_DLSamplesProc.Dispose();
                hv_ImageIndex.Dispose();
                hv_KeyFileExists.Dispose();
                hv_ImageID.Dispose();
                hv_FileNameRelative.Dispose();
                hv_FileNameSample.Dispose();
                hv_FileExists.Dispose();
                hv_DictPath.Dispose();
                hv_DLSample.Dispose();
                hv_Exception.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_DatasetSamples.Dispose();
                hv_MinIndex.Dispose();
                hv_MaxIndex.Dispose();
                hv_KeyDirExists.Dispose();
                hv_DictDir.Dispose();
                hv_DLSamplesProc.Dispose();
                hv_ImageIndex.Dispose();
                hv_KeyFileExists.Dispose();
                hv_ImageID.Dispose();
                hv_FileNameRelative.Dispose();
                hv_FileNameSample.Dispose();
                hv_FileExists.Dispose();
                hv_DictPath.Dispose();
                hv_DLSample.Dispose();
                hv_Exception.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region reassign_pixel_values
        // Chapter: Image / Manipulation
        // Short Description: Changes a value of ValuesToChange in Image to NewValue. 
        public void reassign_pixel_values(HObject ho_Image, out HObject ho_ImageOut, HTuple hv_ValuesToChange, HTuple hv_NewValue)
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
        #endregion

        #region convert_rect2_5to8param
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
        #endregion

        #region convert_rect2_8to5param
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
        #endregion

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
