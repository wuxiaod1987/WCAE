using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCAE.WGeos2D.Entities;

namespace WCAE.WGeos2D.Funcs
{
    public static class Geos2D_Intersection
    {
        #region 单线，单线之间相交
        public static List<WPoint2D> Intersection_L2C(WLine2D L, WEntity2D C)
        {
            List<WPoint2D> Ps = new List<WPoint2D>();
            WPoint2D P;
            if (C.Kind == GeoKind.Line)
            {
                P = Intersection_L2L((WLine2D)C, L);
                if (P != null) Ps.Add((WPoint2D)P);
                return Ps;
            }
            if (C.Kind == GeoKind.PolyLine)
                return Intersection_L2Pl((WPolyLine2D)C, L);
            return Ps;
        }

        public static WPoint2D Intersection_L2L(WPoint2D P11, WPoint2D P12, WPoint2D P21, WPoint2D P22)
        {
            ///判断头尾点是否相连
            if (Geos2D_Other.Check_PsMerge(P11, P21) == true || Geos2D_Other.Check_PsMerge(P11, P22) == true)
                return P11;
            if (Geos2D_Other.Check_PsMerge(P12, P21) == true || Geos2D_Other.Check_PsMerge(P12, P22) == true)
                return P12;
            ///求交点
            WVector2D d1 = P11.VectorTo(P12);
            WVector2D d2 = P21.VectorTo(P22);

            if (d1.IsParallelTo(d2, WGeos2D_Paras.E_Angle))
                return null;
            double t = (P21 - P11).CrossProduct(d2) / (d1.CrossProduct(d2));
            if (t < 0 || t > 1)
                return null;
            WPoint2D P = P11 + t * d1;
            if (CheckBound_Single(P21.X, P22.X, P.X) == false || CheckBound_Single(P21.Y, P22.Y, P.Y) == false)
                return null;
            return P;
        }
        public static WPoint2D Intersection_L2L(WLine2D L1, WPoint2D P1, WPoint2D P2)
        {
            return Intersection_L2L(L1.StartPoint, L1.EndPoint, P1, P2);
        }
        public static WPoint2D Intersection_L2L(WLine2D L1, WLine2D L2)
        {
            return Intersection_L2L(L1.StartPoint, L1.EndPoint, L2.StartPoint, L2.EndPoint);
        }

