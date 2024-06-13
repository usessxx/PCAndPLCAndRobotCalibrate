using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
//using LogCreate;
using System.Windows.Forms;
using System.Diagnostics;

namespace FX5UPLC
{
    public class FX5UPLCSLMP
    {
        private string IP;
        private int PortNumber;
        private Socket tcpClient;
        private IPAddress ipaddress;
        public bool bl_PCConnectPLCFlag;//PC与PLC已连接FLAG

        public FX5UPLCSLMP(string ip, int portnumber)
        {
            IP = ip;
            PortNumber = portnumber;
        }

        //PC通过以太网(SLMP协议)连接PLC
        public int PCConnectPLC()
        {
            int PCConnectPLCResults = 0;
            ipaddress = IPAddress.Parse(IP);
            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//新建一个socket对象（IPV4协议，数据流传输、TCP模式）
            IAsyncResult ar;
            ar = tcpClient.BeginConnect(ipaddress, PortNumber, null, null);
            bool success = ar.AsyncWaitHandle.WaitOne(3000);
            if (!success)
            {
                //LogMessageCreate.CreateLog("PC连接PLC超时!", "");
               // MessageBox.Show("数据采集系统:PC连接PLC超时!");
                PCPLCConnectClose();
                PCConnectPLCResults = 1;//PC连接PLC超时
                return PCConnectPLCResults;
            }
            if (tcpClient.Poll(1000, SelectMode.SelectRead))//判断socket状态，连接正常返回false，异常返回true;
            {
                //LogMessageCreate.CreateLog("PC连接PLC错误", "");
                //MessageBox.Show("数据采集系统:PC连接PLC错误！");

                //byte[] data = new byte[tcpClient.ReceiveBufferSize];
                //int length = tcpClient.Receive(data);
                //string s = Encoding.UTF8.GetString(data, 0, length);
                //Console.WriteLine(" 接受到的信息是: " + s);

                PCPLCConnectClose();
                PCConnectPLCResults = 2;//PC连接PLC错误
            }
            else
            {
                bl_PCConnectPLCFlag = true;
                PCConnectPLCResults = 99;//PC与PLC连接成功
                //LogMessageCreate.CreateLog("PC与PLC连接成功", "");
            }
            return PCConnectPLCResults;
        }

        //关闭PC与PLC的连接
        public void PCPLCConnectClose()
        {
            tcpClient.Close();
            bl_PCConnectPLCFlag = false;
        }

        //PC读取PLC数据(请求)
        public int PCSendRequestToPLC(string sPCReadPLCDataRequestMessage)
        {
            if (bl_PCConnectPLCFlag == false)//判断未连接时禁止发送以免代码抛出异常
            {
               // MessageBox.Show("数据采集系统:PC与PLC尚未建立连接！");
                return 1;//异常
            }
            string T_message = sPCReadPLCDataRequestMessage;
            byte[] SZ1 = HexStringToBytes(T_message);
            try
            {
                tcpClient.Send(SZ1, 0, SZ1.Length, SocketFlags.None);
               // LogMessageCreate.CreateLog("PC发送读取PLC数据请求", "");
                return 99;//正常
            }
            catch (Exception ex)
            {
                // LogMessageCreate.CreateLog("PC读取PLC数据时,发送请求到PLC失败", "");
                // LogMessageCreate.CreateErrLog(ex);
                // MessageBox.Show("数据采集系统:PC读取PLC数据时,发送请求到PLC失败" + "\r\n" + ex.ToString());
                PCPLCConnectClose();
                return 2;//异常
            }
        }

