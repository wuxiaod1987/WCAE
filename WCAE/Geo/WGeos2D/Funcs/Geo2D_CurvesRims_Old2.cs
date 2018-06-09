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
    /// 由三个点连接的内夹角
    /// </summary>

    static class Geo2D_CurvesRims_Old2
    {
        private static List<int>[] Get_Rims(ref WEntity2D[] Cs, ref WGeometry2D WGC)
        {
            List<int>[] Rims = new List<int>[0];                     /////用于输出的所有的圈圈
            List<int>[] Ns = new List<int>[0];                       /////每个节点相连的节点的编号
            int[][] Ls = new int[Cs.Length][];                       /////每条线的头尾节点编号
            List<WVector2D>[] Vs = new List<WVector2D>[0];           /////每个节点连接对应节点时的线的向量
            List<PAngle>[] As = new List<PAngle>[0];                 /////每个节点与对应节点连接的内夹角，连接几个点就有几个内夹角
            /////
            P2Ps_Connection(ref Cs, ref WGC, ref Ls, ref Ns, ref Vs);/////求出点点之间的关系
            P2Ps_Angles(ref Ns, ref Vs, ref As);                     /////求出每个点的内夹角
            ///从自由节点开始层层剖分
            List<int> Rimt;       /////计算时使用的单个Rim，若该Rim不正确则不会被输出
            int Npo = 0;          /////某一次的扫描起点节点编号
            int Check_Num = -1;   /////不能够形成正确Rim的节点的编号
            /////
            int Qrim = 0;
            while(true)    /////for (int k = 0; k < 30;k++ )
            {
                Qrim = Rims.Length;
                Npo = -1;
                for (int i = 0; i < Ns.Length; i++)
                    if (As[i].Count > 0 && i > Check_Num)
                    {
                        Npo = i;
                        break;
                    }
                if (Npo == -1) break;                           /////如果找不到有连线的节点则不要继续了
                /////
                Rimt = Create_SingleRim(Npo, ref Ns, ref As);   /////创建一个Rim
                if (Check_RimtValid(ref Rimt, ref Ns) == false) /////判断Rimt是否可用
                {
                    Check_Num = Npo;                            /////如果这个圈圈是无效的，则在开始搜索时把这个节点排除
                    continue;                                   /////从这点的下一编号点开始
                }
                Check_Num = -1;
                /////
                Update_As(ref Rimt, ref Rims, ref As);          /////将已经Check后OK的Rimt输入，并更新Ns数组
                if (Rims.Length == Qrim) break;                 /////此时的Rims是节点集合，而不是线的集合
            }
            /////
            Convert_Rims2Ls(ref Rims, ref Ls);                  /////将Rims由点点连接转换为线线连接
            Delete_Repeat(ref Rims);
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
                sw.WriteLine("_______");
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

        #region 更新点点关系
        /// 将已经Check后OK的Rimt输入，并更新Ns数组
        private static void Update_As(ref List<int> Rimt, ref List<int>[] Rims, ref List<PAngle>[] As)
        {
            /////开始删除
            int A, B, O;
            for (int i = 0; i < Rimt.Count; i++)
            {
                O = Rimt[i];
                if (i == 0)
                    A = Rimt[Rimt.Count - 1];
                else
                    A = Rimt[i - 1];
                ///
                if (i == Rimt.Count - 1)
                    B = Rimt[0];
                else
                    B = Rimt[i + 1];
                for (int j = 0; j < As[O].Count; j++)
                {
                    if (As[O][j].B == B && As[O][j].A == A)
                    {
                        As[O].RemoveAt(j);
                        break;
                    }
                    if (As[O][j].B == A && As[O][j].A == B)
                    {
                        As[O].RemoveAt(j);
                        break;
                    }
                }
            }
            /////加进新的Rim
            Array.Resize<List<int>>(ref Rims, Rims.Length + 1);
            Rims[Rims.Length - 1] = Rimt;
        }
        #endregion

        #region 创建Single Rim
        /// 创建一个Rim
        private static List<int> Create_SingleRim(int Npo, ref List<int>[] Ns, ref List<PAngle>[] As)
        {
            List<int>  Rimt = new List<int>();   /////初始化一下Rimt
            int Npos = 0;                        /////扫描点
            int Nnext = 0;                       /////扫描点的下一点
            int Npre = 0;                        /////扫描点的前一点
            /////形成单个Rim
            /////往前搜索
            Npos = Npo;
            Nnext = As[Npos][0].B;
            Rimt.Add(Npos);
            while (true)
            {
                Npre = Npos;
                Npos = Nnext;
                Rimt.Add(Npos);

                Nnext = Next_Point(Npos, Npre, ref Ns, ref As);
                if (Nnext == -1 || Nnext == Npo) 
                    break;
            }
            return Rimt;
        }

        /// 使用已经算出的节点内夹角找到下一点
        private static int Next_Point(int Np, int Npre, ref List<int>[] Ns, ref List<PAngle>[] As)
        {
            if (As[Np].Count < 1) return -1;
            if (Ns[Np].Count == 2)
            {
                if (Ns[Np][0] == Npre)
                    return Ns[Np][1];
                else
                    return Ns[Np][0];
            }
            /////找到所有剩下的相邻线，找到角度
            List<int> Nsn = new List<int>();
            List<double> Asn = new List<double>();
            for (int i = 0; i < As[Np].Count; i++)
            {
                if (As[Np][i].A == Npre)
                {
                    Nsn.Add(As[Np][i].B);
                    Asn.Add(As[Np][i].Angle);
                    continue;
                }
                if (As[Np][i].B == Npre)
                {
                    Nsn.Add(As[Np][i].A);
                    Asn.Add(2 * Math.PI - As[Np][i].Angle);
                    continue;
                }
            }
            /////
            double Angle_Min = double.MaxValue;
            int Nnext = 0;
            for (int i = 0; i < Nsn.Count; i++)
            {
                if (Asn[i] < Angle_Min)
                {
                    Angle_Min = Asn[i];
                    Nnext = Nsn[i];
                }
            }
            Asn = null;
            Nsn = null;
            return Nnext;
        }

        /// 使用向量角度值计算下一点
        private static int Next_Point1(int Np, int Npre, ref List<int>[] Ns, ref List<WVector2D>[] Vs)
        {
            if (Ns[Np].Count < 2) return -1;
            if (Ns[Np].Count == 2)
            {
                if (Ns[Np][0] == Npre)
                    return Ns[Np][1];
                else
                    return Ns[Np][0];
            }
            /////////////////////////
            int Ppre=0;
            for (int i = 0; i < Ns[Np].Count; i++)
                if (Ns[Np][i] == Npre) Ppre = i;

            double Angle_Min = double.MaxValue;
            double Angle_t;
            int Nnext = 0;

            for (int i = 0; i < Ns[Np].Count; i++)
            {
                if (i == Ppre) continue;
                Angle_t = Vs[Np][Ppre].AngleAntiCrossWiseTo(Vs[Np][i]);
                if ( Angle_t < Angle_Min)
                {
                    Angle_Min = Angle_t;
                    Nnext = Ns[Np][i];
                }
            }
            return Nnext;
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
        #endregion

        #region 线线关系，点点关系的处理
        /// 求出点点之间的关系
        private static void P2Ps_Connection(ref WEntity2D[] Cs, ref WGeometry2D WGC, ref int[][] Ls,
                                            ref List<int>[] Ns, ref List<WVector2D>[] Vs)
        {
            double[] Xs = new double[0];                  /////每个节点的X坐标
            double[] Ys = new double[0];                  /////每个节点的Y坐标
            
            bool c;       ////判断某个节点是否可以输入，默认为true，重复则输入false
            WPoint2D[] Pt = new WPoint2D[2];
            double x, y;
            int Q;
            ///
            for (int i = 0; i < Cs.Length; i++)
            {
                Ls[i] = new int[2];
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
                            Ls[i][j] = k;
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
                        Ls[i][j] = Q - 1;
                    }
                }
            }
            //////
            int Nl;
            Vs = new List<WVector2D>[Ns.Length];
            /////此时将Ns转换为每个节点相连的节点的编号
            for (int i = 0; i < Ns.Length; i++)
            {
                Vs[i] = new List<WVector2D>();
                for (int j = 0; j < Ns[i].Count; j++)
                {
                    Nl = Ns[i][j];
                    if (Ls[Nl][0] == i)
                    {
                        Ns[i][j] = Ls[Nl][1];
                        Vs[i].Add(((WCurve2D)Cs[Nl]).Direction_Head);
                    }
                    else
                    {
                        Ns[i][j] = Ls[Nl][0];
                        Vs[i].Add(((WCurve2D)Cs[Nl]).Direction_Tail);
                    }
                }
            }
        }

        /// 求出每个点的内夹角
        private static void P2Ps_Angles(ref List<int>[] Ns,ref List<WVector2D>[] Vs, ref List<PAngle>[] As)
        {
            double[] Ans;
            int[] Nus;
            WVector2D[] Vns;
            double Ant;
            int Nut;
            WVector2D Vnt;
            Array.Resize<List<PAngle>>(ref As, Ns.Length);
            for (int i = 0; i < Ns.Length; i++)
            {
                Ans = new double[Ns[i].Count];
                Nus = new int[Ns[i].Count];
                Vns = new WVector2D[Ns[i].Count];
                Ans[0] = 0;
                Nus[0] = Ns[i][0];
                Vns[0] = Vs[i][0];
                /////赋值
                for (int j = 1; j < Ns[i].Count; j++)
                {
                    Ans[j] = Vs[i][0].AngleAntiCrossWiseTo(Vs[i][j]);
                    Nus[j] = Ns[i][j];
                    Vns[j] = Vs[i][j];
                }
                /////排序
                for (int j = 1; j < Ans.Length - 1; j++)
                    for (int k = j + 1; k < Ans.Length; k++)
                        if (Ans[j] > Ans[k])
                        {
                            Ant = Ans[k];
                            Nut = Nus[k];
                            Vnt = Vns[k];
                            Ans[k] = Ans[j];
                            Nus[k] = Nus[j];
                            Vns[k] = Vns[j];
                            Ans[k] = Ant;
                            Nus[j] = Nut;
                            Vns[j] = Vnt;
                        }
                /////求取
                As[i] = new List<PAngle>();
                for (int j = 0; j < Ans.Length - 1; j++)
                    As[i].Add(new PAngle(i, Nus[j], Nus[j + 1], Vns[j].AngleAntiCrossWiseTo(Vns[j + 1])));
                As[i].Add(new PAngle(i, Nus[Ans.Length - 1], Nus[0], Vns[Ans.Length - 1].AngleAntiCrossWiseTo(Vns[0])));
            }
            Ans = null;
            Nus = null;
            Vns = null;
        }
        #endregion

        #region 计算、判断、转换函数
        /// 删除重复的Rim
        private static void Delete_Repeat(ref List<int>[] Rims)
        {
            List<int> R1, R2;
            bool[] Check = new bool[Rims.Length];
            for (int i = 0; i < Rims.Length - 1; i++) Check[i] = true;
            for (int i = 0; i < Rims.Length - 1; i++)
            {
                for (int j = i + 1; j < Rims.Length; j++)
                {
                    if (Rims[i].Count != Rims[j].Count) continue;
                    R1 = new List<int>();
                    R2 = new List<int>();
                    for (int k = 0; k < Rims[i].Count; k++)
                    {
                        R1.Add(Rims[i][k]);
                        R2.Add(Rims[j][k]);
                    }
                    R1.Sort();
                    R2.Sort();
                    Check[j]=false;
                    for (int k = 0; k < Rims[i].Count; k++)
                        if (R1[k] != R2[k]) Check[j] = true;
                }
            }
            R1 = null;
            R2 = null;
            /////
            for (int i = 0; i < Check.Length; i++)
            {
                if (Check[i] == false)
                {
                    if (i != Check.Length)
                    {
                        for (int j = i; j < Check.Length - 1; j++)
                        {
                            Rims[j] = Rims[j + 1];
                            Check[j] = Check[j + 1];
                        }
                    }
                    Array.Resize<List<int>>(ref Rims, Rims.Length - 1);
                    Array.Resize<bool>(ref Check, Check.Length - 1);
                }
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
        #endregion

        #endregion
    }
}