        public static List<WPoint2D> Intersection_L2Pl(WPolyLine2D PL, WLine2D L)
        {
            List<WPoint2D>[] Pst = new List<WPoint2D>[0];
            ///Check头尾
            if (Geos2D_Other.Check_PsMerge(PL.StartPoint, L.StartPoint) == true || 
                Geos2D_Other.Check_PsMerge(PL.StartPoint, L.EndPoint) == true)
            {
                Array.Resize<List<WPoint2D>>(ref Pst, Pst.Length + 1);
                Pst[Pst.Length - 1] = new List<WPoint2D>();
                Pst[Pst.Length - 1].Add(PL.StartPoint);
            }
            if (Geos2D_Other.Check_PsMerge(PL.EndPoint, L.StartPoint) == true || 
                Geos2D_Other.Check_PsMerge(PL.EndPoint, L.EndPoint) == true)
            {
                Array.Resize<List<WPoint2D>>(ref Pst, Pst.Length + 1);
                Pst[Pst.Length - 1] = new List<WPoint2D>();
                Pst[Pst.Length - 1].Add(PL.EndPoint);
            }
            /////判断是否相互顶在一起
            //WPoint2D? Pnt = Geos2D_Other.ClosestPoint_P2PL(L.StartPoint, PL, true);
            //if (Pnt != null && Geos2D_Other.Check_PsMerge((WPoint2D)Pnt, L.StartPoint) == true)
            //{
            //    Array.Resize<List<WPoint2D>>(ref Pst, Pst.Length + 1);
            //    Pst[Pst.Length - 1] = new List<WPoint2D>();
            //    Pst[Pst.Length - 1].Add(L.StartPoint);
            //}
            //Pnt = Geos2D_Other.ClosestPoint_P2PL(L.EndPoint, PL, true);
            //if (Pnt != null && Geos2D_Other.Check_PsMerge((WPoint2D)Pnt, L.EndPoint) == true)
            //{
            //    Array.Resize<List<WPoint2D>>(ref Pst, Pst.Length + 1);
            //    Pst[Pst.Length - 1] = new List<WPoint2D>();
            //    Pst[Pst.Length - 1].Add(L.EndPoint);
            //}
            //Pnt = Geos2D_Other.ClosestPoint_P2L(PL.StartPoint, L, true);
            //if (Pnt != null && Geos2D_Other.Check_PsMerge((WPoint2D)Pnt, PL.StartPoint) == true)
            //{
            //    Array.Resize<List<WPoint2D>>(ref Pst, Pst.Length + 1);
            //    Pst[Pst.Length - 1] = new List<WPoint2D>();
            //    Pst[Pst.Length - 1].Add(PL.StartPoint);
            //}
            //Pnt = Geos2D_Other.ClosestPoint_P2L(PL.EndPoint, L, true);
            //if (Pnt != null && Geos2D_Other.Check_PsMerge((WPoint2D)Pnt, PL.EndPoint) == true)
            //{
            //    Array.Resize<List<WPoint2D>>(ref Pst, Pst.Length + 1);
            //    Pst[Pst.Length - 1] = new List<WPoint2D>();
            //    Pst[Pst.Length - 1].Add(PL.EndPoint);
            //}
            ///
            bool Array_Start = true;     /////判断Array是否要增加新数，如为true则增加新数
            bool List_Start = true;      /////判断是否List要重新开始，如果为true则从新开始
            for (int i = 0; i < PL.Count-1; i++)
            {
                if (CheckBound_Double(L, PL[i], PL[i + 1]) == true)
                {
                    if (Array_Start == true)
                    {
                        Array.Resize<List<WPoint2D>>(ref Pst, Pst.Length + 1);
                        Pst[Pst.Length - 1] = new List<WPoint2D>();
                        List_Start = true;
                        Array_Start = false;
                    }
                    if (List_Start == true)
                        Pst[Pst.Length - 1].Add(PL[i]);
                    Pst[Pst.Length - 1].Add(PL[i + 1]);
                }
                else
                    Array_Start = true;
            }
            //////////
            List<WPoint2D> Ps = new List<WPoint2D>();
            WPoint2D P;
            for (int i = 0; i < Pst.Length; i++)
            {
                for (int j = 0; j < Pst[i].Count - 1; j++)
                {
                    P = Intersection_L2L(L, Pst[i][j], Pst[i][j + 1]);
                    if (P == null) continue;
                    Ps.Add((WPoint2D)P);
                    break;
                }
            }
            Pst = null;
            return Ps;
        }
        #endregion

        #region 单线到多线的相交
        private static List<WPoint2D> Intersection_Cv2Cvs(WEntity2D C, ref WEntity2D[] Cs)
        {
            if (C.Kind == GeoKind.Line)
                return Intersection_L2Cvs((WLine2D)C, ref Cs);
            return new List<WPoint2D>();
        }

        /// <summary>
        /// PolyLine和Line数组之间的交点
        /// </summary>
        public static List<WPoint2D> Intersection_Cv2Ls(WEntity2D C, ref WEntity2D[] Ls)
        {
            List<WPoint2D> Ps = new List<WPoint2D>();
            List<WPoint2D> Pts = new List<WPoint2D>();
            for (int i = 0; i < Ls.Length; i++)
            {
                Pts = Intersection_L2C((WLine2D)Ls[i], C);
                if (Pts == null || Pts.Count == 0) continue;
                for (int j = 0; j < Pts.Count; j++)
                    Ps.Add(Pts[j]); 
            }
                return Ps;
        }