        //PC批量读取PLC寄存器D/R/W(请求)之报文
        public string PCReadPLCDataRequestMessage(int iRegisterStartNumber, string sRegisterName, int iRegisterPoints)
        {
            string _sRegisterStartNumber = Convert.ToString(iRegisterStartNumber, 16).PadLeft(4, '0').ToUpper();
            string _sTemp1 = _sRegisterStartNumber.Substring(0, 2);
            string _sTemp2 = _sRegisterStartNumber.Substring(2, 2);
            _sRegisterStartNumber = _sTemp2 + _sTemp1;

            string _sRegisterPoints = Convert.ToString(iRegisterPoints, 16).PadLeft(4, '0').ToUpper();
            _sTemp1 = _sRegisterPoints.Substring(0, 2);
            _sTemp2 = _sRegisterPoints.Substring(2, 2);
            _sRegisterPoints = _sTemp2 + _sTemp1;

            string _sRegisterName = "";
            if (sRegisterName == "D" || sRegisterName == "d")
                _sRegisterName = "00A8";
            if (sRegisterName == "R" || sRegisterName == "r")
                _sRegisterName = "00AF";
            if (sRegisterName == "W" || sRegisterName == "w")
                _sRegisterName = "00B4";

            string sRequestDatalength = "0C00";
            string sCommand = "0104";
            string sSubCommand = "0000";
            return "5000" + "00FF" + "FF03" + "00" + sRequestDatalength + "0000" + sCommand + sSubCommand + _sRegisterStartNumber + _sRegisterName + _sRegisterPoints;
        }


        //PC读取PLC数据(请求)之接收PLC响应数据
        public PCReceivePLCData PCReceivePLCDataOfRead(int iDataLength)
        {
            PCReceivePLCData readData;
            readData.iResults = 0;
            readData.sData = null;
            Stopwatch Watch = new Stopwatch();
            Watch.Start();
            while (true)//循环检测是否接收到PLC的数据
            {
                int byteLength = 1024;
                while (byteLength < 11 + iDataLength * 2)
                {
                    byteLength = byteLength + 1024;
                }

                byte[] data = new byte[byteLength];
                int length = 0;
                try
                {
                    Thread.Sleep(1);//线程挂起n ms 延时作用
                    length = tcpClient.Receive(data);//检测是否接收到PLC的数据


                    if (length <= 11 && Watch.ElapsedMilliseconds <= 3000)
                    {
                        continue;
                    }
                    if (length <= 0 && Watch.ElapsedMilliseconds > 3000)
                    {
                        // LogMessageCreate.CreateLog("PC发送读取PLC数据(请求)后等待PLC响应超时!", "");
                        //MessageBox.Show("数据采集系统:PC发送读取PLC数据(请求)后等待PLC响应超时!");
                        Watch.Stop();
                        readData.iResults = 5;//读取异常
                        return readData;
                    }
                }
                catch (Exception ex)
                {
                    //LogMessageCreate.CreateLog("PC读取PLC数据时,接收PLC响应数据失败!", "");
                    //LogMessageCreate.CreateErrLog(ex);
                    //MessageBox.Show("数据采集系统:PC读取PLC数据时,接收PLC响应数据失败!" + "\r\n" + ex.ToString());
                    readData.iResults = 1;//读取异常
                    return readData;
                }
                try//错误侦测
                {
                    Watch.Stop();
                    if (length != (11 + iDataLength * 2) && length != (22 + iDataLength * 2))
                    {
                        //LogMessageCreate.CreateLog("PC读取PLC数据时,接收的PLC响应数据长度不对!", "");
                        //MessageBox.Show("数据采集系统:PC读取PLC数据时,接收的PLC响应数据长度不对! " + length.ToString() + " != " + (11 + iDataLength * 2).ToString());
                        readData.iResults = 2;//读取异常
                        return readData;
                    }
                    byte[] data1 = new byte[length];
                    Array.Copy(data, 0, data1, 0, length);//指定长度数据复制
                    if (data1[9] != 0 || data1[10] != 0)
                    {
                        //LogMessageCreate.CreateLog("PC读取PLC数据时,PLC反馈错误! PLC反馈的错误代码：" + BitConverter.ToString(data1, 10, 1) + BitConverter.ToString(data1, 9, 1), "");
                       // MessageBox.Show("数据采集系统:PC读取PLC数据时,PLC反馈错误! PLC反馈的错误代码：" + BitConverter.ToString(data1, 10, 1) + BitConverter.ToString(data1, 9, 1));//9、10Byte是PLC返回的结束码，正常结束时为0000
                        readData.iResults = 3;//读取异常
                        return readData;
                    }
                    if (data1[9] == 0 && data1[10] == 0)
                    {
                       // LogMessageCreate.CreateLog("PC读取PLC数据时,PC成功接收PLC数据", "");
                        byte[] data2 = new byte[length - 11];              //新数组=接收数据减去返回命令码共11字节剩余返回数据
                        Array.Copy(data, 11, data2, 0, length - 11);       //数据复制至新数组

                        short[] aa1 = new short[data2.Length / 2];         //数据保存在数组aa1
                        for (int i = 0; i < data2.Length / 2; i++)
                        {
                            aa1[i] = BitConverter.ToInt16(data2, i * 2);   //将两个8位的byte高低位反转组合成一个16位的int
                        }
                        readData.iResults = 99;//读取正常
                        readData.sData = aa1;
                        return readData;
                    }
                }
                catch (Exception ex)
                {
                    //LogMessageCreate.CreateLog("PC读取PLC数据时,PC接收PLC响应数据失败!", "");
                    //LogMessageCreate.CreateErrLog(ex);
                    //MessageBox.Show("数据采集系统:PC读取PLC数据时,PC接收PLC响应数据失败!" + "\r\n" + ex.ToString());
                    readData.iResults = 4;//读取异常
                    return readData;
                }
            }
        }

