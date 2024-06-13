using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchModel.Halcon
{
    public class DetectSilverCfg
    {
        public string RoiId { get; set; } = "SilverROI_1";

        /// <summary>
        /// 获取关于roi 中提取银浆 所有边缘信息
        /// </summary>
        /// <param name="num">银浆个数</param>
        /// <returns></returns>
        public List<string> GetAllNames(int AgNum,int CuEdgeNum)
        {
            List<string> names = new List<string>();
            string[] direct = new string[] { "Top", "Down", "Left", "Right" };
            for (int i = 0; i < AgNum; i++)
            {
                for (int j = 0; j < direct.Length; j++)
                {
                    names.Add($"{RoiId}_{i + 1}_{direct[j]}");
                }
            }
            for (int i = 0; i < CuEdgeNum; i++)
            {
                names.Add($"{RoiId}_Cu_{i + 1}");
            }
            return names;
        }
    }

    public class FAICfg
    {
        public string FAIName { set; get; }
        public string MeasureA { set; get; }
        public string MeasureB { set; get; }
        public double MaxValue { set; get; }
        public double MinValue { set; get; }
    }

    public enum SilverOutputEnum
    {
        RowEdgeTops,
        ColumnEdgeTops,
        RowEdgeDowns,
        ColumnEdgeDowns,
        RowEdgeLefts,
        ColumnEdgeLefts,
        RowEdgeRights,
        ColumnEdgeRights,
        CuEdgeRows,
        CuEdgeColumns,
        SilverNumber
    }
}
