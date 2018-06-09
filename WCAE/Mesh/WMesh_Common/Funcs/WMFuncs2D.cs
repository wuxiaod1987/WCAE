using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WCAE.Entities;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Funcs;
using WCAE.WGeos2D.Entities;
using WCAE.WMesh2D;

namespace WCAE.WMesh2D.Funcs
{
    /// <summary>
    /// 常用的Mesh功能函数放置在此静态类中
    /// </summary>
    public static class WMFuncs2D
    {
        #region "其他计算"
        /// <summary>
        /// 按照顺序排列点，并输出点的顺序
        /// </summary>
        /// <param name="Ps">待排序点集</param>
        /// <param name="XY_Check">true表示X排序，false表示Y排序</param>
        /// <param name="SB_Check">true表示从小到大，false表示从大到小</param>
        /// <returns></returns>
        public static int[] Ps_Sort(ref WPoint2D[] Ps, bool XY_Check, bool SB_Check)
        {
            int quan = Ps.Length;
            WPoint2D Pt;
            int Nt;
            int[] Ns = new int[quan];
            for (int i = 0; i < quan; i++)
                Ns[i] = i;
            for (int i = 0; i < quan - 1; i++)
            {
                for (int j = i + 1; j <= quan - 1; j++)
                {
                    if (XY_Check == true)
                    {
                        if (SB_Check == true)
                        {
                            if (Ps[i].X > Ps[j].X)
                            {
                                Pt = Ps[i];
                                Ps[i] = Ps[j];
                                Ps[j] = Pt;

                                Nt = Ns[i];
                                Ns[i] = Ns[j];
                                Ns[j] = Nt;
                            }
                        }
                        else
                        {
                            if (Ps[i].X < Ps[j].X)
                            {
                                Pt = Ps[i];
                                Ps[i] = Ps[j];
                                Ps[j] = Pt;

                                Nt = Ns[i];
                                Ns[i] = Ns[j];
                                Ns[j] = Nt;
                            }
                        }
                    }
                    else
                    {
                        if (SB_Check == true)
                        {
                            if (Ps[i].Y > Ps[j].Y)
                            {
                                Pt = Ps[i];
                                Ps[i] = Ps[j];
                                Ps[j] = Pt;

                                Nt = Ns[i];
                                Ns[i] = Ns[j];
                                Ns[j] = Nt;
                            }
                        }
                        else
                        {
                            if (Ps[i].Y < Ps[j].Y)
                            {
                                Pt = Ps[i];
                                Ps[i] = Ps[j];
                                Ps[j] = Pt;

                                Nt = Ns[i];
                                Ns[i] = Ns[j];
                                Ns[j] = Nt;

                            }
                        }
                    }
                }
            }
            return Ns;
        }
        /// <summary>
        ///  按照顺序排列点，并输出点的顺序
        /// </summary>
        /// <param name="Ps">待排序点集</param>
        /// <param name="XY_Check">true表示X排序，false表示Y排序</param>
        /// <param name="SB_Check">true表示从小到大，false表示从大到小</param>
        /// <returns></returns>
        public static int[] Ps_Sort(List<WPoint2D> Ps, bool XY_Check, bool SB_Check)
        {
            int quan = Ps.Count;
            WPoint2D Pt;
            int Nt;
            int[] Ns = new int[quan];
            for (int i = 0; i < quan; i++)
                Ns[i] = i;
            for (int i = 0; i < quan - 1; i++)
            {
                for (int j = i + 1; j <= quan - 1; j++)
                {
                    if (XY_Check == true)
                    {
                        if (SB_Check == true)
                        {
                            if (Ps[i].X > Ps[j].X)
                            {
                                Pt = Ps[i];
                                Ps[i] = Ps[j];
                                Ps[j] = Pt;

                                Nt = Ns[i];
                                Ns[i] = Ns[j];
                                Ns[j] = Nt;
                            }
                        }
                        else
                        {
                            if (Ps[i].X < Ps[j].X)
                            {
                                Pt = Ps[i];
                                Ps[i] = Ps[j];
                                Ps[j] = Pt;

                                Nt = Ns[i];
                                Ns[i] = Ns[j];
                                Ns[j] = Nt;
                            }
                        }
                    }
                    else
                    {
                        if (SB_Check == true)
                        {
                            if (Ps[i].Y > Ps[j].Y)
                            {
                                Pt = Ps[i];
                                Ps[i] = Ps[j];
                                Ps[j] = Pt;

                                Nt = Ns[i];
                                Ns[i] = Ns[j];
                                Ns[j] = Nt;
                            }
                        }
                        else
                        {
                            if (Ps[i].Y < Ps[j].Y)
                            {
                                Pt = Ps[i];
                                Ps[i] = Ps[j];
                                Ps[j] = Pt;

                                Nt = Ns[i];
                                Ns[i] = Ns[j];
                                Ns[j] = Nt;

                            }
                        }
                    }
                }
            }
            return Ns;
        }