        //存储PC从PLC读取的数据
        public struct PCReceivePLCData
        {
            public int iResults;//异常代码
            public short[] sData;//数据
        }

        //PC向PLC写入数据
        public int PCWritePLCData(string sPCWritePLCDataMessage)
        {
            if (bl_PCConnectPLCFlag == false)//判断未连接时禁止发送以免代码抛出异常
            {
                //MessageBox.Show("数据采集系统:PC与PLC尚未建立连接！");
                return 1;//异常
            }
            string T_message = sPCWritePLCDataMessage;
            byte[] SZ1 = HexStringToBytes(T_message);
            try
            {
                tcpClient.Send(SZ1, 0, SZ1.Length, SocketFlags.None);
                //LogMessageCreate.CreateLog("PC写入PLC数据", "");
                return 99;//正常
            }
            catch (Exception ex)
            {
                //LogMessageCreate.CreateLog("PC写入PLC数据失败", "");
                //LogMessageCreate.CreateErrLog(ex);
                //MessageBox.Show("数据采集系统:PC写入PLC数据失败" + "\r\n" + ex.ToString());
                PCPLCConnectClose();
                return 1;//异常
            }
        }

        //PC向PLC写入数据之接收PLC响应数据
        public int PCReceivePLCDataOfWrite()
        {
            Stopwatch Watch = new Stopwatch();
            Watch.Start();
            while (true)//循环检测是否接收到PLC的数据
            {
                byte[] data = new byte[1024];
                int length = 0;
                try
                {
                    Thread.Sleep(1);//线程挂起n ms 延时作用
                    length = tcpClient.Receive(data);//检测是否接收到PLC的数据
                    if (length == 0 && Watch.ElapsedMilliseconds <= 3000)
                    {
                        continue;
                    }
                    if (length == 0 && Watch.ElapsedMilliseconds > 3000)
                    {
                       // LogMessageCreate.CreateLog("PC向PLC写入数据后等待PLC响应超时!", "");
                        //MessageBox.Show("数据采集系统:PC向PLC写入数据后等待PLC响应超时!");
                        Watch.Stop();
                        return 4;//写入异常
                    }
                }
                catch (Exception ex)
                {
                    //LogMessageCreate.CreateLog("PC向PLC写入数据时,接收PLC响应数据失败!", "");
                   // LogMessageCreate.CreateErrLog(ex);
                    //MessageBox.Show("数据采集系统:PC向PLC写入数据时,接收PLC响应数据失败!" + "\r\n" + ex.ToString());
                    return 1;//写入异常
                }
                try//错误侦测
                {
                    Watch.Stop();
                    byte[] data1 = new byte[length];
                    Array.Copy(data, 0, data1, 0, length);//指定长度数据复制
                    if (data1[length-2] != 0 || data1[length-1] != 0)
                    {
                       // LogMessageCreate.CreateLog("PC向PLC写入数据时,PLC反馈错误! PLC反馈的错误代码：" + BitConverter.ToString(data1, 10, 1) + BitConverter.ToString(data1, 9, 1), "");
                        //MessageBox.Show("数据采集系统:PC向PLC写入数据时,PLC反馈错误! PLC反馈的错误代码：" + BitConverter.ToString(data1, 10, 1) + BitConverter.ToString(data1, 9, 1));//9、10Byte是PLC返回的结束码，正常结束时为0000
                        return 2;//写入异常
                    }
                    if (data1[length-2] == 0 && data1[length-1] == 0)
                    {
                       // LogMessageCreate.CreateLog("PC成功向PLC写入数据", "");
                        return 99;//写入正常
                    }
                }
                catch (Exception ex)
                {
                    //LogMessageCreate.CreateLog("PC向PLC写入数据时,PC接收PLC响应数据失败!", "");
                    //LogMessageCreate.CreateErrLog(ex);
                   // MessageBox.Show("PC向PLC写入数据时,PC接收PLC响应数据失败!" + "\r\n" + ex.ToString());
                    return 3;//写入异常
                }
            }
        }

