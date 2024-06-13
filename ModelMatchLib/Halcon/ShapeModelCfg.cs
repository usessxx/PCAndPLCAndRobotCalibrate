using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchModel.Halcon
{
    public class ShapeModelCfg
    {
        [Category("\t\t\t\t搜索角度")]
        [DisplayName("起始角度")]
        [Description("搜索模板的起始角度")]
        public int AngleStart { set; get; } = 0;

        [Category("\t\t\t\t搜索角度")]
        [DisplayName("角度范围")]
        [Description("搜索模板的角度范围")]
        public int AngleExtent { set; get; } = 360;

        [Category("\t\t\t\t搜索角度")]
        [DisplayName("步长")]
        [Description("搜索模板的角度步长")]
        public int AngleStep { set; get; } = 12;

        [Category("\t\t\t\t模板参数")]
        [DisplayName("对比度（低）")]
        [Description("")]
        public int MinContrast { set; get; } = 79;

        [Category("\t\t\t\t模板参数")]
        [DisplayName("对比度（高）")]
        [Description("")]
        public int MaxContrast { set; get; } = 146;

        [Category("\t\t\t\t模板参数")]
        [DisplayName("最小组件参数")]
        [Description("生成模板轮廓，单条最小尺寸")]
        public int MinComponentLength { set; get; } = 58;

        [Category("\t\t\t\t模板参数")]
        [DisplayName("金字塔层级")]
        [Description("")]
        public int NumLevels { set; get; } = 4;

        [Category("\t\t\t\t查找参数")]
        [DisplayName("匹配最小分数")]
        [Description("")]
        public double MinScore { set; get; } = 0.5;

        [Category("\t\t\t\t查找参数")]
        [DisplayName("匹配个数")]
        [Description("0为不限制个数")]
        public int NumMatches { set; get; } = 1;

        [Category("\t\t\t\t查找参数")]
        [DisplayName("重叠系数")]
        [Description("")]
        public double MaxOverLap { set; get; } = 0.5;


        //是否启用Ncc
        public bool EnableNcc = true;
    }
    public enum ShapeModelCfgEnum
    {
        NumLevels,
        AngleStart,
        AngleExtent,
        AngleStep,
        MinContrast,
        MaxContrast,
        MinComponentLength,
        MinScore,
        NumMatches,
        MaxOverLap,
        EnableNcc
    }


    public class FindModelROICfg
    {
        public string RoiId { set; get; } = "ModelROI_1";
    }
    public enum ShapeModelOutputEnum
    {
        MatchRow,
        MatchCol,
        MatchAngle,
        MatchScore
    }

}
