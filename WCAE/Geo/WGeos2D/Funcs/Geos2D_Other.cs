using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCAE.WGeos2D.Entities;

namespace WCAE.WGeos2D.Funcs
{
    public static class Geos2D_Other
    {
        #region 点到两点连线的最近点
        public static WPoint2D ClosestPoint_P2C(WPoint2D P, WEntity2D C, bool Extent)
        {
            if (C.Kind == GeoKind.Line)
                return ClosestPoint_P2L(P, (WLine2D)C, Extent);
            return ClosestPoint_P2PL(P, (WPolyLine2D)C, Extent);
        }

        /// <summary>
        /// 点P到Poly的最近点
        /// </summary>
        /// <param name="P"></param>
        /// <param name="PL"></param>
        /// <param name="Extent">是否超出，如false当最近点不在两点之间时返回null</param>
        /// <returns></returns>
        public static WPoint2D ClosestPoint_P2PL(WPoint2D P, WPolyLine2D PL, bool Extent)
        {
            WVector2D v; /////P2-P 
            WVector2D r; /////P2-P1
            double dotProduct, dot0;
            WPoint2D Po; /////用于输出的变量

            v = PL[0].VectorTo(P);     /////开始反向使用
            r = PL[0].VectorTo(PL[1]).Normalize();
            dotProduct = v.DotProduct(r);
            if (dotProduct <= 0)       /////这决定了该函数只能用在不复杂的情况
            {
                if (!Extent) return null;
                else
                {
                    Po = PL[0];
                    Po.CheckNum = 0;
                    return Po;
                }
            }

            dot0 = dotProduct;

            for (int i = 1; i < PL.Count; i++)
            {
                v = PL[i].VectorTo(P);
                r = PL[i].VectorTo(PL[i - 1]).Normalize();
                dotProduct = v.DotProduct(r);
                
                if (dotProduct >= 0)
                {
                    double l = PL[i - 1].DistanceTo(PL[i]);
                    WVector2D alongVector = dot0 * r.Negate();
                    Po = PL[i - 1] + alongVector;
                    Po.CheckNum = i - 1;   /////输出时将该点的之前节点的编号也输出，利于下一步使用
                    return Po;
                }
                dot0 = dotProduct * -1;
            }

            if (!Extent) return null;
            else
            {
                Po = PL[PL.Count - 1];
                Po.CheckNum = PL.Count - 2;
                return Po;
            }
                
        }

        /// <summary>
        /// 点P到P1P2两点连线的最近点
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="P"></param>
        /// <param name="Extent">是否超出，如false当最近点不在两点之间时返回null</param>
        /// <returns></returns>
        public static WPoint2D ClosestPoint_P2L(WPoint2D P1, WPoint2D P2, WPoint2D P, bool Extent)
        {
            WVector2D v = P1.VectorTo(P);
            WVector2D r = new WVector2D(P2.X - P1.X, P2.Y - P1.Y);     /////P1-P2
            r = r.Normalize();
            double l = P1.DistanceTo(P2);
            double dotProduct = v.DotProduct(r);
            if (!Extent)
            {
                if (dotProduct < 0)
                    return null;
                if (dotProduct > l)
                    return null;
            }
            WVector2D alongVector = dotProduct * r;
            return P1 + alongVector;
        }
        public static WPoint2D ClosestPoint_P2L(WPoint2D P, WLine2D L, bool Extent)
        {
            return ClosestPoint_P2L(L.StartPoint, L.EndPoint, P, Extent);
        }

        public static List<WPoint2D> ClosestPoint_Ps2Cs(WEntity2D C, ref  List<WPoint2D> Ps)
        {
            List<WPoint2D> Pso = new List<WPoint2D>();
            WPoint2D P;
            for (int i = 0; i < Ps.Count; i++)
            {
                P = ClosestPoint_P2C(Ps[i], C, true);
                if (P == null) continue;
                Pso.Add((WPoint2D)P);    
            }
            return Pso; 
        }
        #endregion

        #region 求点到线的最近距离
        public static double ClosestDistance_P2C(WEntity2D E, WPoint2D P, bool Extent)
        {
            if (E.Kind == GeoKind.Line)
            {
                WLine2D L = (WLine2D)E;
                return ClosestDistance_P2L(L.StartPoint, L.EndPoint, P, Extent);
            }
            else
            {
                WPolyLine2D PL = (WPolyLine2D)E;
                return ClosestDistance_P2PL(P, PL, Extent);
            }        }