        //PC批量写入PLC寄存器D/R/W报文----INT16(-32768 到 +32767)
        public string PCWritePLCDataMessage(int iRegisterStartNumber, string sRegisterName, int iRegisterPoints, short[] iData)
        {
            string _sRegisterStartNumber = Convert.ToString(iRegisterStartNumber, 16).PadLeft(4, '0').ToUpper();
            string _sTemp1 = _sRegisterStartNumber.Substring(0, 2);
            string _sTemp2 = _sRegisterStartNumber.Substring(2, 2);
            _sRegisterStartNumber = _sTemp2 + _sTemp1;

            string _sRegisterPoints = Convert.ToString(iRegisterPoints, 16).PadLeft(4, '0').ToUpper();
            _sTemp1 = _sRegisterPoints.Substring(0, 2);
            _sTemp2 = _sRegisterPoints.Substring(2, 2);
            _sRegisterPoints = _sTemp2 + _sTemp1;

            string _sRegisterName = "";
            if (sRegisterName == "D" || sRegisterName == "d")
                _sRegisterName = "00A8";
            if (sRegisterName == "R" || sRegisterName == "r")
                _sRegisterName = "00AF";
            if (sRegisterName == "W" || sRegisterName == "w")
                _sRegisterName = "00B4";

            string sData = "";
            for (int i = 0; i < iData.Length; i++)
            {
                string sData0 = Convert.ToString(iData[i], 16).PadLeft(4, '0').ToUpper();
                string sData1 = sData0.Substring(0, 2);
                string sData2 = sData0.Substring(2, 2);
                sData = sData + sData2 + sData1;
            }

            string sCommand = "0114";
            string sSubCommand = "0000";

            string sRequestDatalength = Convert.ToString(6 + (_sRegisterStartNumber.Length + _sRegisterName.Length + _sRegisterPoints.Length + sData.Length) / 2, 16).PadLeft(4, '0').ToUpper();
            string sRequestDatalength1 = sRequestDatalength.Substring(0, 2);
            string sRequestDatalength2 = sRequestDatalength.Substring(2, 2);
            sRequestDatalength = sRequestDatalength2 + sRequestDatalength1;

            return "5000" + "00FF" + "FF03" + "00" + sRequestDatalength + "0000" + sCommand + sSubCommand + _sRegisterStartNumber + _sRegisterName + _sRegisterPoints + sData;
        }