        /// <summary>
        /// 一条直线与许多线相交
        /// </summary>
        /// <param name="L"></param>
        /// <param name="Cs"></param>
        /// <returns></returns>
        public static List<WPoint2D> Intersection_L2Cvs(WLine2D L, ref WEntity2D[] Cs)
        {
            ///线对线的临时变量
            List<WPoint2D> Ps = new List<WPoint2D>();
            ///线对Poly的临时变量
            List<WPoint2D> Pst;
            WPoint2D P;
            for (int i = 0; i < Cs.Length; i++)
            {
                if (Cs[i].Kind == GeoKind.Line)        /////直线直线相交
                {
                    P = Intersection_L2L(L, (WLine2D)Cs[i]);
                    if (P == null) continue;
                    Ps.Add(new WPoint2D(((WPoint2D)P).X, ((WPoint2D)P).Y, 0));  /////((WPoint2D)P).CheckNum + i));
                }
                if (Cs[i].Kind == GeoKind.PolyLine)    /////直线多段线相交
                {
                    Pst = Intersection_L2Pl((WPolyLine2D)Cs[i], L);
                    if (Pst.Count == 0) continue;
                    for (int j = 0; j < Pst.Count; j++)
                        Ps.Add(new WPoint2D(((WPoint2D)Pst[j]).X, ((WPoint2D)Pst[j]).Y, 0)); /////((WPoint2D)Pst[j]).CheckNum + i));
                }
            }
            P = null;
            Pst = null;
            return Ps;
        }
        #endregion

        #region 判断数，点与直线之间的关系
        /// <summary>
        /// 判断一个数是否在两个数之间
        /// </summary>
        private static bool CheckBound_Single(double a1, double a2, double b)
        {
            if (b >= a1 && b <= a2)
                return true;
            if (b >= a2 && b <= a1)
                return true;
            if (Math.Abs(b - a1) < WGeos2D_Paras.E_Merge)
                return true;
            if (Math.Abs(b - a2) < WGeos2D_Paras.E_Merge)
                return true;
            return false;
        }

        private static bool CheckBound_Single(WLine2D L, double x)
        {
            if (x <= L.StartPoint.X && x >= L.EndPoint.X)
                return true;
            if (x >= L.StartPoint.X && x <= L.EndPoint.X)
                return true;
            return false;
        }

        private static bool CheckBound_Single(double y, WLine2D L)
        {
            if (y <= L.StartPoint.Y && y >= L.EndPoint.Y)
                return true;
            if (y >= L.StartPoint.Y && y <= L.EndPoint.Y)
                return true;
            return false;
        }

        private static bool CheckBound_Single(WLine2D L, WPoint2D P)
        {
            if (P.X < L.StartPoint.X && P.X < L.EndPoint.X)
                return false;
            if (P.X > L.StartPoint.X && P.X > L.EndPoint.X)
                return false;
            if (P.Y < L.StartPoint.Y && P.Y < L.EndPoint.Y)
                return false;
            if (P.Y > L.StartPoint.Y && P.Y > L.EndPoint.Y)
                return false;
            return true;
        }

        private static bool CheckBound_Double(WLine2D L, WPoint2D P1, WPoint2D P2)
        {
            if (P1.X > L.StartPoint.X && P1.X > L.EndPoint.X && P2.X > L.StartPoint.X && P2.X > L.EndPoint.X)
                return false;
            if (P1.X < L.StartPoint.X && P1.X < L.EndPoint.X && P2.X < L.StartPoint.X && P2.X < L.EndPoint.X)
                return false;
            if (P1.Y > L.StartPoint.Y && P1.Y > L.EndPoint.Y && P2.Y > L.StartPoint.Y && P2.Y > L.EndPoint.Y)
                return false;
            if (P1.Y < L.StartPoint.Y && P1.Y < L.EndPoint.Y && P2.Y < L.StartPoint.Y && P2.Y < L.EndPoint.Y)
                return false;
            return true;
        }
        #endregion
    }
}

