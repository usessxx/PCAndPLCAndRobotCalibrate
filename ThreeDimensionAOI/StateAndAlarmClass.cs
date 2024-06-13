using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionAVI
{
    public class StateAndAlarmClass
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：状态及报警类
        //文件功能描述：用于设置报警，状态名称
        //
        //
        //创建标识：MaLi 20220402
        //
        //修改标识：MaLi 20220402 Change
        //修改描述：状态及报警类
        //
        //------------------------------------------------------------------------------------*/
        //
        const int ALARM_KIND_QUANTITY = 700;//报警，分为致命报警（index:0-99），重度报警(index:100-299)，中度报警(index:300-499)，轻度报警(index:500-699)
        const int STATE_KIND_QUANTITY = 100;//状态种类计数
        /*************************外部可设定读取参数*******************************/
        public string[] _alarmMsg = new string[ALARM_KIND_QUANTITY];//用以存储报警信息，0-英文，1-中文
        public bool[] _alarmFlag = new bool[ALARM_KIND_QUANTITY];//用于标识当前哪个报警
        public bool[] _oldAlarmFlag = new bool[ALARM_KIND_QUANTITY];//用于标识之前当前哪个报警，用于检测报警是否发生变化
        public string[] _stateMsg = new string[STATE_KIND_QUANTITY];//string数组用于保存state显示的相关message
        //*************************公共静态变量*******************************//
        
        //*************************内部私有变量*******************************//

        //
        //

        /// <summary>
        /// 构造函数
        /// </summary>
        public StateAndAlarmClass()
        {
            InitializeAlarmAndStateMsg();
        }

        #region InitializeAlarmAndStateMsg:设备状态及报警变量初始化
        private void InitializeAlarmAndStateMsg()
        {
            //报警，分为致命报警（index:0-99），重度报警(index:100-299)，中度报警(index:300-499)，轻度报警(index:500-699)
            //其中，致命报警和重度必须切换到手动才能消除，中度报警在运行过程中可以消除，轻度报警不影响设备继续运行（可以理解为警告）
            for (int i = 0; i < 100; i++)
            {
                _alarmMsg[i] = i.ToString("000") + ":致 RESERVED";
            }

            for (int i = 100; i < 300; i++)
            {
                _alarmMsg[i] = i.ToString("000") + ":重 RESERVED";
            }

            for (int i = 300; i < 500; i++)
            {
                _alarmMsg[i] = i.ToString("000") + ":中 RESERVED";
            }

            for (int i = 500; i < ALARM_KIND_QUANTITY; i++)
            {
                _alarmMsg[i] = i.ToString("000") + ":轻 RESERVED";
            }

            #region Alarm Message
            _alarmMsg[0] = "000:致 急停报警(致)";
            _alarmMsg[1] = "001:致 运动控制卡异常报警";

            _alarmMsg[100] = "100:重  X1轴伺服报警(1-IN01应当ON)";
            _alarmMsg[101] = "101:重  Y1轴伺服报警(1-IN02应当ON)";
            _alarmMsg[102] = "102:重  R1轴伺服报警(1-IN03应当ON)";

            _alarmMsg[132] = "132:重  X1轴负限位报警(1-IN33应当ON)";
            _alarmMsg[133] = "133:重  X1轴正限位报警(1-IN34应当ON)";
            _alarmMsg[134] = "134:重  Y1轴负限位报警(1-IN41应当ON)";
            _alarmMsg[135] = "135:重  Y1轴正限位报警(1-IN42应当ON)";
            _alarmMsg[136] = "136:重  R1轴负限位报警(1-IN49应当ON)";
            _alarmMsg[137] = "137:重  R1轴正限位报警(1-IN50应当ON)";
            _alarmMsg[138] = "138:重  Y2轴负限位报警(1-IN57应当ON)";
            _alarmMsg[139] = "139:重  Y2轴正限位报警(1-IN58应当ON)";
            _alarmMsg[140] = "140:重  Y3轴负限位报警(2-IN33应当ON)";
            _alarmMsg[141] = "141:重  Y3轴正限位报警(2-IN34应当ON)";
            _alarmMsg[142] = "142:重  Y4轴负限位报警(2-IN41应当ON)";
            _alarmMsg[143] = "143:重  Y4轴正限位报警(2-IN42应当ON)";

            _alarmMsg[300] = "300:中  气压低报警";

            _alarmMsg[310] = "310:中  前安全门未关好报警（1-IN11应当ON）";
            _alarmMsg[311] = "311:中  后安全门未关好报警（1-IN12应当ON）";

            _alarmMsg[320] = "320:中  相机移动气缸回超时报警(1-IN14应当ON)";
            _alarmMsg[321] = "321:中  相机移动气缸出超时报警(1-IN15应当ON)";
            _alarmMsg[322] = "322:中  传送线1(外侧)阻挡气缸下超时报警(2-IN21应当ON)";
            _alarmMsg[323] = "323:中  传送线1(外侧)阻挡气缸上超时报警(2-IN22应当ON)";
            _alarmMsg[324] = "324:中  传送线1(外侧)夹紧气缸松开超时报警(2-IN23应当ON)";
            _alarmMsg[325] = "325:中  传送线1(外侧)夹紧气缸夹紧超时报警(2-IN24应当ON)";
            _alarmMsg[326] = "326:中  传送线2(里侧)阻挡气缸下超时报警(2-IN17应当ON)";
            _alarmMsg[327] = "327:中  传送线2(里侧)阻挡气缸上超时报警(2-IN18应当ON)";
            _alarmMsg[328] = "328:中  传送线2(里侧)夹紧气缸松开超时报警(2-IN19应当ON)";
            _alarmMsg[329] = "329:中  传送线2(里侧)夹紧气缸夹紧超时报警(2-IN20应当ON)";
            _alarmMsg[340] = "340:中  传送线1上料超时报警（请检查产品是否卡住）";
            _alarmMsg[341] = "341:中  传送线1下料超时报警（请检查产品是否卡住）";
            _alarmMsg[342] = "342:中  传送线2上料超时报警（请检查产品是否卡住）";
            _alarmMsg[343] = "343:中  传送线2下料超时报警（请检查产品是否卡住）";

            _alarmMsg[350] = "350:中  传送线1产品扫码失败报警，请人工确认或输入条形码！";
            _alarmMsg[351] = "351:中  传送线2产品扫码失败报警，请人工确认或输入条形码！";
            _alarmMsg[352] = "352:中  传送线1产品扫码失败报警，请人工取走或流出无法扫码的产品！";
            _alarmMsg[353] = "353:中  传送线2产品扫码失败报警，请人工取走或流出无法扫码的产品！";

            _alarmMsg[360] = "360:中 相机连续多次抓取图像失败报警，请检测相机连线状态及设备状态！";

            _alarmMsg[370] = "370:中 入板请求MES系统通讯失败报警！";
            _alarmMsg[371] = "371:中 检测完毕，传输数据至MES系统通讯失败报警！";

            //分两个报警的目的是为了实现某些情况下不停机，继续运行功能
            _alarmMsg[380] = "380:中 设备检测产品之后直通率低于设定值报警！";
            _alarmMsg[500] = "500:轻 设备检测产品之后直通率低于设定值警告！";

            _alarmMsg[390] = "390:中  传送线1产品识别MARK点1失败报警，请人工取走或流出无法扫码的产品！";
            _alarmMsg[391] = "391:中  传送线1产品识别MARK点2失败报警，请人工取走或流出无法扫码的产品！";
            _alarmMsg[392] = "392:中  传送线2产品识别MARK点1失败报警，请人工取走或流出无法扫码的产品！";
            _alarmMsg[393] = "393:中  传送线2产品识别MARK点2失败报警，请人工取走或流出无法扫码的产品！";

            _alarmMsg[400] = "400:中  传送线1中有多余产品报警，请人工取走多余产品或选择流出所有产品！";
            _alarmMsg[401] = "401:中  传送线2中有多余产品报警，请人工取走多余产品或选择流出所有产品！";

            _alarmMsg[410] = "410:中  传送线1需要检测样品板报警，当前流入的产品不为样品板，请人工取走或流出产品！";
            _alarmMsg[510] = "510:轻  传送线1需要检测样品板警告，超过时限设备将无法检测正式产品！";
            _alarmMsg[411] = "411:中  传送线2需要检测样品板报警，当前流入的产品不为样品板，请人工取走或流出产品！";
            _alarmMsg[511] = "511:轻  传送线2需要检测样品板警告，超过时限设备将无法检测正式产品！";

            _alarmMsg[420] = "420:中  检测过程中左相机获取图像失败后，重新启动相机报警！";
            _alarmMsg[421] = "421:中  检测过程中右相机获取图像失败后，重新启动相机报警！";
            #endregion

            for (int i = 0; i < STATE_KIND_QUANTITY; i++)
            {
                _stateMsg[i] = "";
                _alarmFlag[i] = false;
            }

            #region State Message
            _stateMsg[0] = "闲置";
            _stateMsg[1] = "设备运行中";
            #endregion
        }

        #endregion
    }
}
