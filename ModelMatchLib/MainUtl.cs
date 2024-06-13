using HalconDotNet;
using MatchModel.Halcon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MatchModel
{
    public partial class MainUtl : UserControl
    {
        public const string CfgPath = "ShapeModel/";

        public MainUtl()
        {
            InitializeComponent();
        }



        ShapeModelFrm smf = new ShapeModelFrm(CfgPath);
        FindLineFrm dsf = new FindLineFrm(CfgPath);
        private void button2_Click(object sender, EventArgs e)
        {
            smf.ShowDialog();
            //设置完模板参数后，同步到检测参数中
            dsf.Init(CfgPath, smf.OrigImage, smf.AffineModelContour);
        }

        private void MainUtl_Load(object sender, EventArgs e)
        {
            smf.Init(CfgPath);

            dsf.Init(CfgPath, smf.OrigImage, smf.AffineModelContour);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dsf.ShowDialog();
        }
    }
}