        /// <summary>
        /// 求点P到P1P2连线的距离
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="P"></param>
        /// <param name="Extent">是否超出，如false当最近点不在两点之间时返回最大值</param>
        /// <returns></returns>
        public static double ClosestDistance_P2L(WPoint2D P1, WPoint2D P2, WPoint2D P, bool Extent)
        {
            WVector2D v = P1.VectorTo(P);
            WVector2D r = new WVector2D(P2.X - P1.X, P2.Y - P1.Y);     /////P1-P2
            r = r.Normalize();
            double l = P1.DistanceTo(P2);
            double dotProduct = v.DotProduct(r);
            if (!Extent)
            {
                if (dotProduct < 0 || dotProduct > l)
                    return double.MaxValue;
            }
            else
            {
                if (dotProduct < 0) return P1.DistanceTo(P);
                if (dotProduct > l) return P2.DistanceTo(P);
            }

            WVector2D alongVector = dotProduct * r;
            WPoint2D Pm = P1 + alongVector;
            return P.DistanceTo(Pm);
        }

        /// <summary>
        /// 求点P到Poly的距离
        /// </summary>
        /// <param name="P"></param>
        /// <param name="PL"></param>
        /// <param name="Extent">是否超出，如false当最近点不在两点之间时返回最大值</param>
        /// <returns></returns>
        public static double ClosestDistance_P2PL(WPoint2D P, WPolyLine2D PL, bool Extent)
        {
            WVector2D v; /////P2-P 
            WVector2D r; /////P2-P1
            double dotProduct, dot0;

            v = PL[0].VectorTo(P);     /////开始反向使用
            r = PL[0].VectorTo(PL[1]).Normalize();
            dotProduct = v.DotProduct(r);
            if (dotProduct <= 0)       /////这决定了该函数只能用在不复杂的情况
            {
                if (!Extent) return double.MaxValue;
                else return PL[0].DistanceTo(P);
            }

            dot0 = dotProduct;

            for (int i = 1; i < PL.Count; i++)
            {
                v = PL[i].VectorTo(P);
                r = PL[i].VectorTo(PL[i - 1]).Normalize();
                dotProduct = v.DotProduct(r);

                if (dotProduct >= 0)
                {
                    double l = PL[i - 1].DistanceTo(PL[i]);
                    WVector2D alongVector = dot0 * r.Negate();
                    WPoint2D Pm = PL[i - 1] + alongVector;
                    return Pm.DistanceTo(P);
                }
                dot0 = dotProduct * -1;
            }

            if (!Extent) return double.MaxValue;
            else return PL[PL.Count - 1].DistanceTo(P);
        }
        #endregion

        #region 找圆的圆心
        /// <summary>
        /// 找到所有圆的圆心并整体输出
        /// </summary>
        /// <param name="Cs"></param>
        /// <returns></returns>
        public static List<WPoint2D> GetCenter_Circles(ref WEntity2D[] Cs)
        {
            List<WPoint2D> Ps = new List<WPoint2D>();
            for (int i = 0; i < Cs.Length; i++)
                if (Cs[i].Kind == GeoKind.Circle)
                    Ps.Add(((WCircle2D)Cs[i]).Center);
            return Ps;
        }
        #endregion

        /// 给定一组收尾相连的线，判断每根线是否是首尾颠倒的，颠倒为true
        /// <summary>
        /// 给定一组收尾相连的线，判断每根线是否是首尾颠倒的，颠倒为true
        /// </summary>
        public static bool[] Check_HeadTail(WEntity2D[] Curves)
        {
            bool[] Out = new bool[Curves.Length];
            Out[0] = false;
            
            WCurve2D C = (WCurve2D)Curves[0];
            WPoint2D P0 = C.EndPoint;
            for (int i = 1; i < Out.Length; i++)
            {
                C = (WCurve2D)Curves[i];
                if (P0.DistanceTo(C.StartPoint) > P0.DistanceTo(C.EndPoint))
                {
                    Out[i] = true;
                    P0 = C.StartPoint;
                }
                else
                {
                    Out[i] = false;
                    P0 = C.EndPoint;
                }
            }
            return Out;
        }

