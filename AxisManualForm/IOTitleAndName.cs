using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AxisAndIOForm
{
    public class IOTitleAndName
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：IO标题及名称类
        //文件功能描述：用于存储IO的标题及名称类
        //
        //
        //创建标识：MaLi 20220402
        //
        //修改标识：MaLi 20220402 Change
        //修改描述：IO标题及名称类
        //
        //
        //------------------------------------------------------------------------------------*/

        //*************************外部可设定读取变量*******************************//
        public string[,] _ioName;//io名称，外部设置，用于标识IO的名称，第一组0为输入，第二组1为输出
        public string[,] _ioTitleName;//io标题名称
        public Color[] _signalClr = new Color[4] { Color.Gray, Color.LightGreen, Color.Gray, Color.Red };//信号颜色，0-input off时的颜色，1-input on时的颜色，2-output off时的颜色，3-output on时的颜色
        public Color[] _textClr = new Color[4] { Color.White, Color.Black, Color.White, Color.Black };//字体颜色，0-input off时的颜色，1-input on时的颜色，2-output off时的颜色，3-output on时的颜色
        //*************************内部私有变量*******************************//
        //
        //

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxIOQuantity"></param>
        public IOTitleAndName(int maxIOQuantity)
        {
            _ioTitleName = new string[2, maxIOQuantity];
            _ioName = new string[2, maxIOQuantity];
            VariateInitial(maxIOQuantity);
        }

        #region  VariateInitial:变量初始化
        private void VariateInitial(int maxIOQuantity)
        {
            for (int i = 0; i < maxIOQuantity; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    _ioName[j, i] = "空";
                    _ioTitleName[j, i] = "NULL";
                }
            }

            #region IO标题名
            for (int i = 0; i < maxIOQuantity; i++)
            {
                _ioTitleName[0, i] = (i / maxIOQuantity).ToString() + "-IN" + (i % maxIOQuantity).ToString("00");
                _ioTitleName[1, i] = (i / maxIOQuantity).ToString() + "-OUT" + (i % maxIOQuantity).ToString("00");
            }

            #endregion

            #region IO名称
            //每块卡最多支持320个输入及输出(包含扩展模块)
            //#1卡输入
            _ioName[0, 0] = "启动按钮";
            _ioName[0, 1] = "停止按钮";
            _ioName[0, 2] = "复位按钮";
            _ioName[0, 3] = "急停按钮";
            _ioName[0, 4] = "0-IN04";
            _ioName[0, 5] = "前安全门传感器";
            _ioName[0, 6] = "左移载顶升气缸下检测";
            _ioName[0, 7] = "左移载顶升气缸上检测";
            _ioName[0, 8] = "右移载顶升气缸下检测";
            _ioName[0, 9] = "右移载顶升气缸上检测";
            _ioName[0, 10] = "左侧平台物料感应检测(备用)";
            _ioName[0, 11] = "工作台吸盘气压检测";
            _ioName[0, 12] = "左移栽取料吸盘气压检测";
            _ioName[0, 13] = "右移栽取料吸盘气压检测";
            _ioName[0, 14] = "左侧平台吸盘气压检测(备用)";
            _ioName[0, 15] = "前机请求出板信号(备用)";
            _ioName[0, 16] = "后机允许出板信号(备用)";
            _ioName[0, 17] = "后安全门传感器";
            _ioName[0, 18] = "0-IN18";
            _ioName[0, 19] = "0-IN19";
            _ioName[0, 20] = "0-IN20";
            _ioName[0, 21] = "0-IN21";
            _ioName[0, 22] = "0-IN22";
            _ioName[0, 23] = "0-IN23";
            _ioName[0, 24] = "0-IN24";
            _ioName[0, 25] = "0-IN25";
            _ioName[0, 26] = "0-IN26";
            _ioName[0, 27] = "0-IN27";
            _ioName[0, 28] = "0-INNA";
            _ioName[0, 29] = "0-INNA";
            _ioName[0, 30] = "0-INNA";
            _ioName[0, 31] = "0-INNA";
            _ioName[0, 32] = "0-INNA";
            _ioName[0, 33] = "0-INNA";
            _ioName[0, 34] = "0-INNA";
            _ioName[0, 35] = "0-INNA";
            _ioName[0, 36] = "0-INNA";
            _ioName[0, 37] = "0-INNA";
            _ioName[0, 38] = "0-INNA";
            _ioName[0, 39] = "0-INNA";
            _ioName[0, 40] = "0-INNA";
            _ioName[0, 41] = "0-INNA";
            _ioName[0, 42] = "0-INNA";
            _ioName[0, 43] = "0-INNA";
            _ioName[0, 44] = "0-INNA";
            _ioName[0, 45] = "0-INNA";
            _ioName[0, 46] = "0-INNA";
            _ioName[0, 47] = "0-INNA";
            _ioName[0, 48] = "0-INNA";
            _ioName[0, 49] = "0-INNA";
            _ioName[0, 50] = "0-INNA";
            _ioName[0, 51] = "0-INNA";
            _ioName[0, 52] = "0-INNA";
            _ioName[0, 53] = "0-INNA";
            _ioName[0, 54] = "0-INNA";
            _ioName[0, 55] = "0-INNA";
            _ioName[0, 56] = "0-INNA";
            _ioName[0, 57] = "0-INNA";
            _ioName[0, 58] = "0-INNA";
            _ioName[0, 59] = "0-INNA";
            _ioName[0, 60] = "0-INNA";
            _ioName[0, 61] = "0-INNA";
            _ioName[0, 62] = "0-INNA";
            _ioName[0, 63] = "0-INNA";

            _ioName[0, 64] = "0-IN64";
            _ioName[0, 65] = "0-IN65";
            _ioName[0, 66] = "0-IN66";
            _ioName[0, 67] = "0-IN67";
            _ioName[0, 68] = "0-IN68";
            _ioName[0, 69] = "0-IN69";
            _ioName[0, 70] = "0-IN70";
            _ioName[0, 71] = "0-IN71";
            _ioName[0, 72] = "0-IN72";
            _ioName[0, 73] = "0-IN73";
            _ioName[0, 74] = "0-IN74";
            _ioName[0, 75] = "0-IN75";
            _ioName[0, 76] = "0-IN76";
            _ioName[0, 77] = "0-IN77";
            _ioName[0, 78] = "0-IN78";
            _ioName[0, 79] = "0-IN79";
            _ioName[0, 80] = "0-IN80";
            _ioName[0, 81] = "0-IN81";
            _ioName[0, 82] = "0-IN82";
            _ioName[0, 83] = "0-IN83";
            _ioName[0, 84] = "0-IN84";
            _ioName[0, 85] = "0-IN85";
            _ioName[0, 86] = "0-IN86";
            _ioName[0, 87] = "0-IN87";
            _ioName[0, 88] = "0-IN88";
            _ioName[0, 89] = "0-IN89";
            _ioName[0, 90] = "0-IN90";
            _ioName[0, 91] = "0-IN91";
            _ioName[0, 92] = "0-IN92";
            _ioName[0, 93] = "0-IN93";
            _ioName[0, 94] = "0-IN94";
            _ioName[0, 95] = "0-IN95";
            _ioName[0, 96] = "0-IN96";
            _ioName[0, 97] = "0-IN97";
            _ioName[0, 98] = "0-IN98";
            _ioName[0, 99] = "0-IN99";
            _ioName[0, 100] = "0-IN100";
            _ioName[0, 101] = "0-IN101";
            _ioName[0, 102] = "0-IN102";
            _ioName[0, 103] = "0-IN103";
            _ioName[0, 104] = "0-IN104";
            _ioName[0, 105] = "0-IN105";
            _ioName[0, 106] = "0-IN106";
            _ioName[0, 107] = "0-IN107";
            _ioName[0, 108] = "0-IN108";
            _ioName[0, 109] = "0-IN109";
            _ioName[0, 110] = "0-IN100";
            _ioName[0, 111] = "0-IN111";
            _ioName[0, 112] = "0-IN112";
            _ioName[0, 113] = "0-IN113";
            _ioName[0, 114] = "0-IN114";
            _ioName[0, 115] = "0-IN115";
            _ioName[0, 116] = "0-IN116";
            _ioName[0, 117] = "0-IN117";
            _ioName[0, 118] = "0-IN118";
            _ioName[0, 119] = "0-IN119";
            _ioName[0, 120] = "0-IN120";
            _ioName[0, 121] = "0-IN121";
            _ioName[0, 122] = "0-IN122";
            _ioName[0, 123] = "0-IN123";
            _ioName[0, 124] = "0-IN124";
            _ioName[0, 125] = "0-IN125";
            _ioName[0, 126] = "0-IN126";
            _ioName[0, 127] = "0-IN127";

            //#1卡输出
            _ioName[1, 0] = "三色灯-红灯";
            _ioName[1, 1] = "三色灯-黄灯";
            _ioName[1, 2] = "三色灯-绿灯";
            _ioName[1, 3] = "三色灯-蜂鸣器";
            _ioName[1, 4] = "工作台吸盘负压";
            _ioName[1, 5] = "工作台吸盘破负压";
            _ioName[1, 6] = "左移载顶升气缸2-1";
            _ioName[1, 7] = "左移载顶升气缸2-2";
            _ioName[1, 8] = "右移载顶升气缸2-1";
            _ioName[1, 9] = "右移载顶升气缸2-2";
            _ioName[1, 10] = "左移载取料吸盘负压";
            _ioName[1, 11] = "左移载取料吸盘破负压";
            _ioName[1, 12] = "右移载取料吸盘负压";
            _ioName[1, 13] = "右移载取料吸盘破负压";
            _ioName[1, 14] = "左侧平台吸盘负压(备用)";
            _ioName[1, 15] = "左侧平台吸盘破负压(备用)";
            _ioName[1, 16] = "0-OUTNA";
            _ioName[1, 17] = "0-OUTNA";
            _ioName[1, 18] = "0-OUTNA";
            _ioName[1, 19] = "0-OUTNA";
            _ioName[1, 20] = "0-OUTNA";
            _ioName[1, 21] = "0-OUTNA";
            _ioName[1, 22] = "0-OUTNA";
            _ioName[1, 23] = "0-OUTNA";
            _ioName[1, 24] = "0-OUTNA";
            _ioName[1, 25] = "0-OUTNA";
            _ioName[1, 26] = "0-OUTNA";
            _ioName[1, 27] = "0-OUTNA";
            _ioName[1, 28] = "0-OUTNA";
            _ioName[1, 29] = "0-OUTNA";
            _ioName[1, 30] = "0-OUTNA";
            _ioName[1, 31] = "0-OUTNA";
            _ioName[1, 32] = "0-OUTNA";
            _ioName[1, 33] = "0-OUTNA";
            _ioName[1, 34] = "0-OUTNA";
            _ioName[1, 35] = "0-OUTNA";
            _ioName[1, 36] = "0-OUTNA";
            _ioName[1, 37] = "0-OUTNA";
            _ioName[1, 38] = "0-OUTNA";
            _ioName[1, 39] = "0-OUTNA";
            _ioName[1, 40] = "0-OUTNA";
            _ioName[1, 41] = "0-OUTNA";
            _ioName[1, 42] = "0-OUTNA";
            _ioName[1, 43] = "0-OUTNA";
            _ioName[1, 44] = "0-OUTNA";
            _ioName[1, 45] = "0-OUTNA";
            _ioName[1, 46] = "0-OUTNA";
            _ioName[1, 47] = "0-OUTNA";
            _ioName[1, 48] = "0-OUTNA";
            _ioName[1, 49] = "0-OUTNA";
            _ioName[1, 50] = "0-OUTNA";
            _ioName[1, 51] = "0-OUTNA";
            _ioName[1, 52] = "0-OUTNA";
            _ioName[1, 53] = "0-OUTNA";
            _ioName[1, 54] = "0-OUTNA";
            _ioName[1, 55] = "0-OUTNA";
            _ioName[1, 56] = "0-OUTNA";
            _ioName[1, 57] = "0-OUTNA";
            _ioName[1, 58] = "0-OUTNA";
            _ioName[1, 59] = "0-OUTNA";
            _ioName[1, 60] = "0-OUTNA";
            _ioName[1, 61] = "0-OUTNA";
            _ioName[1, 62] = "0-OUTNA";
            _ioName[1, 63] = "0-OUTNA";

            _ioName[1, 64] = "启动按钮灯";
            _ioName[1, 65] = "停止按钮灯";
            _ioName[1, 66] = "复位按钮灯";
            _ioName[1, 67] = "0-OUT67";
            _ioName[1, 68] = "LED照明灯控制继电器";
            _ioName[1, 69] = "0-OUT69";
            _ioName[1, 70] = "0-OUT70";
            _ioName[1, 71] = "0-OUT71";
            _ioName[1, 72] = "0-OUT72";
            _ioName[1, 73] = "0-OUT73";
            _ioName[1, 74] = "0-OUT74";
            _ioName[1, 75] = "0-OUT75";
            _ioName[1, 76] = "0-OUT76";
            _ioName[1, 77] = "0-OUT77";
            _ioName[1, 78] = "0-OUT78";
            _ioName[1, 79] = "0-OUT79";
            _ioName[1, 80] = "0-OUT80";
            _ioName[1, 81] = "0-OUT81";
            _ioName[1, 82] = "0-OUT82";
            _ioName[1, 83] = "0-OUT83";
            _ioName[1, 84] = "0-OUT84";
            _ioName[1, 85] = "0-OUT85";
            _ioName[1, 86] = "0-OUT86";
            _ioName[1, 87] = "0-OUT87";
            _ioName[1, 88] = "0-OUT88";
            _ioName[1, 89] = "0-OUT89";
            _ioName[1, 90] = "0-OUT90";
            _ioName[1, 91] = "0-OUT91";
            _ioName[1, 92] = "0-OUT92";
            _ioName[1, 93] = "0-OUT93";
            _ioName[1, 94] = "0-OUT94";
            _ioName[1, 95] = "0-OUT95";
            _ioName[1, 96] = "0-OUT96";
            _ioName[1, 97] = "0-OUT97";
            _ioName[1, 98] = "0-OUT98";
            _ioName[1, 99] = "0-OUT99";
            _ioName[1, 100] = "0-OUT100";
            _ioName[1, 101] = "0-OUT101";
            _ioName[1, 102] = "0-OUT102";
            _ioName[1, 103] = "0-OUT103";
            _ioName[1, 104] = "0-OUT104";
            _ioName[1, 105] = "0-OUT105";
            _ioName[1, 106] = "0-OUT106";
            _ioName[1, 107] = "0-OUT107";
            _ioName[1, 108] = "0-OUT108";
            _ioName[1, 109] = "0-OUT109";
            _ioName[1, 110] = "0-OUT110";
            _ioName[1, 111] = "0-OUT111";
            _ioName[1, 112] = "0-OUT112";
            _ioName[1, 113] = "0-OUT113";
            _ioName[1, 114] = "0-OUT114";
            _ioName[1, 115] = "0-OUT115";
            _ioName[1, 116] = "0-OUT116";
            _ioName[1, 117] = "0-OUT117";
            _ioName[1, 118] = "0-OUT118";
            _ioName[1, 119] = "0-OUT119";
            _ioName[1, 120] = "0-OUT120";
            _ioName[1, 121] = "0-OUT121";
            _ioName[1, 122] = "0-OUT122";
            _ioName[1, 123] = "0-OUT123";
            _ioName[1, 124] = "0-OUT124";
            _ioName[1, 125] = "0-OUT125";
            _ioName[1, 126] = "0-OUT126";
            _ioName[1, 127] = "0-OUT127";
            #endregion
        }
        #endregion

    }
}
