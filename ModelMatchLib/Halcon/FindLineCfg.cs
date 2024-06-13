using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchModel.Halcon
{
    public class FindLineCfg
    {
        public string RoiId { get; set; } = "FindEgdeROI_1";
        public string Transition { get; set; } = "positive";
        public string Select { get; set; } = "first";
        public int MeasureThreshold { get; set; } = 0;

        public string ImageSelect { get; set; } = "AG";
    }

    public enum CfgDescription
    {
        [Description("第一点")] first,
        [Description("最后点")] last,
        [Description("所有")] all,
        [Description("白到黑")] negative,
        [Description("黑到白")] positive,
        [Description("银浆")] AG,
        [Description("胶水")] OC,
    }
}
