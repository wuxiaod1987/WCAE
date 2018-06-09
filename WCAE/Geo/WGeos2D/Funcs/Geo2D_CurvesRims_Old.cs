using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;

namespace WCAE.WGeos2D.Funcs
{
    /// <summary>
    /// 已经过期不用
    /// </summary>
    static class Geo2D_CurvesRims_Old
    {
        public static List<int>[] Get_Rims(ref WEntity2D[] Cs, ref WGeometry2D WGC)
        {
            List<int>[] Ns = new List<int>[0];   /////每个节点相连的节点的编号
            List<int>[] Rims = new List<int>[0]; /////用于输出的所有的圈圈
            int[][] Ls = new int[Cs.Length][];   /////每条线的头尾节点编号
            double[] Xs = new double[0];         /////每个节点的X坐标
            double[] Ys = new double[0];         /////每个节点的Y坐标
            /////
            P2Ps_Connection(ref Cs, ref WGC,     /////求出点点之间的关系
                            ref Xs, ref Ys, 
                            ref Ns, ref Ls);
            ///从自由节点开始层层剖分
            List<int> Rimt;       /////计算时使用的单个Rim，若该Rim不正确则不会被输出
            int Npo = 0;          /////某一次的扫描起点节点编号
            int Check_Num = -1;   /////不能够形成正确Rim的节点的编号
            /////
            while (true)
            {
                Npo = -1;
                for (int i = 0; i < Ns.Length; i++)
                    if (Ns[i].Count == 2 && i > Check_Num)
                    {
                        Npo = i;
                        break;
                    }
                if (Npo == -1) break;                           /////如果找不到有连线的自由节点则不要继续了
                /////
                Rimt = Create_SingleRim(Npo, ref Ns);           /////创建一个Rim
                /////
                if (Check_RimtValid(ref Rimt, ref Ns) == false) /////判断Rimt是否可用
                {
                    Check_Num = Npo;                            /////如果这个圈圈是无效的，则在开始搜索时把这个节点排除
                    continue;                                   /////从这点的下一编号点开始
                }
                Check_Num = -1;
                /////
                Update_Ns(ref Rimt, ref Ns, ref Rims);          /////将已经Check后OK的Rimt输入，并更新Ns数组
                                                                /////此时的Rims是节点集合，而不是线的集合
            }
            /////
            Convert_Rims2Ls(ref Rims, ref Ls);                  /////将Rims由点点连接转换为线线连接
            /////
            //Output(ref sw, ref Xs, ref Ys, ref Ns, ref Rims, ref Ls);   /////输出
            return Rims;
        }

        #region 过程函数
        private static void Output(ref StreamWriter sw, ref double[] Xs, ref double[] Ys, ref List<int>[] Ns, ref List<int>[] Rims, ref int[][] Ls)
        {
            sw.WriteLine("输出点坐标");
            for (int i = 0; i < Xs.Length; i++)
            {
                sw.WriteLine(Convert.ToString(i));
                sw.WriteLine(Convert.ToString(Math.Round(Xs[i], 2)) + "," + Convert.ToString(Math.Round(Ys[i], 2)));
            }
            sw.WriteLine("##############");

            sw.WriteLine("线的头尾点");
            for (int i = 0; i < Ls.Length; i++)
            {
                sw.WriteLine(Convert.ToString(i));
                sw.WriteLine(Convert.ToString(Ls[i][0]) + "," + Convert.ToString(Ls[i][1]));
            }
            sw.WriteLine("##############");

            sw.WriteLine("输出相邻点");
            for (int i = 0; i < Ns.Length; i++)
            {
                sw.WriteLine(Convert.ToString(i));
                for (int j = 0; j < Ns[i].Count; j++)
                    sw.WriteLine(Ns[i][j]);
                sw.WriteLine("@");
            }
            sw.WriteLine("##############");
            for (int i = 0; i < Rims.Length; i++)
            {
                sw.WriteLine(Convert.ToString(i));
                sw.WriteLine("@");
                for (int j = 0; j < Rims[i].Count; j++)
                    sw.WriteLine(Convert.ToString(Rims[i][j]));
                sw.WriteLine("@@@@@@");
            }
        }

