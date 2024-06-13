using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace DAM02AIAO
{
    public class DAM02AIAOOperation
    {
        public SerialPort comm = new SerialPort();

        #region analogOutput
        /// <summary>
        /// analogOutput 模拟量输出(0-10V)
        /// </summary>
        /// <param name="channel">int:0,1</param>
        /// <param name="analogData">int:0-10</param>
        public void analogOutput(int channel, int analogData)
        {
            byte[] info = CModbusDll.WriteAOInfo(254, channel, Convert.ToInt16(analogData * 100));
            byte[] rst = sendinfo(info);
            if (rst != null)
                ShowAO(rst);
        }
        #endregion

        public bool OpenSerialPort(string commPortName)
        {
            //关闭时点击，则设置好端口，波特率后打开
            try
            {
                comm.PortName = commPortName; //串口名 COM1
                comm.BaudRate = 9600; //波特率  38400
                comm.DataBits = 8; // 数据位 8
                comm.ReadBufferSize = 4096;
                comm.StopBits = StopBits.One;
                comm.Parity = Parity.None;
                comm.Open();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private byte[] sendinfo(byte[] info)
        {
            if (comm == null)
            {
                //comm = new SerialPort();
                return null;
            }

            if (comm.IsOpen == false)
            {
                //OpenSerialPort();
                return null;
            }
            try
            {
                byte[] data = new byte[2048];
                int len = 0;

                comm.Write(info, 0, info.Length);
                DebugInfo("发送", info, info.Length);

                try
                {
                    Thread.Sleep(50);
                    Stream ns = comm.BaseStream;
                    ns.ReadTimeout = 50;
                    len = ns.Read(data, 0, 2048);

                    DebugInfo("接收", data, len);
                }
                catch (Exception)
                {
                    return null;
                }
                return analysisRcv(data, len);
            }
            catch (Exception)
            {

            }
            return null;
        }

        private byte[] analysisRcv(byte[] src, int len)
        {
            if (len < 6) return null;
            if (src[0] != 254) return null;

            switch (src[1])
            {
                case 0x01:
                    if (CMBRTU.CalculateCrc(src, src[2] + 5) == 0x00)
                    {
                        byte[] dst = new byte[src[2]];
                        for (int i = 0; i < src[2]; i++)
                            dst[i] = src[3 + i];
                        return dst;
                    }
                    break;
                case 0x02:
                    if (CMBRTU.CalculateCrc(src, src[2] + 5) == 0x00)
                    {
                        byte[] dst = new byte[src[2]];
                        for (int i = 0; i < src[2]; i++)
                            dst[i] = src[3 + i];
                        return dst;
                    }
                    break;
                case 0x04:
                    if (CMBRTU.CalculateCrc(src, src[2] + 5) == 0x00)
                    {
                        byte[] dst = new byte[src[2]];
                        for (int i = 0; i < src[2]; i++)
                            dst[i] = src[3 + i];
                        return dst;
                    }
                    break;
                case 0x05:
                    if (CMBRTU.CalculateCrc(src, 8) == 0x00)
                    {
                        byte[] dst = new byte[1];
                        dst[0] = src[4];
                        return dst;
                    }
                    break;
                case 0x0f:
                    if (CMBRTU.CalculateCrc(src, 8) == 0x00)
                    {
                        byte[] dst = new byte[1];
                        dst[0] = 1;
                        return dst;
                    }
                    break;
                case 0x06:
                    if (CMBRTU.CalculateCrc(src, 8) == 0x00)
                    {
                        byte[] dst = new byte[4];
                        dst[0] = src[2];
                        dst[1] = src[3];
                        dst[2] = src[4];
                        dst[3] = src[5];
                        return dst;
                    }
                    break;
                case 0x10:
                    if (CMBRTU.CalculateCrc(src, 8) == 0x00)
                    {
                        byte[] dst = new byte[4];
                        dst[0] = src[2];
                        dst[1] = src[3];
                        dst[2] = src[4];
                        dst[3] = src[5];
                        return dst;
                    }
                    break;
            }
            return null;
        }

        private string DebugInfo(string infotxt, byte[] info, int len = 0)
        {
            string debuginfo;
            StringBuilder builder = new StringBuilder();
            if (info != null)
            {
                if (len == 0) len = info.Length;
                //判断是否是显示为16禁止
                //依次的拼接出16进制字符串
                for (int i = 0; i < len; i++)
                {
                    builder.Append(info[i].ToString("X2") + " ");
                }
            }
            debuginfo = string.Format("{0}:{1}\r\n", infotxt, builder.ToString());
            builder.Clear();
            return debuginfo;
        }

        private void ShowAO(byte[] rst)
        {
            short regstart = 0;
            short regnum = 0;
            regstart = rst[0]; 
            regstart <<= 8; 
            regstart += rst[1];
            regnum = rst[2]; 
            regnum <<= 8; 
            regnum += rst[3];
            string sss = string.Format("\r\n写入AO成功：地址{0}  数量{1}\r\n", regstart, regnum);
        }
    }

    public static class CMBRTU
    {
        private readonly static ushort[] crcTable = { 
        	0X0000, 0XC0C1, 0XC181, 0X0140, 0XC301, 0X03C0, 0X0280, 0XC241, 
        	0XC601, 0X06C0, 0X0780, 0XC741, 0X0500, 0XC5C1, 0XC481, 0X0440, 
        	0XCC01, 0X0CC0, 0X0D80, 0XCD41, 0X0F00, 0XCFC1, 0XCE81, 0X0E40, 
        	0X0A00, 0XCAC1, 0XCB81, 0X0B40, 0XC901, 0X09C0, 0X0880, 0XC841, 
        	0XD801, 0X18C0, 0X1980, 0XD941, 0X1B00, 0XDBC1, 0XDA81, 0X1A40, 
        	0X1E00, 0XDEC1, 0XDF81, 0X1F40, 0XDD01, 0X1DC0, 0X1C80, 0XDC41, 
        	0X1400, 0XD4C1, 0XD581, 0X1540, 0XD701, 0X17C0, 0X1680, 0XD641, 
        	0XD201, 0X12C0, 0X1380, 0XD341, 0X1100, 0XD1C1, 0XD081, 0X1040, 
        	0XF001, 0X30C0, 0X3180, 0XF141, 0X3300, 0XF3C1, 0XF281, 0X3240, 
        	0X3600, 0XF6C1, 0XF781, 0X3740, 0XF501, 0X35C0, 0X3480, 0XF441, 
        	0X3C00, 0XFCC1, 0XFD81, 0X3D40, 0XFF01, 0X3FC0, 0X3E80, 0XFE41, 
        	0XFA01, 0X3AC0, 0X3B80, 0XFB41, 0X3900, 0XF9C1, 0XF881, 0X3840, 
        	0X2800, 0XE8C1, 0XE981, 0X2940, 0XEB01, 0X2BC0, 0X2A80, 0XEA41, 
        	0XEE01, 0X2EC0, 0X2F80, 0XEF41, 0X2D00, 0XEDC1, 0XEC81, 0X2C40, 
        	0XE401, 0X24C0, 0X2580, 0XE541, 0X2700, 0XE7C1, 0XE681, 0X2640, 
        	0X2200, 0XE2C1, 0XE381, 0X2340, 0XE101, 0X21C0, 0X2080, 0XE041, 
        	0XA001, 0X60C0, 0X6180, 0XA141, 0X6300, 0XA3C1, 0XA281, 0X6240, 
        	0X6600, 0XA6C1, 0XA781, 0X6740, 0XA501, 0X65C0, 0X6480, 0XA441, 
        	0X6C00, 0XACC1, 0XAD81, 0X6D40, 0XAF01, 0X6FC0, 0X6E80, 0XAE41, 
        	0XAA01, 0X6AC0, 0X6B80, 0XAB41, 0X6900, 0XA9C1, 0XA881, 0X6840, 
        	0X7800, 0XB8C1, 0XB981, 0X7940, 0XBB01, 0X7BC0, 0X7A80, 0XBA41, 
        	0XBE01, 0X7EC0, 0X7F80, 0XBF41, 0X7D00, 0XBDC1, 0XBC81, 0X7C40, 
        	0XB401, 0X74C0, 0X7580, 0XB541, 0X7700, 0XB7C1, 0XB681, 0X7640, 
        	0X7200, 0XB2C1, 0XB381, 0X7340, 0XB101, 0X71C0, 0X7080, 0XB041, 
        	0X5000, 0X90C1, 0X9181, 0X5140, 0X9301, 0X53C0, 0X5280, 0X9241, 
        	0X9601, 0X56C0, 0X5780, 0X9741, 0X5500, 0X95C1, 0X9481, 0X5440, 
        	0X9C01, 0X5CC0, 0X5D80, 0X9D41, 0X5F00, 0X9FC1, 0X9E81, 0X5E40, 
        	0X5A00, 0X9AC1, 0X9B81, 0X5B40, 0X9901, 0X59C0, 0X5880, 0X9841, 
        	0X8801, 0X48C0, 0X4980, 0X8941, 0X4B00, 0X8BC1, 0X8A81, 0X4A40, 
        	0X4E00, 0X8EC1, 0X8F81, 0X4F40, 0X8D01, 0X4DC0, 0X4C80, 0X8C41, 
        	0X4400, 0X84C1, 0X8581, 0X4540, 0X8701, 0X47C0, 0X4680, 0X8641, 
        	0X8201, 0X42C0, 0X4380, 0X8341, 0X4100, 0X81C1, 0X8081, 0X4040  
        };

        public static ushort CalculateCrc(byte[] data)
        {
            if (data == null)
                return 0;

            ushort crc = ushort.MaxValue;
            byte tableIndex;

            foreach (byte b in data)
            {
                tableIndex = (byte)(crc ^ b);
                crc >>= 8;
                crc ^= crcTable[tableIndex];
            }
            return crc;
        }

        public static ushort CalculateCrc(byte[] data, int len)
        {
            if (data == null)
                return 0;
            if (data.Length < len) return 0;


            ushort crc = ushort.MaxValue;
            byte tableIndex;

            for (int i = 0; i < len; i++)
            {
                tableIndex = (byte)(crc ^ data[i]);
                crc >>= 8;
                crc ^= crcTable[tableIndex];
            }
            return crc;
        }

        public static byte[] ModbusRTU(byte[] src)
        {
            //000000-Tx:FE 10 00 00 00 07 0E 00 10 00 01 00 00 07 D0 00 64 00 01 00 01 55 E0
            ushort crc = CalculateCrc(src);

            byte[] dst = new byte[src.Length + 2];
            ushort dstindex = 0;
            foreach (byte bt in src)
            {
                dst[dstindex++] = bt;
            }

            dst[dstindex++] = (byte)(crc >> 0);
            dst[dstindex++] = (byte)(crc >> 8);
            return dst;
        }
    }

    public static class CModbusDll
    {
        public static byte[] WriteDO(int addr, int io, bool openclose)
        {
            byte[] src = new byte[8];
            src[0] = (byte)addr;
            src[1] = 0x05;
            src[2] = 0x00;
            src[3] = (byte)io;
            src[4] = (byte)((openclose) ? 0xff : 0x00);
            src[5] = 0x00;
            ushort crc = CMBRTU.CalculateCrc(src, 6);
            src[6] = (byte)(crc & 0xff);
            src[7] = (byte)(crc >> 8);
            return src;
        }
        public static byte[] WriteAllDO(int addr, int ionum, bool openclose)
        {
            byte[] src = new byte[10 + (ionum - 1) / 8];
            int index = 0;
            src[index++] = (byte)addr;
            src[index++] = 0x0f;
            src[index++] = 0x00;
            src[index++] = 0x00;
            src[index++] = 0x00;
            src[index++] = (byte)ionum;
            src[index++] = (byte)((ionum + 7) / 8);
            src[index++] = (byte)((openclose) ? 0xff : 0x00);
            if (ionum > 8) src[index++] = (byte)((openclose) ? 0xff : 0x00);
            if (ionum > 16) src[index++] = (byte)((openclose) ? 0xff : 0x00);
            if (ionum > 24) src[index++] = (byte)((openclose) ? 0xff : 0x00);
            ushort crc = CMBRTU.CalculateCrc(src, index);
            src[index++] = (byte)(crc & 0xff);
            src[index++] = (byte)(crc >> 8);
            return src;
        }
        public static byte[] ReadDO(int addr, int donum)
        {
            byte[] src = new byte[8];
            src[0] = (byte)addr;
            src[1] = 0x01;
            src[2] = 0x00;
            src[3] = 0x00;
            src[4] = 0x00;
            src[5] = (byte)donum;
            ushort crc = CMBRTU.CalculateCrc(src, 6);
            src[6] = (byte)(crc & 0xff);
            src[7] = (byte)(crc >> 8);
            return src;
        }
        public static byte[] ReadDI(int addr, int dinum)
        {
            byte[] src = new byte[8];
            src[0] = (byte)addr;
            src[1] = 0x02;
            src[2] = 0x00;
            src[3] = 0x00;
            src[4] = 0x00;
            src[5] = (byte)dinum;
            ushort crc = CMBRTU.CalculateCrc(src, 6);
            src[6] = (byte)(crc & 0xff);
            src[7] = (byte)(crc >> 8);
            return src;
        }

        public static byte[] ReadAIInfo(int addr, int regstart, int regnum)
        {
            byte[] src = new byte[8];
            src[0] = (byte)addr;
            src[1] = 0x04;
            src[2] = (byte)(regstart >> 8);
            src[3] = (byte)(regstart & 0xff);
            src[4] = (byte)(regnum >> 8);
            src[5] = (byte)(regnum & 0xff);
            ushort crc = CMBRTU.CalculateCrc(src, 6);
            src[6] = (byte)(crc & 0xff);
            src[7] = (byte)(crc >> 8);
            return src;
        }

        public static byte[] WriteAOInfo(int addr, int regstart, short ao)
        {
            byte[] src = new byte[8];
            src[0] = (byte)addr;
            src[1] = 0x06;
            src[2] = (byte)(regstart >> 8);
            src[3] = (byte)(regstart & 0xff);
            src[4] = (byte)(ao >> 8);
            src[5] = (byte)(ao & 0xff);
            ushort crc = CMBRTU.CalculateCrc(src, 6);
            src[6] = (byte)(crc & 0xff);
            src[7] = (byte)(crc >> 8);
            return src;
        }

        public static byte[] WriteAOInfo(int addr, int regstart, int regnum, short[] ao)
        {
            int allnum = 6 + 1 + ao.Length * 2 + 2;
            byte[] src = new byte[allnum];
            src[0] = (byte)addr;
            src[1] = 0x10;
            src[2] = (byte)(regstart >> 8);
            src[3] = (byte)(regstart & 0xff);
            src[4] = (byte)(regnum >> 8);
            src[5] = (byte)(regnum & 0xff);
            src[6] = (byte)(ao.Length * 2);
            for (int i = 0; i < ao.Length; i++)
            {
                src[7 + 2 * i + 0] = (byte)(ao[i] >> 8);
                src[7 + 2 * i + 1] = (byte)(ao[i] & 0xff);
            }
            ushort crc = CMBRTU.CalculateCrc(src, allnum - 2);
            src[allnum - 2] = (byte)(crc & 0xff);
            src[allnum - 1] = (byte)(crc >> 8);
            return src;
        }
    }
}