        /// 计算两点之间的距离
        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns></returns>
        public static double Length_P2P(WPoint2D P1, WPoint2D P2)
        {
            return (Math.Sqrt((P1.X - P2.X) * (P1.X - P2.X) + (P1.Y - P2.Y) * (P1.Y - P2.Y)));
        }

        /// 判断两点是否是同一点
        /// <summary>
        /// 判断两点是否是同一点
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns></returns>
        public static bool Near_Check(WPoint2D P1, WPoint2D P2)
        {
            if ((Math.Abs(P1.X - P2.X) < WGeos2D_Paras.E_Merge) && (Math.Abs(P1.Y - P2.Y) < WGeos2D_Paras.E_Merge))
                return true;
            else
                return false;
        }

        /// 判断两点是否是同一点
        /// <summary>
        /// 判断两点是否是同一点
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns></returns>
        public static bool Near_Check(WPoint2D P1, WNode2D P2)
        {
            if ((Math.Abs(P1.X - P2.X) < WGeos2D_Paras.E_Merge) && (Math.Abs(P1.Y - P2.Y) < WGeos2D_Paras.E_Merge))
                return true;
            else
                return false;
        }
        
        ///// 找到Bnd集中与某点相连的线的编号，输出结果为编号乘以2，头点则再加0，尾点则再加1
        //public static int Find_Next_Bnd(WPoint2D P, ref List<Bnd> Bnds)
        //{
        //    List<WPoint2D> Ps = new List<WPoint2D>();
        //    for (int i = 0; i < Bnds.Count; i++)
        //    {
        //        Ps.Add(Bnds[i].Ps[0]);
        //        Ps.Add(Bnds[i].Ps[Bnds[i].Ps.Count - 1]);
        //    }
        //    int N_min = 0;
        //    double L_min = Length_P2P(P, Ps[0]);
        //    double Lt;
        //    for (int i = 0; i < Ps.Count; i++)
        //    {
        //        Lt = Length_P2P(P, Ps[i]);
        //        if (Lt < L_min)
        //        {
        //            N_min = i;
        //            L_min = Lt;
        //        }
        //    }
        //    return N_min;
        //}

        /// 将Ps翻转
        /// <summary>
        /// 将Ps翻转
        /// </summary>
        /// <param name="Ps"></param>
        /// <param name="Reverse_Check">true为翻转，false为不翻转</param>
        public static void Points_Reverse(ref  List<WPoint2D> Ps, bool Reverse_Check)
        {
            if (Reverse_Check == false)
                return;
            int T = (int)Math.Floor((double)(Ps.Count / 2));
            WPoint2D P;
            int Quan = Ps.Count;
            for (int i = 1; i <= T; i++)
            {
                P = Ps[i - 1];
                Ps[i - 1] = Ps[Quan - i];
                Ps[Quan - i] = P;
            }
        }

        /// 将Ns翻转
        /// <summary>
        /// 将Ns翻转
        /// </summary>
        /// <param name="Ns"></param>
        /// <param name="Reverse_Check">true为翻转，false为不翻转</param>
        public static void Nums_Reverse(ref List<int> Ns, bool Reverse_Check)
        {
            if (Reverse_Check == false)
                return;
            int T = (int)Math.Floor((double)(Ns.Count / 2));
            int N;
            int Quan = Ns.Count;
            for (int i = 1; i <= T; i++)
            {
                N = Ns[i - 1];
                Ns[i - 1] = Ns[Quan - i];
                Ns[Quan - i] = N;
            }
        }
        #endregion