        /// 将Rims由点点连接转换为线线连接
        private static void Convert_Rims2Ls(ref List<int>[] Rims, ref int[][] Ls)
        {
            List<int> Rimt;
            for (int i = 0; i < Rims.Length; i++)
            {
                Rimt = new List<int>();
                for (int j = 0; j < Rims[i].Count; j++)
                    Rimt.Add(Rims[i][j]);
                Rimt.Add(Rims[i][0]);
                /////
                for (int j = 0; j < Rimt.Count - 1; j++)
                    Rims[i][j] = Find_Line(Rimt[j], Rimt[j + 1], ref Ls);
            }
            Rimt = null;
        }

        /// 根据两个节点的编号找出对应的线的编号，没有则返回-1
        private static int Find_Line(int P0, int P1, ref int[][] Ls)
        {
            for (int k = 0; k < Ls.Length; k++)
                if ((P0 == Ls[k][0] && P1 == Ls[k][1]) || (P0 == Ls[k][1] && P1 == Ls[k][0]))
                    return k;
            return -1; 
        }

        /// 创建一个Rim
        private static List<int> Create_SingleRim(int Npo, ref List<int>[] Ns)
        {
            List<int>  Rimt = new List<int>();   /////初始化一下Rimt
            int Np = 0;                          /////扫描点
            int Nn = 0;                          /////扫描点的下一点
            int Np_pre = 0;                      /////扫描点的前一点
            /////形成单个Rim
            /////往前搜索
            Np = Npo;
            Nn = Ns[Np][0];
            Rimt.Add(Np);
            while (true)
            {
                Np_pre = Np;
                Np = Nn;
                Rimt.Add(Np);

                Nn = Next_Point(Np, Np_pre, ref Ns);
                if (Nn == -1) break;
                if (Np == Npo) break;
            }
            /////如果第一轮搜索就转了一圈，说明所有点都是独立点，则继续
            if (Rimt[0] == Rimt[Rimt.Count - 1])
                return Rimt;
            /////往后搜索
            Np = Npo;
            Nn = Ns[Np][1];
            while (true)
            {
                Np_pre = Np;
                Np = Nn;
                Rimt.Insert(0, Np);

                Nn = Next_Point(Np, Np_pre, ref Ns);
                if (Nn == -1) break;
            }
            return Rimt;
        }

        /// 判断Rimt是否可用
        private static bool Check_RimtValid(ref List<int> Rimt, ref List<int>[] Ns)
        {
            bool Check = false;
            if (Rimt[0] == Rimt[Rimt.Count - 1])                         /////Rimt头尾点相同情况
            {
                for (int i = 0; i < Ns[Rimt[Rimt.Count - 1]].Count; i++) /////判断最后点的所有相连点中是否有第二点
                    if (Ns[Rimt[Rimt.Count - 1]][i] == Rimt[1])
                    {
                        Check = true;
                        break;
                    }
            }
            else                                                          /////Rimt头尾点不相同情况
            {
                for (int i = 0; i < Ns[Rimt[Rimt.Count - 1]].Count; i++)  /////判断最后点的所有相连点中是否有第一点
                    if (Ns[Rimt[Rimt.Count - 1]][i] == Rimt[0])
                    {
                        Check = true;
                        break;
                    }
            }
            return Check;
        }