#region 备份
//public static WPoint2D? Intersection_L2L2(WLine2D L1, WLine2D L2)
//{
//    return Intersection_L2L3(L1.StartPoint, L1.EndPoint, L2.StartPoint, L2.EndPoint);
//}
//public static WPoint2D? Intersection_L2L3(WPoint2D P11, WPoint2D P12, WPoint2D P21, WPoint2D P22)
//{
//    ///判断头尾点是否相连
//    if (Geos2D_Other.Check_PsMerge(P11, P21) == true || Geos2D_Other.Check_PsMerge(P11, P22) == true)
//        return P11;
//    if (Geos2D_Other.Check_PsMerge(P12, P21) == true || Geos2D_Other.Check_PsMerge(P12, P22) == true)
//        return P12;
//    ///求交点
//    WVector2D d1 = P11.VectorTo(P12);
//    WVector2D d2 = P21.VectorTo(P22);

//    if (d1.IsParallelTo(d2, WGeos2D_Paras.E_Angle))
//        return null;
//    double t = (P21 - P11).CrossProduct(d2) / (d1.CrossProduct(d2));
//    if (t < 0 || t > 1)
//        return null;
//    WPoint2D P = P11 + t * d1;
//    if (CheckBound_Single(P21.X, P22.X, P.X) == false && CheckBound_Single(P21.Y, P22.Y, P.Y) == false)
//        return null;
//    return P;
//}

//public WPoint2D? Intersection_L2L(WPoint2D P11, WPoint2D P12, WPoint2D P21, WPoint2D P22)
//{
//    double x, y;
//    double a1, a2, b1, b2;
//    if (Math.Abs(P11.X - P12.X) < WGC.E_Comp && Math.Abs(P21.X - P22.X) < WGC.E_Comp)
//        return null;
//    if (Math.Abs(P11.X - P12.X) < WGC.E_Comp)
//    {
//        x = (P11.X + P12.X) / 2;
//        b2 = (P21.Y - P22.Y) / (P21.X - P22.X);
//        a2 = P21.Y - b2 * P21.X;
//        y = b2 * x + a2;
//        if (CheckBound_Single(P11.Y, P12.Y, y) == true && CheckBound_Single(P21.X, P22.X, x) == true && CheckBound_Single(P21.Y, P22.Y, y) == true)
//            return new WPoint2D(x, y);
//        else
//            return null;
//    }
//    if (Math.Abs(P21.X - P22.X) < WGC.E_Comp)
//    {
//        x = (P21.X + P22.X) / 2;
//        b1 = (P11.Y - P12.Y) / (P11.X - P12.X);
//        a1 = P11.Y - b1 * P11.X;
//        y = b1 * x + a1;
//        if (CheckBound_Single(P21.Y, P22.Y,y) == true &&
//            CheckBound_Single(P11.X, P12.X, x) == true && CheckBound_Single(P11.Y, P12.Y, y) == true)
//            return new WPoint2D(x, y);
//        else
//            return null;
//    }
//    b1 = (P11.Y - P12.Y) / (P11.X - P12.X);
//    a1 = P11.Y - b1 * P11.X;
//    b2 = (P21.Y - P22.Y) / (P21.X - P22.X);
//    a2 = P21.Y - b2 * P21.X;
//    if ((b2 - b1) < WGC.E_Comp)
//        return null;
//    x = (a1 - a2) / (b2 - b1);
//    y = a1 + b1 * x;
//    if (CheckBound_Single(P11.X, P12.X, x) == true && CheckBound_Single(P11.Y, P12.Y, y) == true &&
//        CheckBound_Single(P21.X, P22.X, x) == true && CheckBound_Single(P21.Y, P22.Y, y) == true)
//        return new WPoint2D(x, y);
//    return null;

//}
#endregion