        //PC批量写入PLC寄存器D/R/W报文----INT32(-2,147,483,648 到 +2,147,483,647)
        public string PCWritePLCDataMessage(int iRegisterStartNumber, string sRegisterName, int iRegisterPoints, int[] iData)
        {
            string _sRegisterStartNumber = Convert.ToString(iRegisterStartNumber, 16).PadLeft(4, '0').ToUpper();
            string _sTemp1 = _sRegisterStartNumber.Substring(0, 2);
            string _sTemp2 = _sRegisterStartNumber.Substring(2, 2);
            _sRegisterStartNumber = _sTemp2 + _sTemp1;

            string _sRegisterPoints = Convert.ToString(iRegisterPoints * 2, 16).PadLeft(4, '0').ToUpper();
            _sTemp1 = _sRegisterPoints.Substring(0, 2);
            _sTemp2 = _sRegisterPoints.Substring(2, 2);
            _sRegisterPoints = _sTemp2 + _sTemp1;

            string _sRegisterName = "";
            if (sRegisterName == "D" || sRegisterName == "d")
                _sRegisterName = "00A8";
            if (sRegisterName == "R" || sRegisterName == "r")
                _sRegisterName = "00AF";
            if (sRegisterName == "W" || sRegisterName == "w")
                _sRegisterName = "00B4";

            string sData = "";
            for (int i = 0; i < iData.Length; i++)
            {
                string sData0 = Convert.ToString(iData[i], 16).PadLeft(8, '0').ToUpper();
                string sData1 = sData0.Substring(0, 2);
                string sData2 = sData0.Substring(2, 2);
                string sData3 = sData0.Substring(4, 2);
                string sData4 = sData0.Substring(6, 2);
                sData = sData + sData4 + sData3 + sData2 + sData1;
            }

            string sCommand = "0114";
            string sSubCommand = "0000";

            string sRequestDatalength = Convert.ToString(6 + (_sRegisterStartNumber.Length + _sRegisterName.Length + _sRegisterPoints.Length + sData.Length) / 2, 16).PadLeft(4, '0').ToUpper();
            string sRequestDatalength1 = sRequestDatalength.Substring(0, 2);
            string sRequestDatalength2 = sRequestDatalength.Substring(2, 2);
            sRequestDatalength = sRequestDatalength2 + sRequestDatalength1;

            return "5000" + "00FF" + "FF03" + "00" + sRequestDatalength + "0000" + sCommand + sSubCommand + _sRegisterStartNumber + _sRegisterName + _sRegisterPoints + sData;
        }

        //PC批量写入PLC寄存器D/R/W报文----Single
        public string PCWritePLCDataMessage(int iRegisterStartNumber, string sRegisterName, int iRegisterPoints, Single[] fData)
        {
            string _sRegisterStartNumber = Convert.ToString(iRegisterStartNumber, 16).PadLeft(4, '0').ToUpper();
            string _sTemp1 = _sRegisterStartNumber.Substring(0, 2);
            string _sTemp2 = _sRegisterStartNumber.Substring(2, 2);
            _sRegisterStartNumber = _sTemp2 + _sTemp1;

            string _sRegisterPoints = Convert.ToString(iRegisterPoints * 2, 16).PadLeft(4, '0').ToUpper();
            _sTemp1 = _sRegisterPoints.Substring(0, 2);
            _sTemp2 = _sRegisterPoints.Substring(2, 2);
            _sRegisterPoints = _sTemp2 + _sTemp1;

            string _sRegisterName = "";
            if (sRegisterName == "D" || sRegisterName == "d")
                _sRegisterName = "00A8";
            if (sRegisterName == "R" || sRegisterName == "r")
                _sRegisterName = "00AF";
            if (sRegisterName == "W" || sRegisterName == "w")
                _sRegisterName = "00B4";

            string sData = "";
            for (int i = 0; i < fData.Length; i++)
            {
                sData = sData + SFBTOSFH(fData[i]);
            }

            string sCommand = "0114";
            string sSubCommand = "0000";

            string sRequestDatalength = Convert.ToString(6 + (_sRegisterStartNumber.Length + _sRegisterName.Length + _sRegisterPoints.Length + sData.Length) / 2, 16).PadLeft(4, '0').ToUpper();
            string sRequestDatalength1 = sRequestDatalength.Substring(0, 2);
            string sRequestDatalength2 = sRequestDatalength.Substring(2, 2);
            sRequestDatalength = sRequestDatalength2 + sRequestDatalength1;

            return "5000" + "00FF" + "FF03" + "00" + sRequestDatalength + "0000" + sCommand + sSubCommand + _sRegisterStartNumber + _sRegisterName + _sRegisterPoints + sData;
        }