        /// 将已经Check后OK的Rimt输入，并更新Ns数组
        private static void Update_Ns(ref List<int> Rimt, ref List<int>[] Ns, ref List<int>[] Rim)
        {
            int Nt;
            /////开始删除
            Nt = Rimt[0];
            for (int i = 0; i < Ns[Nt].Count; i++)
                if (Ns[Nt][i] == Rimt[1])
                    Ns[Nt].RemoveAt(i);

            Nt = Rimt[Rimt.Count - 1];
            for (int i = 0; i < Ns[Nt].Count; i++)
                if (Ns[Nt][i] == Rimt[Rimt.Count - 2])
                    Ns[Nt].RemoveAt(i);

            for (int i = 1; i < Rimt.Count - 1; i++)
                Ns[Rimt[i]].Clear();

            if (Rimt[0] == Rimt[Rimt.Count - 1])
                Rimt.RemoveAt(0);
            /////加进新的Rim
            Array.Resize<List<int>>(ref Rim, Rim.Length + 1);
            Rim[Rim.Length - 1] = Rimt;
        }

        /// 找到下一点
        private static int Next_Point(int Np, int Np_pre, ref List<int>[] Ns)
        {
            if (Ns[Np].Count != 2) return -1;
            if (Ns[Np][0] == Np_pre)
                return Ns[Np][1];
            else
                return Ns[Np][0];
        }

        /// 求出点点之间的关系
        private static void P2Ps_Connection(ref WEntity2D[] Cs, ref WGeometry2D WGC,
                                            ref double[] Xs, ref double[] Ys, 
                                            ref List<int>[] Ns, ref int[][] Ps)
        {
            bool c;       ////判断某个节点是否可以输入，默认为true，重复则输入false
            WPoint2D[] Pt = new WPoint2D[2];
            double x, y;
            int Q;
            //List<WVector2D>[] Vs;   /////每个节点连接对应节点时的线的角度
            ///
            for (int i = 0; i < Cs.Length; i++)
            {
                Ps[i] = new int[2];
                Pt[0] = ((WCurve2D)Cs[i]).StartPoint;    /////处理起点
                Pt[1] = ((WCurve2D)Cs[i]).EndPoint;      /////处理起点

                for (int j = 0; j < 2; j++)
                {
                    c = false;
                    x = Pt[j].X; y = Pt[j].Y;
                    for (int k = 0; k < Xs.Length; k++)
                    {
                        if (Math.Abs(x - Xs[k]) < WGeos2D_Paras.E_Merge && Math.Abs(y - Ys[k]) < WGeos2D_Paras.E_Merge)
                        {
                            Ps[i][j] = k;
                            Ns[k].Add(i); /////如果该点和已知点距离在误差范围内，在已有节点的连接单元编号后面加上该线的编号
                                          /////此时Ns表示每个节点相连的线的编号
                            c = true;
                            break;
                        }
                    }
                    if (c == false)
                    {
                        Q = Ns.Length + 1;
                        Array.Resize<List<int>>(ref Ns, Q);
                        Array.Resize<double>(ref Xs, Q);
                        Array.Resize<double>(ref Ys, Q);
                        Xs[Q - 1] = x;
                        Ys[Q - 1] = y;
                        Ns[Q - 1] = new List<int>();
                        Ns[Q - 1].Add(i);
                        Ps[i][j] = Q - 1;
                    }
                }
            }
            //////
            int Nl;
            //Vs = new List<WVector2D>[Ns.Length];
            /////此时将Ns转换为每个节点相连的节点的编号
            for (int i = 0; i < Ns.Length; i++)
            {
                //Vs[i] = new List<WVector2D>();
                for (int j = 0; j < Ns[i].Count; j++)
                {
                    Nl = Ns[i][j];
                    if (Ps[Nl][0] == i)
                    {
                        Ns[i][j] = Ps[Nl][1];
                        //Vs[i].Add(((WCurve2D)Cs[Nl]).Direction_Head);
                    }
                    else
                    {
                        Ns[i][j] = Ps[Nl][0];
                        //Vs[i].Add(((WCurve2D)Cs[Nl]).Direction_Tail);
                    }
                }
            }
        }
        #endregion
    }
}
