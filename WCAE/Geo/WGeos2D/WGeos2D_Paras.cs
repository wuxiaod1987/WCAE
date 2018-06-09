using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.WGeos2D
{
    public static class WGeos2D_Paras
    {
        #region 计算参数
        public static double E_Merge = 0.3;            /////计算用的判断两点是否重合的阈值
        public static double E_Angle = 0.0005;
        public static double E_Comp = 0.0001;
        public static double Split_Angle = 150;        /////判断角点的角度依据，角小于它则是Corner
        public static int Round = 2;

        public static double ANGLE_Max = 120;          /////用于分割曲线的最小角度值
        public static double Length_EPSILON = 0.03;    /////用于判断长度是否相近的比例
        public static int Quan_CPs = 5;                /////用于判断两线是否平行的点的数量
        #endregion
    }
}