        //16进制形式编码到字节
        private static byte[] HexStringToBytes(string hexStr)
        {
            if (string.IsNullOrEmpty(hexStr))
            { return new byte[0]; }

            if (hexStr.StartsWith("0x"))
            { hexStr = hexStr.Remove(0, 2); }

            var count = hexStr.Length;
            if (count % 2 == 1)//字节长度为奇数!错误
            { return new byte[0]; }

            var byteCount = count / 2;
            var result = new byte[byteCount];
            for (int ii = 0; ii < byteCount; ++ii)
            {
                var tempBytes = Byte.Parse(hexStr.Substring(2 * ii, 2), System.Globalization.NumberStyles.HexNumber);
                result[ii] = tempBytes;
            }
            return result;
        }

        //二进制浮点数(字符串格式)转换为10进制浮点数
        //SINGLE FLOAT CONVERT CMD
        //sData:binary,32bits,b31-b00.
        public double FCONVERTCMD(string sData)
        {
            double results = 0.0;
            double resultsM1 = 0.0;
            double resultsM2 = 0.0;
            int j0 = 0;
            int j1 = -1;
            int j2 = 7;
            int[] intResults = new int[32];
            while (sData.Length < 32)
            {
                sData = "0" + sData;
            }
            if (sData == "00000000000000000000000000000000")
                return 0.0;
            for (int i = 31; i >= 0; i--)
            {
                intResults[j0] = Convert.ToInt32(sData.Substring(i, 1));
                j0++;
            }
            for (int i = 22; i >= 0; i--)
            {
                resultsM1 = resultsM1 + intResults[i] * Math.Pow(2, j1);
                j1--;
            }
            resultsM1 = resultsM1 + 1;
            for (int i = 30; i >= 23; i--)
            {
                resultsM2 = resultsM2 + intResults[i] * Math.Pow(2, j2);
                j2--;
            }
            resultsM2 = Math.Pow(2, resultsM2) / Math.Pow(2, 127);

            results = resultsM1 * resultsM2;
            if (intResults[31] == 1)
                results = results * -1;
            return results;
        }

