using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCAE.WGeos2D.Entities;

namespace WCAE.WGeos2D.Funcs
{
    public static class Geos2D_Modify
    {
        #region "找到曲线上按照份数划分的点集"
        /// <summary>
        /// 根据小段长度划分
        /// </summary>
        /// <param name="C">待划分曲线</param>
        /// <param name="Length">每一段的长度</param>
        /// <returns></returns>
        public static List<WPoint2D> DotCurve_Times(WEntity2D C, double Length)
        {
            int Time = (int)Math.Ceiling(((WCurve2D)C).Length / Length);
            return DotCurve_Times(C, Time); 
        }
        public static List<WPoint2D> DotCurve_Times(WEntity2D C, int Times)
        {
            if (Times == 0 || Times == 1)
            {
                List<WPoint2D> Out = new List<WPoint2D>();
                Out.Add(((WCurve2D)C).StartPoint);
                Out.Add(((WCurve2D)C).EndPoint);
                return Out;
            }
            if (C.Kind == GeoKind.Line)
                return Dotline_Times((WLine2D)C, Times);
            return DotPolyline_Times((WPolyLine2D)C, Times);
        }
        public static List<WPoint2D> DotCurve_Times(WEntity2D C, WPoint2D P1, WPoint2D P2, int Times, ref WGeometry2D WGC)
        {
            if (Times == 0 || Times == 1)
            {
                List<WPoint2D> Out = new List<WPoint2D>();
                Out.Add(((WCurve2D)C).StartPoint);
                Out.Add(((WCurve2D)C).EndPoint);
                return Out;
            }
            if (C.Kind == GeoKind.Line)
                return Dotline_Times((WLine2D)C, P1, P2, Times);
            return DotPolyline_Times((WPolyLine2D)C, P1, P2, Times, ref WGC);
        }

        /// <summary>
        /// 划分直线段
        /// </summary>
        /// <param name="L"></param>
        /// <param name="Times">实际输出的点集有Times+1个点，从0到Times</param>
        /// <returns></returns>
        private static List<WPoint2D> Dotline_Times(WLine2D L, int Times)
        {
            List<WPoint2D> Ps = new List<WPoint2D>();
            Ps.Add(L.StartPoint);
            WVector2D v = L.StartPoint.VectorTo(L.EndPoint);
            double l = v.Length / Times;
            v = v.Normalize();
            WPoint2D P = L.StartPoint;
            for (int i = 1; i < Times; i++)
            {
                P = P + v * l;
                Ps.Add(P);
            }
            Ps.Add(L.EndPoint);
            return Ps;
        }
        private static List<WPoint2D> Dotline_Times(WLine2D L, WPoint2D P1, WPoint2D P2, int Times)
        {
            List<WPoint2D> Ps = new List<WPoint2D>();
            Ps.Add(P1);
            WVector2D v = P1.VectorTo(P2);
            double l = v.Length / Times;
            v = v.Normalize();
            WPoint2D P = P1;
            for (int i = 1; i < Times; i++)
            {
                P = P + v * l;
                Ps.Add(P);
            }
            Ps.Add(P2);
            return Ps;
        }
        /// <summary>
        /// 划分Poly
        /// </summary>
        /// <param name="PL"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        private static List<WPoint2D> DotPolyline_Times(WPolyLine2D PL, int Times)
        {
            double l = PL.Length / Times;
            List<WPoint2D> Ps = new List<WPoint2D>();
            Ps.Add(PL.StartPoint);

            double[] Lsum = new double[PL.Count];   /////求出各个点到起点的距离
            Lsum[0] = 0;
            for (int i = 1; i < PL.Count; i++)
                Lsum[i] = Lsum[i - 1] + PL.Vertexs_Len[i - 1];

            int j = 0;
            WVector2D v;
            for (int i = 1; i < Times; i++)
                for (; j < Lsum.Length - 1; j++)
                    if (Lsum[j] <= (l * i) && (l * i) < Lsum[j + 1])
                    {
                        v = PL[j].VectorTo(PL[j + 1]).Normalize();
                        Ps.Add(PL[j] + v * ((i * l) - Lsum[j]));
                        break;
                    }

            Ps.Add(PL.EndPoint);
            return Ps;
        }
        private static List<WPoint2D> DotPolyline_Times(WPolyLine2D PL, WPoint2D P1, WPoint2D P2, int Times, ref WGeometry2D WGC)
        {
            ///求取最近点
            WPoint2D p1 = (WPoint2D)Geos2D_Other.ClosestPoint_P2PL(P1, PL, true);
            WPoint2D p2 = (WPoint2D)Geos2D_Other.ClosestPoint_P2PL(P2, PL, true);
            if (p1.CheckNum > p2.CheckNum)   ///如果p2排在p1之前，需要对换位置
            {
                WPoint2D p = p1;
                p1 = p2;
                p2 = p;
            }
            List<WPoint2D> Ps = new List<WPoint2D>();
            Ps.Add(p1);
            for (int i = p1.CheckNum + 1; i <= p2.CheckNum; i++)
                Ps.Add(PL[i]);
            Ps.Add(p2);
            WPolyLine2D PL2 = new WPolyLine2D(Ps, ref WGC);
            return DotPolyline_Times(PL2, Times);
        }
        #endregion

        public static void Combine_Curves(WEntity2D Co, WEntity2D Ct)
        {
            WCurve2D C1 = (WCurve2D)Co;
            WCurve2D C2 = (WCurve2D)Ct;
            int check = -1;

            /////起点相连
            if ((C1.StartPoint.X - C2.StartPoint.X) < WGeos2D_Paras.E_Merge && 
                (C1.StartPoint.Y - C2.StartPoint.Y) < WGeos2D_Paras.E_Merge)
                check = 1;

 
        }
    }
}