        /// 得到一组线从头到尾的顺序
        /// <summary>
        /// 得到一组线从头到尾的顺序
        /// </summary>
        /// <param name="Curves"></param>
        /// <returns></returns>
        public static int[] Sort_Curves(WEntity2D[] Curves)
        {
            int Quan = Curves.Length;

            WPoint2D Pt;      /////上一条线的尾端点
            WPoint2D Ps, Pe;  /////扫描线的起点及终点

            List<int> At = new List<int>();   /////预先存储的顺序数组
            for (int i = 1; i < Quan; i++)
                At.Add(i);

            int[] Ao = new int[Quan];     /////输出用的标示曲线顺序的数组
            Ao[0] = 0;
            bool[] Co = new bool[Quan];   /////输出用的标示曲线起点开始或是终点开始的数组，True为首尾颠倒
            Co[0] = true;

            WCurve2D C = (WCurve2D)Curves[0];
            int Num;
            double l;
            Pt = C.EndPoint;

            for (int i = 1; i < Quan; i++)
            {

                for (int j = 0; j < At.Count; j++)
                {
                    Num = At[j];
                    C = (WCurve2D)Curves[Num];
                    Ps = C.StartPoint;
                    Pe = C.EndPoint;

                    l = Ps.DistanceTo(Pt);
                    if (l < WGeos2D_Paras.E_Merge)
                    {
                        Ao[i] = Num;
                        Co[i] = false;
                        Pt = Pe;
                        At.RemoveAt(j);
                        break;
                    }
                    l = Pe.DistanceTo(Pt);
                    if (l < WGeos2D_Paras.E_Merge)
                    {
                        Ao[i] = Num;
                        Co[i] = true;
                        Pt = Ps;
                        At.RemoveAt(j);
                        break;
                    }
                }
            }
            ////////////////
            for (int i = 0; i < Quan; i++)
            {
                Ao[i] *= 2;
                if (Co[i] == true)   /////如果首尾颠倒则输出一个奇数，否则输出一个偶数
                    Ao[i]++;
            }
            return Ao;
        }

        /// 在点集中删除重复节点
        /// <summary>
        /// 在点集中删除重复节点
        /// </summary>
        public static void Merge_Points(ref List<WPoint2D> Ps)
        {
            for (int i = 0; i < Ps.Count - 1; i++)
                for (int j = i + 1; j < Ps.Count; j++)
                    if (Math.Abs(Ps[i].X - Ps[j].X) < WGeos2D_Paras.E_Merge && Math.Abs(Ps[i].Y - Ps[j].Y) < WGeos2D_Paras.E_Merge)
                    {
                        Ps.RemoveAt(j);
                        j--;
                    }
        }

        /// 判断两点是否可以合并
        /// <summary>
        /// 判断两点是否可以合并
        /// </summary>
        public static bool Check_PsMerge(WPoint2D P1, WPoint2D P2)
        {
            if ((Math.Abs(P1.X - P2.X) < WGeos2D_Paras.E_Merge) && (Math.Abs(P1.Y - P2.Y) < WGeos2D_Paras.E_Merge))
                return true;
            return false;
        }

        /// 将一堆WEntity2D[]数组连接成为单个数组
        /// <summary>
        /// 将一堆WEntity2D[]数组连接成为单个数组,如果Free_Check为False不会检查是否是自由线
        /// </summary>
        public static WEntity2D[] Combine_CsArray(ref WEntity2D[][] Cs, bool Free_Check)
        {
            WEntity2D[] Cs_Out = new WCurve2D[0];
            for (int i = 0; i < Cs.Length; i++)
            {
                for (int j = 0; j < Cs[i].Length; j++)
                {
                    if (((WCurve2D)Cs[i][j]).Free_Check == false && Free_Check == true) continue;
                    Array.Resize<WEntity2D>(ref Cs_Out, Cs_Out.Length + 1);
                    Cs_Out[Cs_Out.Length - 1] = Cs[i][j];
                }
            }
            return Cs_Out;
        }

        public static void RemoveAt_InEntityArray(ref WEntity2D[] Es, int Index)
        {
            if (Index < 0) return;
            if (Index > Es.Length - 1) return;
            for (int i = Index; i < Es.Length - 1; i++)
                Es[i] = Es[i + 1];
            Array.Resize<WEntity2D>(ref Es, Es.Length - 1);
        }
    }
}