        //把10进制单精度浮点数转换为二进制数据
        //Single float Decimal convert to Single float Binary
        public string SFDTOSFB(Single data)
        {
            string results = "";
            string IntStrs = "";
            //Single dataAbs, dataAbsFlt, dataAbsFltM2;
            double dataAbs, dataAbsFlt, dataAbsFltM2;
            long dataAbsInt, resultsL;
            if (data == 0.0f)
            {
                return "00000000000000000000000000000000";
            }
            if (data < 0)
                dataAbs = data * -1;
            else
                dataAbs = data;
            dataAbsInt = (long)dataAbs;
            dataAbsFlt = dataAbs % 1;
            IntStrs = Convert.ToString(dataAbsInt, 2);
            //dataAbs<1
            if (dataAbs < 1)
            {
                int xxx = 70;
                string[] ss = new string[xxx];
                int i = 0;
                dataAbsFltM2 = dataAbsFlt * 2;
                while (i < xxx)
                {
                    int dataAbsFltInt = (int)(dataAbsFltM2);
                    double dataAbsFltFlt = dataAbsFltM2 % 1;
                    ss[i] = dataAbsFltInt.ToString();
                    dataAbsFltM2 = dataAbsFltFlt * 2;
                    i++;
                }
                results = "";
                foreach (string sss in ss)
                {
                    results += sss;
                }
                if (results == "0000000000000000000000000000000000000000000000000000000000000000000000")
                {
                    return "00000000000000000000000000000000";
                }
                else
                {
                    int j = 0;
                    while (results.Substring(0, 1) == "0")
                    {
                        results = results.Substring(1, results.Length - 1);
                        j++;
                    }
                    results = results.Substring(1, results.Length - 1);
                    string tStrs2 = Convert.ToString(127 - j - 1, 2);
                    while (tStrs2.Length < 8)
                    {
                        tStrs2 = "0" + tStrs2;
                    }
                    results = tStrs2 + results;
                    while (results.Length < 31)
                    {
                        results = results + "0";
                    }
                    if (data < 0)
                        results = "1" + results;
                    else
                        results = "0" + results;
                    return results;
                }
            }
            //dataAbs>=1
            else
            {
                int dddd = 24;
                if (IntStrs.Length < dddd)
                {
                    int i = 0;
                    string[] ss = new string[dddd - IntStrs.Length];

                    dataAbsFltM2 = dataAbsFlt * 2;

                    while (i < dddd - IntStrs.Length)
                    {
                        int dataAbsFltInt = (int)(dataAbsFltM2);
                        double dataAbsFltFlt = dataAbsFltM2 % 1;
                        ss[i] = dataAbsFltInt.ToString();
                        dataAbsFltM2 = dataAbsFltFlt * 2;
                        i++;
                    }
                    foreach (string sss in ss)
                    {
                        results += sss;
                    }
                    results = IntStrs + results;
                    results = results.Substring(1, dddd - 1);
                }
                else
                {
                    results = IntStrs.Substring(1, dddd - 1);
                    if (IntStrs.Length > dddd)
                    {
                        if (IntStrs.Substring(dddd, 1) == "1")
                        {
                            resultsL = Convert.ToInt32(results, 2);
                            resultsL = resultsL + 1;
                            results = Convert.ToString(resultsL, 2);
                        }
                    }
                }
                string tStrs = Convert.ToString(127 + IntStrs.Length - 1, 2);
                while (tStrs.Length < 8)
                {
                    tStrs = "0" + tStrs;
                }
                results = tStrs + results;

                if (data < 0)
                    results = "1" + results;
                else
                    results = "0" + results;
                return results;
            }
        }

        //把二进制浮点数数据转换为16进制字符串形式
        //Single float Binaryconvert to Single float Hex 
        public string SFBTOSFH(Single fData)
        {
            string[] dataStrs = new string[8];
            string ss = SFDTOSFB(fData);//把10进制单精度浮点数转换为二进制数据
            string datas = "";
            dataStrs[0] = ss.Substring(0, 4);//每4个bits一分割
            dataStrs[1] = ss.Substring(4, 4);
            dataStrs[2] = ss.Substring(8, 4);
            dataStrs[3] = ss.Substring(12, 4);
            dataStrs[4] = ss.Substring(16, 4);
            dataStrs[5] = ss.Substring(20, 4);
            dataStrs[6] = ss.Substring(24, 4);
            dataStrs[7] = ss.Substring(28, 4);
            datas = datas + (Convert.ToInt32(dataStrs[0], 2)).ToString("X");//每4个bits的二进制数据(字符串形式)先转化为10进制整数,然后再转换为16进制字符串
            datas = datas + (Convert.ToInt32(dataStrs[1], 2)).ToString("X");
            datas = datas + (Convert.ToInt32(dataStrs[2], 2)).ToString("X");
            datas = datas + (Convert.ToInt32(dataStrs[3], 2)).ToString("X");
            datas = datas + (Convert.ToInt32(dataStrs[4], 2)).ToString("X");
            datas = datas + (Convert.ToInt32(dataStrs[5], 2)).ToString("X");
            datas = datas + (Convert.ToInt32(dataStrs[6], 2)).ToString("X");
            datas = datas + (Convert.ToInt32(dataStrs[7], 2)).ToString("X");
            return datas.Substring(6, 2) + datas.Substring(4, 2) + datas.Substring(2, 2) + datas.Substring(0, 2);
        }

    }
}