        #region "单元、节点输出"
        /// 节点集输出至文件
        /// <summary>
        /// 节点集输出至文件
        /// </summary>
        /// <param name="Ps">节点集位置</param>
        /// <param name="Nums">节点集编号</param>
        /// <param name="Reverse_Check">是否倒序输出</param>
        /// <param name="Num_Pre">判断第一个节点是否输出的编号，相同则不输出</param>
        /// <param name="sw">输出对象</param>
        public static void Ns_Out(ref List<WPoint2D> Ps, ref List<int> Nums, bool Reverse_Check, int Num_Pre, ref StreamWriter sw)
        {
            int Quan = Ps.Count;
            if (Reverse_Check == false)
            {
                if (Nums[0] != Num_Pre)
                    WMFuncs2D.N_Out(Ps[0], Nums[0], ref sw);
                for (int i = 1; i < Quan; i++)
                    WMFuncs2D.N_Out(Ps[i], Nums[i], ref  sw);
            }
            else
            {
                if (Nums[Quan - 1] != Num_Pre)
                    WMFuncs2D.N_Out(Ps[Quan - 1], Nums[Quan - 1], ref sw);
                for (int i = Quan - 2; i >= 0; i--)
                    WMFuncs2D.N_Out(Ps[i], Nums[i], ref  sw);
            }
        }

        /// 节点集输出至文件，用在Glass上
        /// <summary>
        /// 节点集输出至文件，用在Glass上
        /// </summary>
        /// <param name="Ps">节点集位置</param>
        /// <param name="Nums">节点集编号</param>
        /// <param name="sw">输出对象</param>
        /// <param name="Mid_Check">判断中间的节点是否输出</param>
        /// <param name="HT_Check">判断两头的节点是否输出</param>
        /// <param name="Round">保留小数点数位</param>
        public static void Ns_Out(ref List<WPoint2D> Ps, ref List<int> Nums, ref StreamWriter sw, bool Mid_Check, bool HT_Check)
        {
            if (HT_Check == true)
                WMFuncs2D.N_Out(Ps[0], Nums[0], ref sw);
            if (Mid_Check == true)
                for (int i = 1; i < Ps.Count - 1; i++)
                    WMFuncs2D.N_Out(Ps[i], Nums[i], ref sw);
            if (HT_Check == true)
                WMFuncs2D.N_Out(Ps[Ps.Count - 1], Nums[Ps.Count - 1], ref sw);
        }

        /// 单节点输出
        /// <summary>
        /// 单节点输出
        /// </summary>
        /// <param name="P"></param>
        /// <param name="Num"></param>
        /// <param name="sw"></param>
        public static void N_Out(WPoint2D P, int Num, ref StreamWriter sw)
        {
            sw.WriteLine(Convert.ToString(Num) + "	" +
                         Convert.ToString(Math.Round(P.X, WGeos2D_Paras.Round)) + "," +
                         Convert.ToString(Math.Round(P.Y, WGeos2D_Paras.Round)) + "," +
                         Convert.ToString(P.CheckNum));
        }

        /// 单元集输出
        /// <summary>
        /// 单元集输出
        /// </summary>
        /// <param name="Es"></param>
        /// <param name="sw"></param>
        public static void Es_Out(ref WElement2D[] Es, ref StreamWriter sw)
        {
            int Quan = Es.Length;
            for (int i = 0; i < Quan; i++)
                sw.WriteLine(Convert.ToString(Es[i].Kind) + "	" +
                             Convert.ToString(Es[i].N1) + "," +
                             Convert.ToString(Es[i].N2) + "," +
                             Convert.ToString(Es[i].N3) + "," +
                             Convert.ToString(Es[i].N4));
        }

        /// 单元集合输出
        /// <summary>
        /// 单元集合输出
        /// </summary>
        /// <param name="Es"></param>
        /// <param name="sw"></param>
        /// <param name="Kind_M">Kind_M为True表示输出Kind为电单元，单元类型转为负数</param>
        public static void Es_Out(ref WElement2D[] Es, ref StreamWriter sw, bool Kind_M)
        {
            int Quan = Es.Length;
            for (int i = 0; i < Quan; i++)
                if (Kind_M == true)
                    sw.WriteLine(Convert.ToString(Es[i].Kind * (-1)) + "	" +
                             Convert.ToString(Es[i].N1) + "," +
                             Convert.ToString(Es[i].N2) + "," +
                             Convert.ToString(Es[i].N3) + "," +
                             Convert.ToString(Es[i].N4));
                else
                    sw.WriteLine(Convert.ToString(Es[i].Kind) + "	" +
                             Convert.ToString(Es[i].N1) + "," +
                             Convert.ToString(Es[i].N2) + "," +
                             Convert.ToString(Es[i].N3) + "," +
                             Convert.ToString(Es[i].N4));
        }
        #endregion

        #region "节点、单元处理（生成，输出）"
        #region "节点App"
        public static int NsID_App(ref List<WPoint2D> Ps, ref List<int> Nums, int Num_Start)
        {
            Nums = new List<int>();
            int Quan = Ps.Count;
            for (int i = 1; i <= Quan; i++)
                Nums.Add(Num_Start + i);
            return Num_Start + Quan;
 
        }

