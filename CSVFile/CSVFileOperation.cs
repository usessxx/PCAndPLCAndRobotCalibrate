using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace CSVFile
{
    public class CSVFileOperation
    {
        #region SaveCSV:DataTable数据保存为csv文件(列名写入csv文件)
        /// <summary>
        /// DataTable数据保存为csv文件(列名写入csv文件)
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="filePath">csv文件路径</param>
        /// <returns></returns>
        public static void SaveCSV(DataTable dt, string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(filePath, FileMode.Create,FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            string data = "";

            for (int i = 0; i < dt.Columns.Count; i++)//写入列
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            for (int i = 0; i < dt.Rows.Count; i++) //写入各行数据
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文引号 英文引号需要换成两个引号
                    if (str.IndexOf(',') >= 0 || str.IndexOf('"')>=0
                      || str.IndexOf('\r') >= 0 || str.IndexOf('\n')>=0) //含逗号 引号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }
        #endregion

        #region SaveCSV2:DataTable数据保存为csv文件(列名不写入csv文件)
        /// <summary>
        /// DataTable数据保存为csv文件(列名不写入csv文件)
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="filePath">csv文件路径</param>
        /// <returns></returns>
        public static void SaveCSV2(DataTable dt, string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            string data = "";

            //for (int i = 0; i < dt.Columns.Count; i++)//写入列
            //{
            //    data += dt.Columns[i].ColumnName.ToString();
            //    if (i < dt.Columns.Count - 1)
            //    {
            //        data += ",";
            //    }
            //}
            //sw.WriteLine(data);

            for (int i = 0; i < dt.Rows.Count; i++) //写入各行数据
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文引号 英文引号需要换成两个引号
                    if (str.IndexOf(',') >= 0 || str.IndexOf('"') >= 0
                      || str.IndexOf('\r') >= 0 || str.IndexOf('\n') >= 0) //含逗号 引号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }       
        #endregion

        #region SaveCSV3:DataTable数据保存为csv文件(列名不写入csv文件),返回保存的结果，true-保存成功，false-保存失败
        /// <summary>
        /// DataTable数据保存为csv文件(列名不写入csv文件)
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="filePath">csv文件路径</param>
        /// <returns>bool:返回成功与否标志，保存成功return true，保存失败return false</returns>
        public static bool SaveCSV3(DataTable dt, string filePath)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                string data = "";

                //for (int i = 0; i < dt.Columns.Count; i++)//写入列
                //{
                //    data += dt.Columns[i].ColumnName.ToString();
                //    if (i < dt.Columns.Count - 1)
                //    {
                //        data += ",";
                //    }
                //}
                //sw.WriteLine(data);


                for (int i = 0; i < dt.Rows.Count; i++) //写入各行数据
                {
                    data = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string str = dt.Rows[i][j].ToString();
                        str = str.Replace("\"", "\"\"");//替换英文引号 英文引号需要换成两个引号
                        if (str.IndexOf(',') >= 0 || str.IndexOf('"') >= 0
                          || str.IndexOf('\r') >= 0 || str.IndexOf('\n') >= 0) //含逗号 引号 换行符的需要放到引号中
                        {
                            str = string.Format("\"{0}\"", str);
                        }

                        data += str;
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);
                }
                sw.Close();
                fs.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region ReadCSV:从csv文件读取数据到DataTable(第一行作为列名)
        /// <summary>
        /// 从csv文件读取数据到DataTable(第一行作为列名)
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <returns>DataTable</returns>
        public static DataTable ReadCSV(string filePath)
        {
            Encoding encoding = GetType(filePath); //Encoding.ASCII;//
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            StreamReader sr = new StreamReader(fs, encoding);

            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                //不要开启，如果开启则顺序变为1，10，11,...。把2，3，4...排到后面去了。
                //dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();

            return dt;
        }
        #endregion

        #region ReadCSV2:从csv文件读取数据到DataTable(第一行作为列名或者作为行)
        /// <summary>
        /// 从csv文件读取数据到DataTable(第一行作为列名或者作为行)
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="columnEnable">false:第一行作为列名;true:第一行作为行</param>
        /// <param name="colCount">第一行作为行时指定列数</param>
        /// <returns>DataTable</returns>
        public static DataTable ReadCSV2(string filePath, bool columnEnable, int colCount)
        {
            Encoding encoding = GetType(filePath); //Encoding.ASCII;//
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            StreamReader sr = new StreamReader(fs, encoding);

            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst && !columnEnable)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }

                if (IsFirst && columnEnable)
                {
                    //创建列
                    columnCount = colCount;
                    tableHead = new string[columnCount];
                    for (int i = 0; i < columnCount; i++)
                    {
                        tableHead[i] = "Default Column" + i.ToString();
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                    IsFirst = false;
                }

                if (!IsFirst)
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (aryLine.Length >= columnCount)
                            dr[j] = aryLine[j];
                        if (aryLine.Length < columnCount && j < aryLine.Length)
                            dr[j] = aryLine[j];
                        if (aryLine.Length < columnCount && j >= aryLine.Length)
                            dr[j] = "";
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                //不要开启，如果开启则顺序变为1，10，11,...。把2，3，4...排到后面去了。
                //dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();

            return dt;
        }
        #endregion

        #region ReadCSVAllLines:从csv文件读取所有行到string[]
        /// <summary>
        /// 从csv文件读取所有行到string[]
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <returns>string[]</returns>
        public static string[] ReadCSVAllLines(string filePath)
        {
            //return File.ReadAllLines(filePath);//在文件被打开时读取会报错
            return WriteSafeReadAllLines(filePath);
        }
        #endregion

        #region WriteSafeReadAllLines CSV文件打开时读取
        private static string[] WriteSafeReadAllLines(String filePath)
        {
            using (var csv = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(csv))
            {
                List<string> file = new List<string>();
                while (!sr.EndOfStream)
                {
                    file.Add(sr.ReadLine());
                }
                return file.ToArray();
            }
        }
        #endregion

        #region ReadCSVLines:从csv文件读取指定行到string[]
        /// <summary>
        /// 从csv文件读取指定行到string[]
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="startRowIndex">读取的开始行数:1-N</param>
        /// <param name="rowCount">读取的行数:1-N</param>
        /// <returns>string[]</returns>
        public static string[] ReadCSVLines(string filePath, int startRowIndex, int rowCount)
        {
            string[] sa = new string[rowCount];
            Array.ConstrainedCopy(ReadCSVAllLines(filePath), startRowIndex - 1, sa, 0, rowCount);
            return sa;
        }
        #endregion

        #region InsertLineInFile:向csv文件插入一行
        /// <summary>
        /// 向csv文件插入一行
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="newLineContent">插入行内容(string)</param>
        /// <param name="position">插入的行位置(int:1-N)</param>
        /// <returns></returns>
        public static void InsertLineInFile(string filePath, string newLineContent, int position)
        {
            string[] lines = ReadCSVAllLines(filePath);
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < position - 1; i++)
                    writer.WriteLine(lines[i]);
                writer.WriteLine(newLineContent);
                for (int i = position - 1; i < lines.Length; i++)
                    writer.WriteLine(lines[i]);
            }
        }
        #endregion

        #region InsertLinesInFile:向csv文件插入N行
        /// <summary>
        /// 向csv文件插入N行
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="newLineNContent">插入N行内容(string[])</param>
        /// <param name="position">插入的行位置(int:1-N)</param>
        /// <returns></returns>
        public static void InsertLinesInFile(string filePath, string[] newLineNContent, int position)
        {
            string[] lines = ReadCSVAllLines(filePath);
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < position - 1; i++)
                    writer.WriteLine(lines[i]);
                for (int i = 0; i < newLineNContent.Length; i++)
                    writer.WriteLine(newLineNContent[i]);
                for (int i = position - 1; i < lines.Length; i++)
                    writer.WriteLine(lines[i]);
            }
        }
        #endregion

        #region ReadCSVLineRowCol:读取CSV文件的指定行,列
        /// <summary>
        /// 读取CSV文件的指定行,列
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="row">指定行:1-N</param>
        /// <param name="column">指定列:1-N</param>
        /// <returns>string</returns>
        public static string ReadCSVLineRowCol(string filePath, int row, int column)
        {
            return ((ReadCSVAllLines(filePath)[row - 1]).Split(','))[column - 1];
        }
        #endregion

        #region ReadCSVLineRow:读取CSV文件的指定行
        /// <summary>
        /// 读取CSV文件的指定行,列
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="row">指定行:1-N</param>
        /// <returns>string</returns>
        public static string ReadCSVLineRow(string filePath, int row)
        {
            return ReadCSVAllLines(filePath)[row - 1];
        }
        #endregion

        #region 给定文件的路径,读取文件的二进制数据,判断文件的编码类型
        /// <summary>
        /// 给定文件的路径,读取文件的二进制数据,判断文件的编码类型
        /// <summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件的编码类型(Encoding)</returns>
        public static Encoding GetType(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }
        #endregion

        #region 通过给定的文件流,判断文件的编码类型
        /// <summary>
        /// 通过给定的文件流,判断文件的编码类型
        /// <summary>
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型(Encoding)</returns>
        public static Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM

            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }
        #endregion

        #region 判断是否是不带 BOM 的 UTF8 格式
        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <summary>
        /// <param name="data"></param>
        /// <returns>bool</returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
        #endregion

        #region ChangeFileName:修改文件名称
        /// <summary>
        //修改文件名称
        /// <summary>
        /// <param name="oldFilePath"></param>
        /// <param name="newFilePath"></param>
        /// <returns>bool</returns>
        public static bool ChangeFileName(string oldFilePath, string newFilePath)
        {
            bool re = false;
            try
            {
                if (File.Exists(oldFilePath))
                {
                    File.Move(oldFilePath, newFilePath);
                    re = true;
                }
            }
            catch
            {
                re = false;
            }
            return re;
        }
        #endregion

        #region SaveCSV_Append:直接向csv文件中写入_Append
        /// <summary>
        /// 直接向csv文件中写入_Append
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="sData">写入内容(string):为空时不写入</param>
        /// <returns></returns>
        public static bool SaveCSV_Append(string filePath, string sData)
        {
            bool re = true;
            try
            {
                FileStream FileStream = new FileStream(filePath, FileMode.Append);
                StreamWriter sw = new StreamWriter(FileStream, Encoding.UTF8);
                if (sData != "")
                {
                    sw.WriteLine(sData);
                }
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                FileStream.Close();
            }
            catch
            {
                re = false;
            }
            return re;
        }
        #endregion

        #region SaveCSV_Write:直接向csv文件中写入_Writeline
        /// <summary>
        /// 直接向csv文件中写入_Writeline
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="sData">写入内容(string)</param>
        /// <returns></returns>
        public static bool SaveCSV_Write(string filePath, string sData)
        {
            bool re = true;
            try
            {
                FileStream FileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(FileStream, Encoding.UTF8);
                sw.WriteLine(sData);
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                FileStream.Close();
            }
            catch
            {
                re = false;
            }
            return re;
        }
        #endregion

        #region ReadLineCSV:从CSV文件读取一行数据
        /// <summary>
        /// 从CSV文件读取一行数据
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <returns>string</returns>
        public static string ReadLineCSV(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string strLine = sr.ReadLine();
            sr.Close();
            fs.Close();
            return strLine;
        }
        #endregion

        #region ReadCsvDataSetEx:从csv文件读取数据到out DataSet
        /// <summary>
        /// 从csv文件读取数据到out DataSet
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="ds">读取数据Out DataSet ds</param>
        /// <returns>bool</returns>
        public static bool ReadCsvDataSetEx(string filePath, out DataSet ds)
        {
            ds = new DataSet();
            DataTable dt = new DataTable();
            ds.Tables.Add(dt);
            if (filePath == "")
                return false;
            try
            {
                string text = File.ReadAllText(filePath, Encoding.GetEncoding(932));
                if (text == null)
                {
                    return false;
                }
                List<string[]> text_array = new List<string[]>();
                List<string> line = new List<string>();
                StringBuilder field = new StringBuilder();
                bool in_quata = false;
                bool field_start = true;
                for (int i = 0; i < text.Length; i++)
                {
                    char ch = text[i];
                    if (in_quata)
                    {
                        if (ch == '\"')
                        {
                            if (i < text.Length - 1 && text[i + 1] == '\"')
                            {
                                field.Append('\"');
                                i++;
                            }
                            else
                                in_quata = false;
                        }
                        else
                        {
                            field.Append(ch);
                        }
                    }
                    else
                    {
                        switch (ch)
                        {
                            case ',':
                                line.Add(field.ToString());
                                field.Remove(0, field.Length);
                                field_start = true;
                                break;
                            case '\"':
                                if (field_start)
                                    in_quata = true;
                                else
                                    field.Append(ch);
                                break;
                            case '\r':
                                if (field.Length > 0 || field_start)
                                {
                                    line.Add(field.ToString());
                                    field.Remove(0, field.Length);
                                }
                                text_array.Add(line.ToArray());
                                line.Clear();
                                field_start = true;
                                if (i < text.Length - 1 && text[i + 1] == '\n')
                                    i++;
                                break;
                            default:
                                field_start = false;
                                field.Append(ch); break;
                        }
                    }
                }
                if (field.Length > 0 || field_start)
                {
                    line.Add(field.ToString());
                }
                if (line.Count > 0)
                {
                    text_array.Add(line.ToArray());
                }
                for (int j = 0; j < text_array[0].Length; j++)
                {
                    DataColumn column = new DataColumn();
                    //column.ColumnName = "C_" + DataUtil.CStr(j); 
                    column.ColumnName = "C_" + j.ToString();
                    dt.Columns.Add(column);
                }
                for (int i = 0; i < text_array.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < text_array[i].Length; j++)
                    {
                        dr[j] = text_array[i][j];
                    }
                    dt.Rows.Add(dr);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region strArrayToDT:从string[]读取数据到out DataTable----只适用于读取闭门器测试结果数据!
        /// <summary>
        /// 从string[]读取数据到out DataTable----只适用于读取闭门器测试结果数据!
        /// </summary>
        /// <param name="strArray">string[]</param>
        /// <param name="startRowIndex">读取的开始行数:1-N</param>
        /// <param name="rowCount">读取的行数:1-N</param>
        /// <param name="dt">out DataTable dt</param>
        /// <returns></returns>
        public static void strArrayToDT(string[] strArray, int startRowIndex, int rowCount, out DataTable dt)
        {
            dt = new DataTable();
            int columnCount = strArray[startRowIndex - 1].Split(';').Length;

            //创建列
            string[] tableHead = new string[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                tableHead[i] = "Default Column" + i.ToString();
                DataColumn dc = new DataColumn(tableHead[i]);
                dt.Columns.Add(dc);
            }

            //添加行
            for (int i = 0; i < rowCount; i++)
            {
                string[] aryLine = strArray[startRowIndex - 1 + i].Split(';');
                DataRow dr = dt.NewRow();
                for (int j = 0; j < columnCount; j++)
                {
                    dr[j] = aryLine[j];
                }
                dt.Rows.Add(dr);
            }
        }
        #endregion

    }
}


