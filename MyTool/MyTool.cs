using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace MyTool
{
    public class FolderAndFileManageClass
    {
        /// <summary>
        /// 扫描指定文件夹获取此文件夹中指定格式的名称列表
        /// </summary>
        /// <param name="FolderPath">文件夹路径</param>
        /// <param name="Format">指定文件的格式，多个以逗号隔开(.txt,.jpg)</param>
        /// <param name="FileQuantity">此种文件的个数</param>
        /// <returns>返回文件名</returns>
        public static string[] ScanFolderAndGetAssignedFormatFileName(string FolderPath, string Format, out int FileQuantity)
        {
            List<string> result = new List<string>();
            string[] strDataFiles = Directory.GetFiles(FolderPath);
            FileQuantity = 0;

            for (int i = 0; i < strDataFiles.Length; i++)
            {
                string format = System.IO.Path.GetExtension(strDataFiles[i]);
                foreach (string s in Format.Split(','))
                {
                    if (format == s)
                    {
                        result.Add(System.IO.Path.GetFileNameWithoutExtension(strDataFiles[i]));
                        FileQuantity++;
                        break;
                    }
                }

            }

            return result.ToArray();
        }

        /// <summary>
        /// 扫描指定文件夹获取此文件夹中指定格式的名称列表
        /// </summary>
        /// <param name="FolderPath">文件夹路径</param>
        /// <param name="Format">指定文件的格式，多个以逗号隔开(.txt,.jpg)</param>
        /// <returns>返回文件名</returns>
        public static string[] ScanFolderAndGetAssignedFormatFileName(string FolderPath, string Format)
        {
            List<string> result = new List<string>();
            string[] strDataFiles = Directory.GetFiles(FolderPath);

            for (int i = 0; i < strDataFiles.Length; i++)
            {
                string format = System.IO.Path.GetExtension(strDataFiles[i]);
                foreach (string s in Format.Split(','))
                {
                    if (format == s)
                    {
                        result.Add(System.IO.Path.GetFileNameWithoutExtension(strDataFiles[i]));
                        break;
                    }
                }

            }

            return result.ToArray();
        }

        /// <summary>
        /// 扫描文件夹中所有的指定格式的文件，并将其以修改时间的先后顺序放置进数组中。顺序由新到旧
        /// </summary>
        /// <param name="FolderPath">指定搜寻的文件夹路径</param>
        /// <param name="Format">指定的文件格式</param>
        /// <returns>返回排列好的数组</returns>
        public static string[] ScanFolderAndReturnTheAllFileNameBaseLastWriteTime(string FolderPath,string Format)
        {
            //List<string> result = new List<string>();
           // string[] strDataFiles = Directory.GetFiles(FolderPath);
            //var files = new DirectoryInfo(FolderPath).GetFiles(Format).OrderBy(FileInfo => FileInfo.LastWriteTime).ToArray();//升序，从最老到最新
            var files = new DirectoryInfo(FolderPath).GetFiles(Format).OrderByDescending(FileInfo => FileInfo.LastWriteTime).ToArray();//降序，从最新到最老
          /*  for (int i = 0; i < strDataFiles.Length; i++)
            {
                string format = System.IO.Path.GetExtension(strDataFiles[i]);
                foreach (string s in Format.Split(','))
                {
                    if (format == s)
                    {
                        result.Add(strDataFiles[i]);
                        break;
                    }
                }
            }

            //直接插入法排序
            string tempPath; 
            for (int i = 1; i < result.Count; i++)
            {
                int j = i - 1;
                tempPath = result[i];
                FileInfo fi1 = new FileInfo(result[i]);
                FileInfo fi2 = new FileInfo(result[j]);
                while(j>=0 && fi1.LastWriteTime > fi2.LastWriteTime)
                {
                    result[j + 1] = result[j];
                    j--;
                    if (j >= 0)
                    {
                        fi2 = new FileInfo(result[j]);
                    }
                }
                result[j + 1] = tempPath;
            }*/

            return files.Select(f=>f.FullName).ToArray();//result.ToArray();
        }

        /// <summary>
        /// 扫描文件夹中所有的指定格式的文件，返回最近修改的一个文件路径
        /// </summary>
        /// <param name="FolderPath">指定搜寻的文件夹路径</param>
        /// <param name="Format">指定的文件格式</param>
        /// <returns>返回最近修改的文件路径</returns>
        public static string ScanFolderAndReturnTheLastFilePathBaseLastWriteTime(string FolderPath, string Format)
        {
            string[] strDataFiles = Directory.GetFiles(FolderPath);
            string lastFilePath = string.Empty;
            FileInfo fi1;
            FileInfo fi2;
            for (int i = 1; i < strDataFiles.Length; i++)
            {
                string format = System.IO.Path.GetExtension(strDataFiles[i]);
                
                foreach (string s in Format.Split(','))
                {
                    if (format == s)
                    {
                        if (lastFilePath == string.Empty)
                        {
                            lastFilePath = strDataFiles[i];
                        }
                        fi1 = new FileInfo(lastFilePath);
                        fi2 = new FileInfo(strDataFiles[i]);
                        if (fi1.LastWriteTime < fi2.LastWriteTime)
                        {
                            lastFilePath = strDataFiles[i];
                        }
                        break;
                    }
                }
            }

            return lastFilePath;
        }

        #region RenameFile：重命名指定文件
        /// <summary>
        /// RenameFile：重命名指定文件
        /// </summary>
        /// <param name="OldFileFullPath">string:原文件所在路径</param>
        /// <param name="NewFileFullPath">string:新文件所在路径</param>
        /// <returns>bool: 返回结果</returns>
        public static bool RenameFile(string OldFileFullPath, string NewFileFullPath)
        {
            try
            {
                FileInfo fI = new FileInfo(OldFileFullPath);
                fI.MoveTo(NewFileFullPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region FilterPathIllegalChar:过滤掉路径中的非法字符
        /// <summary>
        /// FilterPathIllegalChar:过滤掉路径中的非法字符
        /// </summary>
        /// <param name="SourcePath">string:初始路径</param>
        /// <returns>string:过滤处理之后的路径</returns>
        public static string FilterPathIllegalChar(string SourcePath)
        {
            StringBuilder pathBuilder = new StringBuilder(SourcePath);
            foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
            {
                pathBuilder = pathBuilder.Replace(rInvalidChar.ToString(), string.Empty);
            }
            return pathBuilder.ToString();
        }
        #endregion

        #region GetFolderPathInAssignedFolder:获取指定文件夹中的所有文件夹路径
        /// <summary>
        /// GetFolderPathInAssignedFolder:获取指定文件夹中的所有文件夹路径
        /// </summary>
        /// <param name="assignedFolder">string:指定文件夹路径</param>
        /// <returns>List<string>:所有文件夹路径</returns>
        public static List<string> GetFolderPathInAssignedFolder(string assignedFolder)
        {
            List<string> myList = new List<string>();
            //绑定路径到指定的文件夹目录
            DirectoryInfo dir = new DirectoryInfo(assignedFolder);
            //检索表示当前目录的子目录
            FileSystemInfo[] fsinfos = dir.GetDirectories();
            //遍历检索的文件和子目录
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                myList.Add(fsinfo.FullName);
            }
            return myList;
        }
        #endregion

        #region GetAllFile2PathInAssignedFolder:获取指定文件夹中的所有文件路径，包含文件夹
        /// <summary>
        /// GetAllFilesPathInAssignedFolder:获取指定文件夹中的所有文件路径，包含文件夹
        /// </summary>
        /// <param name="assignedFolder">string:指定文件夹路径</param>
        /// <returns>List<string>:所有文件路径</returns>
        public static List<string> GetAllFile2PathInAssignedFolder(string assignedFolder)
        {
            List<string> myList = new List<string>();
            //绑定路径到指定的文件夹目录
            DirectoryInfo dir = new DirectoryInfo(assignedFolder);
            //检索表示当前目录的文件和子目录
            FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();
            //遍历检索的文件和子目录
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                myList.Add(fsinfo.FullName);
            }
            return myList;
        }
        #endregion

        #region GetFilesPathInAssignedFolder:获取指定文件夹中的所有文件路径，不包含文件夹
        /// <summary>
        /// GetFilesPathInAssignedFolder:获取指定文件夹中的所有文件路径，不包含文件夹
        /// </summary>
        /// <param name="assignedFolder">string:指定文件夹路径</param>
        /// <returns>List<string>:所有文件路径，不包含文件夹</returns>
        public static List<string> GetFilesPathInAssignedFolder(string assignedFolder)
        {
            List<string> myList = new List<string>();
            //绑定路径到指定的文件夹目录
            DirectoryInfo dir = new DirectoryInfo(assignedFolder);
            //检索表示当前目录的文件
            FileSystemInfo[] fsinfos = dir.GetFiles();
            //遍历检索的文件和子目录
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                myList.Add(fsinfo.FullName);
            }
            return myList;
        }
        #endregion

    }

    public class DataProcessing
    {
        #region CalculatingDataMean:计算给定数据数组的平均
        /// <summary>
        /// 计算给定数据数组的指定数目的数据的平均值
        /// </summary>
        /// <param name="SourceData">给定数据数组</param>
        /// <param name="DataQuantity">要计算的数据个数</param>
        /// <returns>返回数据平均值</returns>
        public static float CalculatingDataMean(float[] SourceData, int DataQuantity)
        {
            float mean = 0 ;
            if (DataQuantity > SourceData.Length)
            {
                DataQuantity = SourceData.Length;
            }

            if (DataQuantity <= 0)
            {
                return 0;
            }

            for (int i = 0; i < DataQuantity; i++)
            {
                mean += SourceData[i];
            }

            mean = mean / DataQuantity;

           return mean;
        }
        #endregion

        #region CalculatingDataVarience:计算给定数据数组的方差值
        /// <summary>
        /// 计算给定数据数组的指定数目的数据的方差值
        /// </summary>
        /// <param name="SourceData">给定数据数组</param>
        /// <param name="DataQuantity">要计算的数据个数</param>
        /// <returns>返回数据方差</returns>
        public static float CalculatingDataVarience(float[] SourceData, int DataQuantity)
        {
            float variance = 0;
            float mean;
            if (DataQuantity > SourceData.Length)
            {
                DataQuantity = SourceData.Length;
            }
            if (DataQuantity <= 0)
            {
                return 0;
            }

            mean = CalculatingDataMean(SourceData, DataQuantity);

            for (int i = 0; i < DataQuantity; i++)
            {
                variance += (SourceData[i] - mean) * (SourceData[i] - mean);
            }

            variance = variance / DataQuantity;

            return (float)Math.Sqrt((double)variance);
        }
        #endregion

        #region TextBoxDataCheck:Textbox中输入浮点数数据检测，判定是否符合设定需求，以及是否符合上下限
        /// <summary>
        /// TextBoxDataCheck:Textbox中输入浮点数数据检测，判定是否符合设定需求，以及是否符合上下限
        /// </summary>
        /// <param name="TextBoxStr">string:TextBox中输入的string</param>
        /// <param name="MinLimit">float:下限值</param>
        /// <param name="MaxLimit">float:上限值</param>
        /// <param name="UsingMinLimitCheckFlag">bool:启用下限值检测标签，true-进行下限值检测，false-不进行下限值检测</param>
        /// <param name="UsingMaxLimitCheckFlag">bool:启用上限值检测标签，true-进行上限值检测，false-不进行上限值检测</param>
        /// <param name="FloatDataFlag">bool:数据类型为浮点数类型标签，true-数据类型为浮点数，false-数据类型为整型</param>
        /// <param name="LastCheckFlag">bool:最后一次检测标志，在实际过程中，由于小数点和负号的存在，有的时候要返回符号不正确的值，但是在最终的时候，例如-是不合法，这种情况要用Try catch进行检测</param>
        /// <returns>string:返回修改之后的数据</returns>
        public static string TextBoxDataCheck(string TextBoxStr, float MinLimit, float MaxLimit, bool UsingMinLimitCheckFlag, bool UsingMaxLimitCheckFlag, bool FloatDataFlag, bool LastCheckFlag)
        {
            float textBoxFloatData = 0;//用于转换出来的textbox中的数据，当数据类型为浮点数的时候
            int minusSymbolQuantity = CheckAssignedStringQuantityInOneString(TextBoxStr, "-");//获取负号的个数
            int dotSymbolQuantity = CheckAssignedStringQuantityInOneString(TextBoxStr, ".");//获取点号的个数

            //判定负号符号相关
            if (minusSymbolQuantity > 1)//如果负号个数大于1个，直接清空返回
            {
                TextBoxStr = TextBoxStr.Substring(0, TextBoxStr.IndexOf("-") + 1) +
                    TextBoxStr.Substring(TextBoxStr.IndexOf("-") + 1, TextBoxStr.Length - TextBoxStr.IndexOf("-") - 1).Substring(0, TextBoxStr.Substring(TextBoxStr.IndexOf("-") + 1, TextBoxStr.Length - TextBoxStr.IndexOf("-") - 1).IndexOf("-"));
            }
            else if (minusSymbolQuantity == 1 && TextBoxStr.IndexOf("-") != 0)//如果负号数只有一个，但是不处于头部
            {
                TextBoxStr = "-" + TextBoxStr.Substring(0, TextBoxStr.IndexOf("-")) + TextBoxStr.Substring(TextBoxStr.IndexOf("-") + 1, TextBoxStr.Length - TextBoxStr.IndexOf("-") - 1);

                if (Convert.ToSingle(TextBoxStr) < MinLimit && UsingMinLimitCheckFlag)
                {
                    float tempInvertData = Convert.ToSingle(TextBoxStr.Substring(1, TextBoxStr.Length - 1));
                    if ((tempInvertData <= MaxLimit && UsingMaxLimitCheckFlag && tempInvertData >= MinLimit && UsingMinLimitCheckFlag)
                        || (tempInvertData <= MaxLimit && UsingMaxLimitCheckFlag && !UsingMinLimitCheckFlag)
                         || (tempInvertData >= MinLimit && UsingMinLimitCheckFlag && !UsingMaxLimitCheckFlag))
                    {
                        TextBoxStr = TextBoxStr.Substring(1, TextBoxStr.Length - 1);
                    }
                }
            }
            else if (minusSymbolQuantity == 1 && TextBoxStr.IndexOf("-") == 0 && TextBoxStr.Length == 1)//如果负号数只有一个，并且string中只有这一个项目
            {
                if (!LastCheckFlag)//不为最后一次检测
                    return TextBoxStr;
            }

            //判定小数点号相关
            if (dotSymbolQuantity > 1)//如果小数点号个数大于1个，直接清空返回
            {
                TextBoxStr = TextBoxStr.Substring(0, TextBoxStr.IndexOf(".") + 1) +
                    TextBoxStr.Substring(TextBoxStr.IndexOf(".") + 1, TextBoxStr.Length - TextBoxStr.IndexOf(".") - 1).Substring(0, TextBoxStr.Substring(TextBoxStr.IndexOf(".") + 1, TextBoxStr.Length - TextBoxStr.IndexOf(".") - 1).IndexOf("."));
            }
            else if (dotSymbolQuantity == 1 && TextBoxStr.IndexOf(".") == TextBoxStr.Length - 1)//如果只有一个小数点，并且这个小数点处于末尾位置
            {
                if (!LastCheckFlag)
                    return TextBoxStr;
            }


            try
            {
                textBoxFloatData = Convert.ToSingle(TextBoxStr);
            }
            catch
            {
                return "0";
            }

            bool dataSmallerThanMinOrBiggerThanMax = false;
            if (UsingMinLimitCheckFlag && textBoxFloatData < MinLimit)
            {
                textBoxFloatData = MinLimit;
                dataSmallerThanMinOrBiggerThanMax = true;
            }

            if (UsingMaxLimitCheckFlag && textBoxFloatData > MaxLimit)
            {
                textBoxFloatData = MaxLimit;
                dataSmallerThanMinOrBiggerThanMax = true;
            }

            if (FloatDataFlag)
            {
                if (LastCheckFlag || dataSmallerThanMinOrBiggerThanMax)
                    return textBoxFloatData.ToString();
                else
                    return TextBoxStr;
            }
            else
                return Convert.ToInt32(textBoxFloatData).ToString();
        }
        #endregion

        #region TextBoxDataCheck2:Textbox中输入浮点数数据检测，判定是否符合设定需求，以及是否符合上下限
        /// <summary>
        /// TextBoxDataCheck:Textbox中输入浮点数数据检测，判定是否符合设定需求，以及是否符合上下限
        /// </summary>
        /// <param name="TextBoxStr">string:TextBox中输入的string</param>
        /// <param name="MinLimit">float:下限值</param>
        /// <param name="MaxLimit">float:上限值</param>
        /// <param name="UsingMinLimitCheckFlag">bool:启用下限值检测标签，true-进行下限值检测，false-不进行下限值检测</param>
        /// <param name="UsingMaxLimitCheckFlag">bool:启用上限值检测标签，true-进行上限值检测，false-不进行上限值检测</param>
        /// <param name="FloatDataFlag">bool:数据类型为浮点数类型标签，true-数据类型为浮点数，false-数据类型为整型</param>
        /// <param name="LastCheckFlag">bool:最后一次检测标志，在实际过程中，由于小数点和负号的存在，有的时候要返回符号不正确的值，但是在最终的时候，例如-是不合法，这种情况要用Try catch进行检测</param>
        /// <returns>bool:数据OK返回ture</returns>
        public static bool TextBoxDataCheck2(string TextBoxStr, float MinLimit, float MaxLimit, bool UsingMinLimitCheckFlag, bool UsingMaxLimitCheckFlag, bool FloatDataFlag, bool LastCheckFlag)
        {
            float textBoxFloatData = 0;//用于转换出来的textbox中的数据，当数据类型为浮点数的时候
            int minusSymbolQuantity = CheckAssignedStringQuantityInOneString(TextBoxStr, "-");//获取负号的个数
            int dotSymbolQuantity = CheckAssignedStringQuantityInOneString(TextBoxStr, ".");//获取点号的个数

            //判定负号符号相关
            if (minusSymbolQuantity > 1)//如果负号个数大于1个，直接清空返回
            {
                TextBoxStr = TextBoxStr.Substring(0, TextBoxStr.IndexOf("-") + 1) +
                    TextBoxStr.Substring(TextBoxStr.IndexOf("-") + 1, TextBoxStr.Length - TextBoxStr.IndexOf("-") - 1).Substring(0, TextBoxStr.Substring(TextBoxStr.IndexOf("-") + 1, TextBoxStr.Length - TextBoxStr.IndexOf("-") - 1).IndexOf("-"));
            }
            else if (minusSymbolQuantity == 1 && TextBoxStr.IndexOf("-") != 0)//如果负号数只有一个，但是不处于头部
            {
                TextBoxStr = "-" + TextBoxStr.Substring(0, TextBoxStr.IndexOf("-")) + TextBoxStr.Substring(TextBoxStr.IndexOf("-") + 1, TextBoxStr.Length - TextBoxStr.IndexOf("-") - 1);

                if (Convert.ToSingle(TextBoxStr) < MinLimit && UsingMinLimitCheckFlag)
                {
                    float tempInvertData = Convert.ToSingle(TextBoxStr.Substring(1, TextBoxStr.Length - 1));
                    if ((tempInvertData <= MaxLimit && UsingMaxLimitCheckFlag && tempInvertData >= MinLimit && UsingMinLimitCheckFlag)
                        || (tempInvertData <= MaxLimit && UsingMaxLimitCheckFlag && !UsingMinLimitCheckFlag)
                         || (tempInvertData >= MinLimit && UsingMinLimitCheckFlag && !UsingMaxLimitCheckFlag))
                    {
                        TextBoxStr = TextBoxStr.Substring(1, TextBoxStr.Length - 1);
                    }
                }
            }
            else if (minusSymbolQuantity == 1 && TextBoxStr.IndexOf("-") == 0 && TextBoxStr.Length == 1)//如果负号数只有一个，并且string中只有这一个项目
            {
                if (!LastCheckFlag)//不为最后一次检测
                    //return TextBoxStr;
                    return true;
            }

            //判定小数点号相关
            if (dotSymbolQuantity > 1)//如果小数点号个数大于1个，直接清空返回
            {
                TextBoxStr = TextBoxStr.Substring(0, TextBoxStr.IndexOf(".") + 1) +
                    TextBoxStr.Substring(TextBoxStr.IndexOf(".") + 1, TextBoxStr.Length - TextBoxStr.IndexOf(".") - 1).Substring(0, TextBoxStr.Substring(TextBoxStr.IndexOf(".") + 1, TextBoxStr.Length - TextBoxStr.IndexOf(".") - 1).IndexOf("."));
            }
            else if (dotSymbolQuantity == 1 && TextBoxStr.IndexOf(".") == TextBoxStr.Length - 1)//如果只有一个小数点，并且这个小数点处于末尾位置
            {
                if (!LastCheckFlag)
                    //return TextBoxStr;
                    return true;
            }


            try
            {
                textBoxFloatData = Convert.ToSingle(TextBoxStr);
            }
            catch
            {
                //return "0";
                return false;
            }

            bool dataSmallerThanMinOrBiggerThanMax = false;
            if (UsingMinLimitCheckFlag && textBoxFloatData < MinLimit)
            {
                //textBoxFloatData = MinLimit;
                dataSmallerThanMinOrBiggerThanMax = true;
                return false;
            }
            else
            {
                dataSmallerThanMinOrBiggerThanMax = false;
            }


            if (UsingMaxLimitCheckFlag && textBoxFloatData > MaxLimit)
            {
                //textBoxFloatData = MaxLimit;
                dataSmallerThanMinOrBiggerThanMax = true;
                return false;
            }
            else
            {
                dataSmallerThanMinOrBiggerThanMax = false;
            }

            if (FloatDataFlag)
            {
                if (LastCheckFlag && !dataSmallerThanMinOrBiggerThanMax)
                    //return textBoxFloatData.ToString();
                    return true;
                else
                    //return TextBoxStr;
                    return false;
            }
            else
                //return Convert.ToInt32(textBoxFloatData).ToString();
                return true;
        }
        #endregion

        #region TextBoxIPFormatCheck:当需要向TextBox中输入IP地址的时候，检测输入的数据是否OK
        /// <summary>
        /// TextBoxIPFormatCheck:当需要向TextBox中输入IP地址的时候，检测输入的数据是否OK
        /// </summary>
        /// <param name="TextBoxStr">原始数据</param>
        /// <param name="LastCheckFlag">是否是最后一次检测</param>
        public static string TextBoxIPFormatCheck(string TextBoxStr, bool LastCheckFlag)
        {
            string[] tempStr = new string[4] { "", "", "", "" };
            string tempTextStr = TextBoxStr;
            int count = 0;

            while (tempTextStr.IndexOf(".") != -1)
            {
                if (count <= 2 && count >= 0)
                {
                    tempStr[count] = tempTextStr.Substring(0, tempTextStr.IndexOf("."));
                    tempTextStr = tempTextStr.Substring(tempTextStr.IndexOf(".") + 1, tempTextStr.Length - tempTextStr.IndexOf(".") - 1);
                    count++;
                }
            }

            if (count == 3)//如果前三个都放置入textstr中
            {
                tempStr[count] = tempTextStr;
            }
            else if (count < 3)
            {
                tempStr[count] = tempTextStr;
            }

            if (LastCheckFlag && count < 3)
            {
                for (int i = count; i < 4; i++)
                {
                    if(tempStr[i] == "")
                        tempStr[i] = "0";
                }
                count = 3;
            }

            int data = 0;
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    data = Convert.ToInt32(tempStr[i]);
                }
                catch
                {
                    if (tempStr[i] != "")
                        tempStr[i] = "";

                }
                if (data > 255 && tempStr[i] != "")
                {
                    tempStr[i] = "255";
                }
                else if (data < 0 && tempStr[i] != "")
                {
                    tempStr[i] = "0";
                }
                else if (tempStr[i] == "" && LastCheckFlag)
                {
                    tempStr[i] = "0";
                }
                else if(LastCheckFlag)
                {
                    tempStr[i] = Convert.ToInt32(tempStr[i]).ToString();
                }
            }

            tempTextStr = "";

            for (int i = 0; i < count; i++)
            {
                tempTextStr += tempStr[i] + ".";
            }
            tempTextStr += tempStr[count];
            return tempTextStr;

        }

        #endregion

        #region CheckAssignedStringQuantityInOneString:在指定字符串中查找字符串出现的次数

        /// <summary>
        /// CheckAssignedStringQuantityInOneString:在指定字符串中查找字符串出现的次数
       /// </summary>
       /// <param name="BeCheckedStr">string:被检查的字符串</param>
       /// <param name="CheckStr">string:所要查找的字符串</param>
       /// <returns>int:被检查字符串中所要查找的字符串的个数</returns>
        public static int CheckAssignedStringQuantityInOneString(string BeCheckedStr, string CheckStr)
        {
            int quantity = 0;
            int beCheckedStrLength = BeCheckedStr.Length;
            int checkStrLength = CheckStr.Length;
            string tempStr;
            for (int i = 0; i <= beCheckedStrLength - checkStrLength; i++)
            {
                tempStr = BeCheckedStr.Substring(i, checkStrLength);
                if (tempStr == CheckStr)
                {
                    quantity++;
                }
            }

            return quantity;
        }

        #endregion


        #region ChangeTwoShortToFloat：将两个16位short合并成为一个32位flt数据
        /// <summary>
        /// ChangeTwoShortToFloat：将两个16位short合并成为一个32位flt数据
        /// </summary>
        /// <param name="SourceLowerData">short:需要转换的16位short数据的低位数据</param>
        /// <param name="SourceUpperData">short:需要转换的16位short数据的高位数据</param>
        /// <returns>float：返回合并之后的float数据</returns>
        public static float ChangeTwoShortToFloat(short SourceLowerData, short SourceUpperData)
        {
            var lowerByte = BitConverter.GetBytes(SourceLowerData);
            var upperByte = BitConverter.GetBytes(SourceUpperData);

            byte[] fltByte = new byte[4];
            fltByte[0] = lowerByte[0];
            fltByte[1] = lowerByte[1];
            fltByte[2] = upperByte[0];
            fltByte[3] = upperByte[1];

            return System.BitConverter.ToSingle(fltByte, 0);
        }
        #endregion

        #region ChangeFloatToTwoShort：将一个32位float数据拆分成为两个16位short数据
        /// <summary>
        /// ChangeFloatToTwoShort：将一个32位float数据拆分成为两个16位short数据
        /// </summary>
        /// <param name="SourceFlt">short:需要被转换的浮点数</param>
        /// <param name="ChangedToLowerShortData">short:转换之后的低位short数据</param>
        /// <param name="ChangedToUpperShortData">short:转换之后的高位short数据</param>
        public static void ChangeFloatToTwoShort(float SourceFlt, out short ChangedToLowerShortData, out short ChangedToUpperShortData)
        {
            short[] changedToShortData = new short[2];
            var sourceFltByte = BitConverter.GetBytes(SourceFlt);

            byte[] shortByte = new byte[2];
            for (int i = 0; i < changedToShortData.Length; i++)
            {
                shortByte[0] = sourceFltByte[i*2+0];
                shortByte[1] = sourceFltByte[i * 2 + 1];
                changedToShortData[i] = System.BitConverter.ToInt16(shortByte, 0);
            }
            ChangedToLowerShortData = changedToShortData[0];
            ChangedToUpperShortData = changedToShortData[1];
        }
        #endregion


        #region SetDataToOneVariateBaseVariateStringName:基于变量字符串名称对变量进行赋值
        /// <summary>
        /// SetDataToOneVariateBaseVariateStringName:基于变量字符串名称对变量进行赋值
        /// </summary>
        /// <param name="VariateName">string:变量字符串</param>
        /// <param name="VariateFatherForm">Form:变量所在的窗口变量</param>
        /// <param name="SetData">object:需要赋予的值</param>
        public static void SetDataToOneVariateBaseVariateStringName(string VariateName, Form VariateFatherForm, object SetData)
        {
            VariateFatherForm.GetType().GetField(VariateName).SetValue(VariateFatherForm, SetData);
        }
        #endregion

        #region SetDataToOneVariateBaseVariateStringName:基于变量字符串名称对变量进行赋值
        /// <summary>
        /// SetDataToOneVariateBaseVariateStringName:基于变量字符串名称对变量进行赋值
        /// </summary>
        /// <param name="VariateName">string:变量字符串</param>
        /// <param name="DataFormat">string:数据格式</param>
        /// <param name="VariateFatherForm">Form:变量所在的窗口变量</param>
        /// <param name="SetData">object:需要赋予的值</param>
        public static void SetDataToOneVariateBaseVariateStringName(string VariateName, string DataFormat, Form VariateFatherForm, string SetData)
        {
            object setData = null;
            switch (DataFormat)
            {
                case "int":
                    try
                    {
                        setData = Convert.ToInt32(SetData);
                    }
                    catch
                    {
                        setData = Convert.ToInt32("0");
                    }
                    break;
                case "float":
                    try
                    {
                        setData = Convert.ToSingle(SetData);
                    }
                    catch
                    {
                        setData = Convert.ToSingle("0");
                    }
                    break;
                case "string":
                    setData = Convert.ToString(SetData);
                    break;
            }
            VariateFatherForm.GetType().GetField(VariateName).SetValue(VariateFatherForm, setData);
        }
        #endregion

        #region SetDataToOneVariateBaseVariateStringName:基于变量字符串名称对变量进行赋值
        /// <summary>
        /// SetDataToOneVariateBaseVariateStringName:基于变量字符串名称对变量进行赋值
        /// </summary>
        /// <param name="VariateName">string:变量字符串</param>
        /// <param name="DataFormat">string:数据格式</param>
        /// <param name="VariateFatherForm">object:变量所在的类变量</param>
        /// <param name="SetData">object:需要赋予的值</param>
        public static void SetDataToOneVariateBaseVariateStringName(string VariateName, string DataFormat, object VariateObject, string SetData)
        {
            object setData = null;
            switch (DataFormat)
            {
                case "int":
                    try
                    {
                        setData = Convert.ToInt32(SetData);
                    }
                    catch
                    {
                        setData = Convert.ToInt32("0");
                    }
                    break;
                case "float":
                    try
                    {
                        setData = Convert.ToSingle(SetData);
                    }
                    catch
                    {
                        setData = Convert.ToSingle("0");
                    }
                    break;
                case "string":
                    setData = Convert.ToString(SetData);
                    break;
            }
            VariateObject.GetType().GetField(VariateName).SetValue(VariateObject, setData);
        }
        #endregion

        #region GetDataOfOneVariateBaseVariateStringName:基于变量字符串名称获取变量值
        /// <summary>
        /// GetDataOfOneVariateBaseVariateStringName:基于变量字符串名称获取变量值
        /// </summary>
        /// <param name="VariateName">string:变量字符串</param>
        /// <param name="VariateFatherForm">Form:变量所在的窗口变量</param>
        public static object GetDataOfOneVariateBaseVariateStringName(string VariateName, Form VariateFatherForm)
        {
           return VariateFatherForm.GetType().GetField(VariateName).GetValue(VariateFatherForm);
        }
        #endregion

        #region GetMovingAverageBaseSourceData:基于初始数据Datatable获取此数据的滑动平均数
        /// <summary>
        /// GetMovingAverageBaseSourceData:基于初始数据Datatable获取此数据的滑动平均数
        /// </summary>
        /// <param name="sourceDt">DataTable:初始数据集，Column1代表横坐标值，Column2代表纵坐标值</param>
        /// <param name="period">int:移动平均法的期数</param>
        /// <returns>DataTable:基于初始数据获取到的滑动平均数数据集，Column1代表横坐标值，Column2代表纵坐标值</returns>
        public static DataTable GetMovingAverageBaseSourceData(DataTable sourceDt, int period)
        { 
            DataTable movingAverageDt = null;
            movingAverageDt = new DataTable();
            if (period <= 0)//如果期数小于等于0
            {
                return sourceDt;
            }

            if(sourceDt == null)//如果初始数据集为空
            {
                return sourceDt;
            }

            if (sourceDt.Rows.Count < period)//如果数据集的数据个数少于期数，那么返回此数据集
            {
                return sourceDt;
            }
            else
            {
                for(int i=0;i<sourceDt.Columns.Count;i++)
                {
                    movingAverageDt.Columns.Add("Columns"+i,typeof(string));
                }
                DataRow dr = sourceDt.NewRow();
                float sum = 0;
                
                for (int i = period - 1; i < sourceDt.Rows.Count; i++)
                {
                    sum = 0;
                    for (int j = i - period + 1; j <= i; j++)
                    {
                        sum += Convert.ToSingle(sourceDt.Rows[j][1]);
                    }
                    dr[0] = sourceDt.Rows[i][0];
                    dr[1] = Convert.ToString(sum / period);
                    movingAverageDt.Rows.Add(dr.ItemArray);
                }

                return movingAverageDt;
            }
        }
        #endregion

        #region GetMovingAverageBaseSourceData:基于初始数据Datatable获取此数据的滑动平均数
        /// <summary>
        /// GetMovingAverageBaseSourceData:基于初始数据Datatable获取此数据的滑动平均数
        /// </summary>
        /// <param name="sourceDt">DataTable:初始数据集，Column1代表横坐标值，Column2代表纵坐标值</param>
        /// <param name="period">int:移动平均法的期数</param>
        /// <returns>DataTable:基于初始数据获取到的滑动平均数数据集，Column1代表横坐标值，Column2代表纵坐标值，其余Column原封不动获取原始数据</returns>
        public static DataTable GetMovingAverageBaseSourceData2(DataTable sourceDt, int period)
        {
            DataTable movingAverageDt = null;
            movingAverageDt = new DataTable();
            if (period <= 0)//如果期数小于等于0
            {
                return sourceDt;
            }

            if (sourceDt == null)//如果初始数据集为空
            {
                return sourceDt;
            }

            if (sourceDt.Rows.Count < period)//如果数据集的数据个数少于期数，那么返回此数据集
            {
                return sourceDt;
            }
            else
            {
                for (int i = 0; i < sourceDt.Columns.Count; i++)
                {
                    movingAverageDt.Columns.Add("Columns" + i, typeof(string));
                }
                DataRow dr = sourceDt.NewRow();
                float sum = 0;

                for (int i = period - 1; i < sourceDt.Rows.Count; i++)
                {
                    sum = 0;
                    for (int j = i - period + 1; j <= i; j++)
                    {
                        sum += Convert.ToSingle(sourceDt.Rows[j][1]);
                    }
                    dr[0] = sourceDt.Rows[i][0];
                    dr[1] = Convert.ToString(sum / period);
                    for (int j = 2; j < sourceDt.Columns.Count; j++)
                    {
                        dr[j] = sourceDt.Rows[i][j];
                    }
                    movingAverageDt.Rows.Add(dr.ItemArray);
                }

                return movingAverageDt;
            }
        }
        #endregion

        #region GetMovingAverageBaseSourceDataNegative:基于初始数据Datatable反向获取此数据的滑动平均数
        /// <summary>
        /// GetMovingAverageBaseSourceDataNegative:基于初始数据Datatable反向获取此数据的滑动平均数
        /// </summary>
        /// <param name="sourceDt">DataTable:初始数据集，Column1代表横坐标值，Column2代表纵坐标值</param>
        /// <param name="period">int:移动平均法的期数</param>
        /// <returns>DataTable:基于初始数据获取到的滑动平均数数据集，Column1代表横坐标值，Column2代表纵坐标值</returns>
        public static DataTable GetMovingAverageBaseSourceDataNegative(DataTable sourceDt, int period)
        {
            DataTable movingAverageDt = null;
            movingAverageDt = new DataTable();
            DataTable newSourceDt = new DataTable();
            if (period <= 0)//如果期数小于等于0
            {
                return sourceDt;
            }

            if (sourceDt == null)//如果初始数据集为空
            {
                return sourceDt;
            }

            if (sourceDt.Rows.Count < period)//如果数据集的数据个数少于期数，那么返回此数据集
            {
                return sourceDt;
            }
            else
            {
                for (int i = 0; i < sourceDt.Columns.Count; i++)
                {
                    movingAverageDt.Columns.Add("Columns" + i, typeof(string));
                    newSourceDt.Columns.Add("Columns" + i, typeof(string));
                }
                DataRow dr = sourceDt.NewRow();
                float sum = 0;
                for (int i = sourceDt.Rows.Count - 1; i > 0; i--)
                {
                    dr[0] = sourceDt.Rows[i][0];
                    dr[1] = sourceDt.Rows[i][1];
                    newSourceDt.Rows.Add(dr.ItemArray);
                }

                for (int i = period - 1; i < newSourceDt.Rows.Count; i++)
                {
                    sum = 0;
                    for (int j = i - period + 1; j <= i; j++)
                    {
                        sum += Convert.ToSingle(newSourceDt.Rows[j][1]);
                    }
                    dr[0] = newSourceDt.Rows[i][0];
                    dr[1] = Convert.ToString(sum / period);
                    movingAverageDt.Rows.Add(dr.ItemArray);
                }

                newSourceDt.Rows.Clear();
                dr = movingAverageDt.NewRow();
                for (int i = 0; i < movingAverageDt.Rows.Count; i++)
                {
                    dr[0] = movingAverageDt.Rows[movingAverageDt.Rows.Count - i - 1][0];
                    dr[1] = movingAverageDt.Rows[movingAverageDt.Rows.Count - i - 1][1];
                    newSourceDt.Rows.Add(dr.ItemArray);
                }

                return newSourceDt;
            }
        }
        #endregion

        #region GetMovingAverageBaseSourceDataNegative2:基于初始数据Datatable反向获取此数据的滑动平均数
        /// <summary>
        /// GetMovingAverageBaseSourceDataNegative2:基于初始数据Datatable反向获取此数据的滑动平均数
        /// </summary>
        /// <param name="sourceDt">DataTable:初始数据集，Column1代表横坐标值，Column2代表纵坐标值</param>
        /// <param name="period">int:移动平均法的期数</param>
        /// <returns>DataTable:基于初始数据获取到的滑动平均数数据集，Column1代表横坐标值，Column2代表纵坐标值，其余Column原封不动获取原始数据</returns>
        public static DataTable GetMovingAverageBaseSourceDataNegative2(DataTable sourceDt, int period)
        {
            DataTable movingAverageDt = null;
            movingAverageDt = new DataTable();
            DataTable newSourceDt = new DataTable();
            if (period <= 0)//如果期数小于等于0
            {
                return sourceDt;
            }

            if (sourceDt == null)//如果初始数据集为空
            {
                return sourceDt;
            }

            if (sourceDt.Rows.Count < period)//如果数据集的数据个数少于期数，那么返回此数据集
            {
                return sourceDt;
            }
            else
            {
                for (int i = 0; i < sourceDt.Columns.Count; i++)
                {
                    movingAverageDt.Columns.Add("Columns" + i, typeof(string));
                    newSourceDt.Columns.Add("Columns" + i, typeof(string));
                }
                DataRow dr = sourceDt.NewRow();
                float sum = 0;
                
                for (int i = sourceDt.Rows.Count - 1; i > 0; i--)
                {
                    dr[0] = sourceDt.Rows[i][0];
                    dr[1] = sourceDt.Rows[i][1];
                    for (int j = 2; j < sourceDt.Columns.Count; j++)
                    {
                        dr[j] = sourceDt.Rows[i][j];
                    }
                    newSourceDt.Rows.Add(dr.ItemArray);
                }

                for (int i = period - 1; i < newSourceDt.Rows.Count; i++)
                {
                    sum = 0;
                    for (int j = i - period + 1; j <= i; j++)
                    {
                        sum += Convert.ToSingle(newSourceDt.Rows[j][1]);
                    }
                    dr[0] = newSourceDt.Rows[i][0];
                    dr[1] = Convert.ToString(sum / period);
                    for (int j = 2; j < sourceDt.Columns.Count; j++)
                    {
                        dr[j] = newSourceDt.Rows[i][j];
                    }
                    movingAverageDt.Rows.Add(dr.ItemArray);
                }

                newSourceDt.Rows.Clear();
                dr = movingAverageDt.NewRow();
                for (int i = 0; i < movingAverageDt.Rows.Count; i++)
                {
                    dr[0] = movingAverageDt.Rows[movingAverageDt.Rows.Count - i - 1][0];
                    dr[1] = movingAverageDt.Rows[movingAverageDt.Rows.Count - i - 1][1];
                    for (int j = 2; j < movingAverageDt.Columns.Count; j++)
                    {
                        dr[j] = movingAverageDt.Rows[movingAverageDt.Rows.Count - i - 1][j];
                    }
                    newSourceDt.Rows.Add(dr.ItemArray);
                }

                return newSourceDt;
            }
        }
        #endregion
    }

    public class PictureProcessing
    {
        /// <summary>
        /// ResizeBitmapImage:修改Bitmap图片尺寸
        /// </summary>
        /// <param name="oldbmp">Bitmap:旧图像</param>
        /// <param name="newWidth">int:修改后的宽度</param>
        /// <param name="newHeight">int:修改后的高度</param>
        /// <returns>Bitmap:修改尺寸后的图像</returns>
        public static Bitmap ResizeBitmapImage(Bitmap Oldbmp, int NewWidth, int NewHeight)
        {
            try
            {
                Bitmap b = new Bitmap(NewWidth, NewHeight);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
               // g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(Oldbmp, new Rectangle(0, 0, NewWidth, NewHeight), new Rectangle(0, 0, Oldbmp.Width, Oldbmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ResizeImage：修改image图像的尺寸
        /// </summary>
        /// <param name="SourceImage">Image:原image</param>
        /// <param name="NewWidth">int:新的Image的宽度</param>
        /// <param name="NewHeight">int:新的Image的高度</param>
        /// <returns>Image:返回修改过后的image</returns>
        public static Image ResizeImage(Image SourceImage, int NewWidth, int NewHeight)
        {
            Image newImage;
            Bitmap bmp = new Bitmap(NewWidth, NewHeight);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(SourceImage, 0, 0, bmp.Width, bmp.Height);
            newImage= bmp;
            return newImage;
        }
    }

    public class PictureBoxControl
    {
        public static void DrawWordsInTheCenterOfPictureBoxSuitSize(PictureBox SourcePicBox, Font UsingFont, Color BackgroundClr, string DrawText, Color DrawTextFontClr)
        {
            Bitmap bmp = new Bitmap(SourcePicBox.Width, SourcePicBox.Height);
            Graphics g = Graphics.FromImage(bmp);

            Brush backBrush = new SolidBrush(BackgroundClr);
            float fontSize = -0.99f;
            g.FillRectangle(backBrush, new Rectangle(0, 0, SourcePicBox.Width, SourcePicBox.Height));
            SizeF sizeOfFont;

            do
            {
                fontSize++;
                sizeOfFont = g.MeasureString(DrawText, new Font(UsingFont.FontFamily, fontSize, UsingFont.Style));
            } while (sizeOfFont.Width < SourcePicBox.Width && sizeOfFont.Height < SourcePicBox.Height);


            do
            {
                fontSize = fontSize -0.1f;
                sizeOfFont = g.MeasureString(DrawText, new Font(UsingFont.FontFamily, fontSize, UsingFont.Style));
            } while (sizeOfFont.Width > SourcePicBox.Width || sizeOfFont.Height > SourcePicBox.Height);

            Brush textBrush = new SolidBrush(DrawTextFontClr);
            Point textPoint = new Point((int)((SourcePicBox.Width-sizeOfFont.Width)/2),(int)((SourcePicBox.Height-sizeOfFont.Height)/2));
            g.DrawString(DrawText, new Font(UsingFont.FontFamily, fontSize, UsingFont.Style), textBrush, textPoint);

            SourcePicBox.Image = bmp;
        }
    }


    public class TextDisplay
    {
        /// <summary>
        /// 用于实现字符串动态显示
        /// </summary>
        /// <param name="SourceControl">需要控制的Label控件名</param>
        /// <param name="DisplayContents">需要显示的内容</param>
        /// <param name="FontFormat">需要显示的字体名称</param>
        /// <param name="StartIndex">开始的显示的string的编号索引</param>
        /// <param name="Offset">偏移量，用于保证控件不一闪一闪的</param>
        /// <param name="BackColor">需要设置的控件背景颜色</param>
        /// <param name="FontColor">需要设置的控件字体颜色</param>
        /// <returns>返回新的StartIndex</returns>
        public static int AchieveDynamicDisplayTextInLabelControl(Label SourceControl, string DisplayContents, string FontFormat, int StartIndex, int Offset, Color BackColor, Color FontColor)
        {
            Font dispF;
            string labelMsg = "";

            dispF = new Font(FontFormat, SourceControl.Font.Size, SourceControl.Font.Style);

            SizeF fontSize = new SizeF();
            SourceControl.Font = dispF;
            Graphics g = SourceControl.CreateGraphics();
            int dispMaxLength = 0;
            fontSize = g.MeasureString(DisplayContents, dispF);
            if (fontSize.Width <= SourceControl.Width)
            {
                dispMaxLength = DisplayContents.Length;
                labelMsg = DisplayContents;
            }
            else
            {
                DisplayContents += "     ";
                do
                {
                    dispMaxLength++;
                    if (DisplayContents.Length - StartIndex > dispMaxLength)
                        labelMsg = DisplayContents.Substring(StartIndex, dispMaxLength);
                    else if (DisplayContents.Length - StartIndex > 0)
                        labelMsg = DisplayContents.Substring(StartIndex, DisplayContents.Length - StartIndex) + DisplayContents.Substring(0, dispMaxLength - DisplayContents.Length + StartIndex);
                    else
                    {
                        StartIndex = 0;
                        labelMsg = DisplayContents;
                    }
                    fontSize = g.MeasureString(labelMsg, dispF);
                } while (fontSize.Width <= (SourceControl.Width - Offset) && dispMaxLength < DisplayContents.Length);//减20的目的是让在显示中文报警信息的时候不一跳一跳的

                if (fontSize.Width > (SourceControl.Width - Offset) && dispMaxLength > 0)
                    dispMaxLength = dispMaxLength - 1;

                labelMsg = DisplayContents.Substring(StartIndex, DisplayContents.Length - StartIndex);
                if (labelMsg.Length >= dispMaxLength)
                {
                    labelMsg = labelMsg.Substring(0, dispMaxLength);
                    StartIndex++;
                }
                else
                {
                    if (labelMsg.Length != 0)
                    {
                        labelMsg = labelMsg + DisplayContents.Substring(0, dispMaxLength - labelMsg.Length);
                        StartIndex++;
                    }
                    else
                    {
                        labelMsg = DisplayContents.Substring(0, dispMaxLength);
                        StartIndex = 1;
                    }
                }

            }

            SourceControl.Font = dispF;
            SourceControl.Text = labelMsg;
            SourceControl.BackColor = BackColor;
            SourceControl.ForeColor = FontColor;

            return StartIndex;
        }
    }

    public delegate void AddMessageToLOGFileFinish(string addMsg,string logFileName);//声明委托-添加Msg进入LOG文件
    public class TxtFileProcess//用以对TXT文件处理
    {
        public static event AddMessageToLOGFileFinish AddMessageToLOGFileFinishEvent;//声明事件-添加Msg进入LOG文件
        static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();//用于管理资源访问的锁定状态，可实现多线程读取或进行独占式写入访问

        #region About Log File

        #region CreateLog:创建LOG文件
        /// <summary>
        /// CreateLog:创建LOG文件
        /// </summary>
        /// <param name="AddMsg">string：需要添加的Msg内容</param>
        /// <param name="FilePath">string：需要保存的文件的路径，不包含\\，不包含文件名</param>
        /// <param name="FileName">string：需要保存的文件名称</param>
        /// <param name="AssignTimeFlag">bool:是否指定了时间，false-没有指定时间，那么自动根据当前时间添加，true-指定了时间，那么不会添加当前时间</param>
        /// <param name="SizeLimit">int: Txt文件大小上限，当大于此数据后，新建后缀名的txt文件，单位KB</param>
        /// <returns>bool:返回保存结果</returns>
        public static bool CreateLog(string AddMsg, string FilePath, string FileName, bool AssignTimeFlag, int SizeLimit)
        {
            string fileFullPath = FilePath + "\\" + FileName;
            if (FileName.LastIndexOf(".") > 0 && FileName.Length > FileName.LastIndexOf("."))
            {
                if (FileName.Substring(FileName.LastIndexOf(".") + 1, FileName.Length - FileName.LastIndexOf(".") - 1).ToLower() != "txt")
                {
                    fileFullPath = fileFullPath + ".txt";
                }
            }
            else
            {
                FileName += ".txt";
                fileFullPath = FilePath + "\\" + FileName;
            }

            //如果文件夹不存在，那么创建文件夹
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            FileInfo fileInfo = new FileInfo(fileFullPath);
            int addIndex = 1;
            while (File.Exists(fileFullPath) && fileInfo.Length > 1024 * SizeLimit)
            {
                fileFullPath = FilePath + "\\" + FileName;
                fileFullPath = fileFullPath.Substring(0, fileFullPath.LastIndexOf(".")) + addIndex.ToString() + ".txt";
                fileInfo = new FileInfo(fileFullPath);
                addIndex++;
            }

            LogWriteLock.EnterWriteLock();

            try
            {
                if (AssignTimeFlag)
                    File.AppendAllText(fileFullPath, AddMsg + "\r");
                else
                    File.AppendAllText(fileFullPath, string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now) + "->" + AddMsg + "\r");
            }
            catch
            {
                LogWriteLock.ExitWriteLock();
                return false;
            }

            LogWriteLock.ExitWriteLock();

            if (AddMessageToLOGFileFinishEvent != null)
                AddMessageToLOGFileFinishEvent(AddMsg, FilePath);

            return true;
        }


        /// <summary>
        /// CreateLog:创建LOG文件
        /// </summary>
        /// <param name="AddMsg">string：需要添加的Msg内容</param>
        /// <param name="FilePath">string：需要保存的文件的路径，不包含\\，不包含文件名</param>
        /// <param name="FileName">string：需要保存的文件名称</param>
        /// <param name="AssignTimeFlag">bool:是否指定了时间，false-没有指定时间，那么自动根据当前时间添加，true-指定了时间，那么不会添加当前时间</param>
        /// <param name="SizeLimit">int: Txt文件大小上限，当大于此数据后，新建后缀名的txt文件，单位KB</param>
        /// <param name="UsingEventFlag">bool:是否触发事件标志，false-不触发，true-触发</param>
        /// <returns>bool:返回保存结果</returns>
        public static bool CreateLog(string AddMsg, string FilePath, string FileName, bool AssignTimeFlag, int SizeLimit, bool UsingEventFlag)
        {
            string fileFullPath = FilePath + "\\" + FileName;
            if (FileName.LastIndexOf(".") > 0 && FileName.Length > FileName.LastIndexOf("."))
            {
                if (FileName.Substring(FileName.LastIndexOf(".") + 1, FileName.Length - FileName.LastIndexOf(".") - 1).ToLower() != "txt")
                {
                    fileFullPath = fileFullPath + ".txt";
                }
            }
            else
            {
                FileName += ".txt";
                fileFullPath = FilePath + "\\" + FileName;
            }

            //如果文件夹不存在，那么创建文件夹
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            FileInfo fileInfo = new FileInfo(fileFullPath);
            int addIndex = 1;
            while (File.Exists(fileFullPath) && fileInfo.Length > 1024 * SizeLimit)
            {
                fileFullPath = FilePath + "\\" + FileName;
                fileFullPath = fileFullPath.Substring(0, fileFullPath.LastIndexOf(".")) + addIndex.ToString() + ".txt";
                fileInfo = new FileInfo(fileFullPath);
                addIndex++;
            }

            LogWriteLock.EnterWriteLock();

            try
            {
                if (AssignTimeFlag)
                    File.AppendAllText(fileFullPath, AddMsg + "\r");
                else
                    File.AppendAllText(fileFullPath, string.Format("{0:dd/MM/yyyy HH:mm:ss.fff}", DateTime.Now) + "->" + AddMsg + "\r");
            }
            catch
            {
                LogWriteLock.ExitWriteLock();
                return false;
            }

            LogWriteLock.ExitWriteLock();

            if (AddMessageToLOGFileFinishEvent != null && UsingEventFlag)
                AddMessageToLOGFileFinishEvent(AddMsg, FilePath);

            return true;
        }

        #endregion


        public static DataTable LoadAlarmRecordData(string FileFullPath)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Number", typeof(string));
            dt.Columns.Add("Contents", typeof(string));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("AlarmIndex", typeof(int));
            int dtIndex = 0;
            bool dataUsefulFlag = true;//用于判定读取到的这一行的数据是否可用
            LogWriteLock.EnterReadLock();
            
            //StreamReader sr = new StreamReader(FileFullPath);
            //string temp = sr.ReadToEnd();
            //sr.Close();

            IEnumerable<string> alarmData = File.ReadLines(FileFullPath);
            
            foreach (string tempData in alarmData)
            {
                dataUsefulFlag = true;
                DataRow dr = dt.NewRow();
                dr[0] = (dtIndex + 1).ToString("D5");
                if ((tempData.IndexOf(">") + 1 > 0) && (tempData.LastIndexOf("-") - tempData.IndexOf(">") - 1) > 0)
                    dr[1] = tempData.Substring(tempData.IndexOf(">") + 1, tempData.LastIndexOf("-") - tempData.IndexOf(">") - 1);
                else
                    dataUsefulFlag = false;

                if (tempData.IndexOf("-") > 0)
                    dr[2] = tempData.Substring(0, tempData.IndexOf("-"));
                else
                    dataUsefulFlag = false;

                if ((tempData.LastIndexOf(">") + 1) > 0 && (tempData.Length - tempData.LastIndexOf(">") - 1) > 0)
                    dr[3] = tempData.Substring(tempData.LastIndexOf(">") + 1, tempData.Length - tempData.LastIndexOf(">") - 1);
                else
                    dataUsefulFlag = false;

                if (dataUsefulFlag)
                {
                    dt.Rows.Add(dr);
                    dtIndex++;
                }
            }

            LogWriteLock.ExitReadLock();

            return dt;
        }

        #endregion
    }

    /// <summary>
    /// IniFunc:用于对INI文件的读取,保存等处理类
    /// </summary>
    public class IniFunc
    {
        /// <summary>
        /// WritePrivateProfileString:写数据至ini文件
        /// </summary>
        /// <param name="section">string:节</param>
        /// <param name="key">string:键</param>
        /// <param name="value">string:值</param>
        /// <param name="filePath">string:ini文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        /// <summary>
        /// GetPrivateProfileString:从ini文件中读取数据出来
        /// </summary>
        /// <param name="section">string:节</param>
        /// <param name="key">string:键</param>
        /// <param name="defValue">string:未读取到的默认值</param>
        /// <param name="retValue">BuliderString:读取到的数据</param>
        /// <param name="size">int:大小</param>
        /// <param name="filePath">string:ini文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defValue, StringBuilder retValue, int size, string filePath);

        /// <summary>
        /// ReadIni:读取ini文件
        /// </summary>
        /// <param name="section">string:节</param>
        /// <param name="key">string:键</param>
        /// <param name="defValue">string:未读取到的默认值</param>
        /// <param name="filePath">string:ini文件路径</param>
        /// <param name="retValue">StringBuilder:读取到的数据文件</param>
        /// <returns></returns>
        public static int ReadIni(string section, string key, string defValue, string filePath,out StringBuilder retValue)
        {
            retValue = new StringBuilder(1024);
            return GetPrivateProfileString(section, key, defValue, retValue, 256, filePath);
        }

        /// <summary>
        /// WriteIni:写入ini文件
        /// </summary>
        /// <param name="section">string:节</param>
        /// <param name="key">string:键</param>
        /// <param name="value">string:值</param>
        /// <param name="filePath">string:ini文件路径</param>
        /// <returns></returns>
        public static long WriteIni(string section, string key, string value, string filePath)
        {
            return WritePrivateProfileString(section, key, value, filePath);
        }

        /// <summary>
        /// DeleteSection:删除节
        /// </summary>
        /// <param name="section">string:节</param>
        /// <param name="filePath">string:ini文件路径</param>
        /// <returns></returns>
        public static long DeleteSection(string section, string filePath)
        {
            return WritePrivateProfileString(section, null, null, filePath);
        }

        /// <summary>
        /// DeleteKey:删除键
        /// </summary>
        /// <param name="section">string:节</param>
        /// <param name="key">string:键</param>
        /// <param name="filePath">string:ini文件路径</param>
        /// <returns></returns>
        public static long DeleteKey(string section, string key, string filePath)
        {
            return WritePrivateProfileString(section, key, null, filePath);
        }
    }

    public class Other
    {
        /// <summary>
        /// YesterdayCalculate:昨日计算
        /// </summary>
        /// <param name="nowYear">int:当前年</param>
        /// <param name="nowMonth">int:当前月</param>
        /// <param name="nowDay">int:当前日</param>
        /// <param name="yesterdayYear">int: out 昨天年</param>
        /// <param name="yesterdayMonth">int:out 昨天月</param>
        /// <param name="yesterdayDay">int:out 昨天日</param>
        public static void YesterdayCalculate(int nowYear, int nowMonth, int nowDay, out int yesterdayYear, out int yesterdayMonth, out int yesterdayDay)
        {
            yesterdayYear = 0;
            yesterdayMonth = 0;
            yesterdayDay = 0;

            if (nowDay != 1)
            {
                yesterdayYear = nowYear;
                yesterdayMonth = nowMonth;
                yesterdayDay = nowDay - 1;
            }
            else if (nowDay == 1)
            {
                if (nowMonth != 1)
                {
                    yesterdayYear = nowYear;
                    yesterdayMonth = nowMonth - 1;
                }
                else if (nowMonth == 1)
                {
                    yesterdayYear = nowYear - 1;
                    yesterdayMonth = 12;
                }

                if (nowMonth == 5 || nowMonth == 7 || 
                    nowMonth == 10 || nowMonth == 12)
                {
                    yesterdayDay = 30;
                }
                else if (nowMonth == 1 ||
                    nowMonth == 2 || nowMonth == 4 ||
                    nowMonth == 6 || nowMonth == 8 ||
                    nowMonth == 9 || nowMonth == 11)
                {
                    yesterdayDay = 31;
                }
                else
                {
                    yesterdayDay = 28;
                }

            }
        }

        /// <summary>
        /// TomorrowCalculate:明日计算
        /// </summary>
        /// <param name="nowYear">int:当前年</param>
        /// <param name="nowMonth">int:当前月</param>
        /// <param name="nowDay">int:当前日</param>
        /// <param name="tomorrowYear">int: out 明日年</param>
        /// <param name="tomorrowMonth">int:out 明日月</param>
        /// <param name="tomorrowDay">int:out 明日日</param>
        public static void TomorrowCalculate(int nowYear, int nowMonth, int nowDay, out int tomorrowYear, out int tomorrowMonth, out int tomorrowDay)
        {
            tomorrowYear = 0;
            tomorrowMonth = 0;
            tomorrowDay = 0;

            tomorrowYear = nowYear;
            tomorrowMonth = nowMonth;
            tomorrowDay = nowDay - 1;

            if (nowDay == 31)
            {
                tomorrowMonth = nowMonth + 1;
                if (nowMonth != 12)
                {
                    tomorrowYear = nowYear;
                }
                else
                {
                    tomorrowYear = nowYear + 1;
                }
                tomorrowDay = 1;
            }
            else if (nowDay == 30)
            {
                if (nowMonth == 2 || nowMonth == 4 ||
                    nowMonth == 6 || nowMonth == 9 ||
                    nowMonth == 11)
                {
                    tomorrowDay = 1;
                    tomorrowMonth = nowMonth + 1;
                    tomorrowYear = nowYear;
                }
                else if (nowMonth == 1 ||
                    nowMonth == 3 || nowMonth == 5 ||
                    nowMonth == 7 || nowMonth == 8 ||
                    nowMonth == 10 || nowMonth == 12)
                {
                    tomorrowDay = nowDay + 1;
                    tomorrowMonth = nowMonth;
                    tomorrowYear = nowYear;
                }
            }
            else if ((nowDay == 28 || nowDay == 29) && nowMonth == 2)
            {
                tomorrowDay = 1;
                tomorrowMonth = nowMonth + 1;
                tomorrowYear = nowYear;
            }
            else
            {
                tomorrowDay = nowDay + 1;
                tomorrowMonth = nowMonth;
                tomorrowYear = nowYear;
            }

        }

        /// <summary>
        /// TimeAddition:时间加法
        /// </summary>
        /// <param name="sourceYear"></param>
        /// <param name="sourceMonth"></param>
        /// <param name="sourceDay"></param>
        /// <param name="sourceHour"></param>
        /// <param name="sourceMinutes"></param>
        /// <param name="addend"></param>
        /// <param name="newYear"></param>
        /// <param name="newMonth"></param>
        /// <param name="newDay"></param>
        /// <param name="newHour"></param>
        /// <param name="newMinutes"></param>
        public static void TimeAddition(int sourceYear, int sourceMonth, int sourceDay, int sourceHour, int sourceMinutes, float addend, out int newYear, out int newMonth, out int newDay, out int newHour, out int newMinutes)
        {
            newYear = 0;
            newMonth = 0;
            newDay = 0;
            newHour = 0;
            newMinutes = 0;

            float sourceTime = (float)sourceHour + ((float)sourceMinutes / 60f);

            if (sourceTime + addend >= 24)
            {
                for (int i = 0; i < (sourceTime + addend) % 24; i++)
                {
                    TomorrowCalculate(sourceYear, sourceMonth, sourceDay, out newYear, out newMonth, out newDay);
                }
                newHour = (int)(Math.Floor(((sourceTime + addend) % 24) * 10) / 10);
                newMinutes = (int)(((sourceTime + addend) % 24 - (float)newHour) * 60f);
            }
            else
            {
                newYear = sourceYear;
                newMonth = sourceMonth;
                newDay = sourceDay;
                newHour = (int)(Math.Floor((sourceTime + addend) * 10) / 10);
                newMinutes = (int)((sourceTime + addend - (float)newHour) * 60f);
            }
        }

        /// <summary>
        /// CompareTime：比较时间大小，false：时间1大，true:时间2大
        /// </summary>
        /// <param name="year1"></param>
        /// <param name="month1"></param>
        /// <param name="day1"></param>
        /// <param name="hour1"></param>
        /// <param name="minutes1"></param>
        /// <param name="year2"></param>
        /// <param name="month2"></param>
        /// <param name="day2"></param>
        /// <param name="hour2"></param>
        /// <param name="minutes2"></param>
        /// <returns>int:大小结果，-1：时间1大，0：两个时间相等，1：时间2大</returns>
        public static int CompareTime(int year1, int month1, int day1, int hour1, int minutes1, int year2, int month2, int day2, int hour2, int minutes2)
        {
            if (year1 > year2)//如果时间1的年大于时间2的年，那么肯定时间1大
            {
                return -1;
            }
            else if (year1 < year2)//如果时间2的年大于时间1的年，那么肯定时间2大
            {
                return 1;
            }
            else//如果年相等
            {
                if (month1 > month2)//如果时间1的月大于时间2的月，那么肯定时间1大
                {
                    return -1;
                }
                else if (month1 < month2)//如果时间2的月大于时间1的月，那么肯定时间2大
                {
                    return 1;
                }
                else//如果月相等
                {
                    if (day1 > day2)//如果时间1的日大于时间2的日，那么肯定时间1大
                    {
                        return -1;
                    }
                    else if (day1 < day2)//如果时间2的日大于时间1的日，那么肯定时间2大
                    {
                        return 1;
                    }
                    else//如果日相等
                    {
                        if (hour1 > hour2)//如果时间1的时大于时间2的时，那么肯定时间1大
                        {
                            return -1;
                        }
                        else if (hour1 < hour2)//如果时间2的时大于时间1的时，那么肯定时间2大
                        {
                            return 1;
                        }
                        else//如果时相等
                        {
                            if (minutes1 > minutes2)//如果时间1的分大于时间2的分，那么肯定时间1大
                            {
                                return -1;
                            }
                            else if (minutes1 < minutes2)//如果时间2的分大于时间1的时，那么肯定时间2大
                            {
                                return 1;
                            }
                            else//如果分相等
                            {
                                return 0;//两个时间相等
                            }
                        }
                    }
                }
            }

        }
    
    }
}