        public static int NsID_App(ref List<WPoint2D> Ps, ref List<int> Nums, int Num_Start, bool Reverse_Check)
        {
            Nums = new List<int>();
            int Quan = Ps.Count;
            if (Reverse_Check == false)
                for (int i = 1; i <= Quan; i++)
                    Nums.Add(Num_Start + i);
            else
                for (int i = Quan; i >= 1; i--)
                    Nums.Add(Num_Start + i);
            return Num_Start + Quan;
        }

        public static int NsID_App(ref List<WPoint2D> Ps, ref List<int> Nums, bool HT_Check, int Num_Start)
        {
            Nums = new List<int>();
            int Quan = Ps.Count;
            if (HT_Check == false)
            {
                Nums.Add(0);
                for (int i = 1; i < Quan - 1; i++)
                    Nums.Add(Num_Start + i);
                Nums.Add(0);
                return Num_Start + Quan - 2;
            }
            else
                return NsID_App(ref Ps, ref  Nums, Num_Start);
        }
        #endregion

        #region "单元App"
        public static int E4sID_App(ref List<int> Ns1, ref List<int> Ns2, ref WElement2D[] Es, int Num_Start)
        {
            int Quan = Ns1.Count - 1;
            Array.Resize<WElement2D>(ref Es, Quan);
            int Num;
            for (int i = 0; i < Quan; i++)
            {
                Num = Num_Start + i + 1;
                Es[i] = new WElement2D(4, Ns1[i], Ns2[i], Ns2[i + 1], Ns1[i + 1]);
            }
            return Num_Start + Quan - 1;
        }

        public static int E2sID_App(ref List<int> Ns, ref WElement2D[] Es, int Num_Start, bool Append_Check)
        {
            int Quan = Ns.Count - 1;
            int Quan_Pre, Num;
            if (Append_Check == true)
                Quan_Pre = Es.Length;
            else
                Quan_Pre = 0;
            Array.Resize<WElement2D>(ref Es, Quan + Quan_Pre);
            for (int i = 0; i < Quan; i++)
            {
                Num = Num_Start + i + 1;
                Es[i + Quan_Pre] = new WElement2D(2, Ns[i], Ns[i + 1], 0, 0);
            }
            return Num_Start + Quan - 1;
        }

        public static int E2ID_App(int N1, int N2, ref WElement2D[] Es, int Num_Start, bool Append_Check)
        {
            int Quan_Pre;
            if (Append_Check == true)
                Quan_Pre = Es.Length;
            else
                Quan_Pre = 0;
            Array.Resize<WElement2D>(ref Es, Quan_Pre + 1);
            Es[Quan_Pre] = new WElement2D(2, N1, N2, 0, 0);
            return Num_Start + 1;
        }
        #endregion
        #endregion

        #region Mesh线
        /// <summary>
        /// Mesh线集合
        /// </summary>
        /// <param name="Cs"></param>
        /// <param name="Mesh_Length"></param>
        public static void Mesh_Curves(ref WEntity2D[] Cs, double Mesh_Length)
        {
            WCurve2D C;
            int Times;
            for (int i = 0; i < Cs.Length; i++)
            {
                C = (WCurve2D)Cs[i];
                if (C.Meshed_Check == true)
                {
                    if (C.StartPoint.DistanceTo(C.Nodes[0]) > (C.StartPoint.DistanceTo(C.Nodes[C.Nodes.Count - 1])))
                        WMFuncs2D.Points_Reverse(ref C.Nodes, true);                       /////翻转点集
                        continue;
                }
                Times = (int)Math.Ceiling(C.Length / Mesh_Length);
                C.Nodes = Geos2D_Modify.DotCurve_Times(Cs[i], Times);
            }
        }

        /// <summary>
        /// Mesh单条线
        /// </summary>
        /// <param name="Cin"></param>
        /// <param name="Mesh_Length"></param>
        public static void Mesh_Curve(WEntity2D Cin, double Mesh_Length)
        {
            WCurve2D C = (WCurve2D)Cin;
            int Times;
            if (C.Meshed_Check == true)
            {
                if (C.StartPoint.DistanceTo(C.Nodes[0]) > (C.StartPoint.DistanceTo(C.Nodes[C.Nodes.Count - 1])))
                    WMFuncs2D.Points_Reverse(ref C.Nodes, true);                       /////翻转点集
                return;
            }
            Times = (int)Math.Ceiling(C.Length / Mesh_Length);
            C.Nodes = Geos2D_Modify.DotCurve_Times(C, Times);
        }
        #endregion
    }
}
