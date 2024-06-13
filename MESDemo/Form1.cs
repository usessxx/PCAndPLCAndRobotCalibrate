using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MES;
using Newtonsoft.Json;


namespace MESDemo
{
    public partial class Form1 : Form
    {
        MESOperation newMesOperation;
        public Form1()
        {
            InitializeComponent();
            newMesOperation = new MESOperation();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            newMesOperation.MEStry();
            // 序列化结果
            textBox1.Text = newMesOperation.mestrySerializationResult;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MESOperation.PanelsCheckIn newPanelsCheckIn = new MESOperation.PanelsCheckIn();
            newPanelsCheckIn.Panel = "2022022406483104525Y";
            newPanelsCheckIn.Resource = "HZ-B-1W-FLINE1";
            newPanelsCheckIn.Machine = "SMT-X-0001";
            newPanelsCheckIn.Product = "12621";
            newPanelsCheckIn.OperatorName = "33001434";
            newPanelsCheckIn.WorkArea = "SMT-FEA";
            newPanelsCheckIn.TestType = "X-MIC/X-BGA/AOI/SPI";
            newPanelsCheckIn.Site = "YCSMT";
            newPanelsCheckIn.Mac = "5C-DE-34-9B-94-3C";
            newPanelsCheckIn.ProgramName = "Jade-2826";
            newPanelsCheckIn.IpAddress = "10.2.38.12";
            newPanelsCheckIn.OptTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");

            string panelsCheckIn = newMesOperation.serializationPanelsCheckIn(newPanelsCheckIn);
            textBox2.Text = panelsCheckIn;
            MESOperation.PanelsCheckInResult newPanelsCheckInResult = new MESOperation.PanelsCheckInResult();
            newPanelsCheckInResult.result = newMesOperation.HttpPost(newMesOperation.serverlURL + newMesOperation.paneleCheckEntryPath, panelsCheckIn);
            textBox3.Text = newPanelsCheckInResult.result;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MESOperation.PanelsCheckOut newPanelsCheckOut = new MESOperation.PanelsCheckOut();

            newPanelsCheckOut.Panel = "2022022406483104525Y";
            newPanelsCheckOut.Resource = "YC2-4A-SLINE7";
            newPanelsCheckOut.Machine = "SMT-AVII-0002";
            newPanelsCheckOut.Product = "JA12670_03_B";
            newPanelsCheckOut.WorkArea = "SMT-FA";
            newPanelsCheckOut.TestType = "3DAOI";
            newPanelsCheckOut.OperatorName = "33001434";
            newPanelsCheckOut.OperatorType = "PANEL";
            newPanelsCheckOut.testMode = "PANEL";
            newPanelsCheckOut.Site = "YCSMT";
            newPanelsCheckOut.Mac = "00-1B-21-26-CE-1A";
            newPanelsCheckOut.ProgramName = "Jade-2826";
            newPanelsCheckOut.trackType = "L/R";
            newPanelsCheckOut.hasTrackFlag = "0";
            newPanelsCheckOut.IpAddress = "10.2.38.12";
            newPanelsCheckOut.CarrierId = "";
            newPanelsCheckOut.CoverPlate = "";
            newPanelsCheckOut.TestTime = "";
            string panelsCheckOut = newMesOperation.serializationPanelsCheckOut(newPanelsCheckOut);
            string tempStr = panelsCheckOut.Substring(0, panelsCheckOut.Length - 1);
            tempStr = tempStr + ",\"detaillist\":[";
            MESOperation.detaillist detailTest = new MESOperation.detaillist();


            detailTest.PanelId = "2022022406483104525Y";
            detailTest.testType = "3DAOI";
            detailTest.pcsSeq = "11";
            detailTest.partSeq = "1";
            detailTest.testResult = "pass";
            detailTest.operatorName = "33001434";
            detailTest.defectCode = "";
            detailTest.testFile = "";
            detailTest.imagePath = "";
            string tempDetailListStr = JsonConvert.SerializeObject(detailTest);
            tempStr += tempDetailListStr+",";

            detailTest.PanelId = "2022022406483104525Y";
            detailTest.testType = "3DAOI";
            detailTest.pcsSeq = "12";
            detailTest.partSeq = "1";
            detailTest.testResult = "pass";
            detailTest.operatorName = "33001434";
            detailTest.defectCode = "";
            detailTest.testFile = "";
            detailTest.imagePath = "";
            tempDetailListStr = JsonConvert.SerializeObject(detailTest);
            tempStr += tempDetailListStr+"],"+"\"summarylist\":[";

            MESOperation.summarylist test2 = new MESOperation.summarylist();
            test2.PanelId = "2022022406483104525Y";
            test2.pcsSeq = "11";
            test2.testResult = "fail";
            test2.operatorName = "33001434";
            test2.operatorTime = DateTime.Now.ToString("yy-mm-dd:hh-mm-ss");
            tempStr += JsonConvert.SerializeObject(test2)+",";

            test2.PanelId = "2022022406483104525Y";
            test2.pcsSeq = "12";
            test2.testResult = "fail";
            test2.operatorName = "33001434";
            test2.operatorTime = DateTime.Now.ToString("yy-mm-dd:hh-mm-ss");
            tempStr += JsonConvert.SerializeObject(test2) + "]}";

            panelsCheckOut = tempStr;
            textBox4.Text = panelsCheckOut;

            MESOperation.PanelsCheckOutResult newPanelsCheckOutResult = new MESOperation.PanelsCheckOutResult();
            newPanelsCheckOutResult.result = newMesOperation.HttpPost(newMesOperation.serverlURL + newMesOperation.paneleCheckExitPath, panelsCheckOut);
            textBox5.Text = newPanelsCheckOutResult.result;        
        }
    }
}
