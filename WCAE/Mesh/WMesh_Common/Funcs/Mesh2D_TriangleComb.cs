using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TriangleNet;
using TriangleNet.IO;
using TriangleNet.Geometry;
using TriangleNet.Data;
using WCAE.WMesh2D.Entities;

namespace WCAE.WMesh2D.Funcs
{
    /// <summary>
    /// 三角形单元合并为四边形单元
    /// </summary>
    static class Mesh2D_TriangleComb
    {
        #region Step1_对所有单元进行合并
        /// <summary>
        /// 对所有单元进行合并
        /// </summary>
        /// <param name="Ns"></param>
        /// <param name="Es"></param>
        /// <param name="MP"></param>
        public static void Elements_Combine(ref Node[] Ns, ref Element[] Es, ref WMesh2D_Para MP)
        {
            for (int i = 0; i < Es.Length; i++)
                Element_Combine(ref Ns, ref Es, i, MP.MaxAngle_Qua);
        }
        #endregion

        #region "Step2_对某个单元进行合并"
        static private void Element_Combine(ref Node[] Ns, ref Element[] Es, int Num_E, double Angle_Max)
        {
            if (Es[Num_E].Valid == false)  /////该单元无效就不要继续了
                return;
            if (Es[Num_E].Kind == 4)
                return;
            int[] EN = Element_Combine_ParaCompute(ref Ns, ref Es, Num_E, Angle_Max);
            if (EN[0] == 0)                /////无法合并就不要继续了
                return;
            Es[Num_E].Kind = 4;
            Es[Num_E].N1 = EN[1];
            Es[Num_E].N2 = EN[2];
            Es[Num_E].N3 = EN[3];
            Es[Num_E].N4 = EN[4];

            EsB_Removing(ref Ns[EN[1]], EN[0]);
            EsB_Removing(ref Ns[EN[2]], EN[0]);
            EsB_Removing(ref Ns[EN[4]], EN[0]);
            Es[EN[0]].Valid = false;
        }
        #endregion

        #region "Step3_计算单次合并的最优的单元及节点"
        /// <summary>
        /// 合并单元，找到合适的相邻单元及第四节点
        /// </summary>
        /// <param name="Ns"></param>
        /// <param name="Es"></param>
        /// <param name="Num_E">需要合并的单元编号</param>
        /// <returns></returns>
        static private int[] Element_Combine_ParaCompute(ref Node[] Ns, ref Element[] Es, int Num_E, double Angle_Max)
        {
            double Cos_E = Math.Cos(Angle_Max / 180 * Math.PI);   /////能够接受的最大角度的Cos值

            int N1 = Es[Num_E].N1;
            int N2 = Es[Num_E].N2;
            int N3 = Es[Num_E].N3;

            Node Ns1 = Ns[N1];
            Node Ns2 = Ns[N2];

            int[][] Nos = new int[3][];
            Nos[0] = Fourth_Node_Find(N1, N2, N3, Num_E, ref Ns, ref Es);     /////N1,N2情况
            Nos[1] = Fourth_Node_Find(N2, N3, N1, Num_E, ref Ns, ref Es);     /////N2,N3情况
            Nos[2] = Fourth_Node_Find(N3, N1, N2, Num_E, ref Ns, ref Es);     /////N1,N3情况

            double[] A = new double[3];
            A[0] = Fourth_Node_Check(ref Ns, Nos[0], Cos_E);
            A[1] = Fourth_Node_Check(ref Ns, Nos[1], Cos_E);
            A[2] = Fourth_Node_Check(ref Ns, Nos[2], Cos_E);
            ///////////////////
            byte K = 4;
            double Am = 9999;
            bool[] C = new bool[3];
            for (byte i = 0; i < 3; i++)
            {
                if (A[i] <= 0)
                    C[i] = false;
                else
                {
                    C[i] = true;
                    K = i;
                    Am = A[i];
                }
            }
            if (K == 4)                    /////如果没有一个点满足要求，就不要继续了
                return new int[] { 0, 0 };
            //////////////////
            for (byte i = 0; i < 3; i++)
            {
                if (A[i] < Am && C[i] == true)
                {
                    K = i;
                    Am = A[i];
                }
            }
            return Nos[K];
        }
        #endregion

        #region "Step4_求相邻单元及第四点"
        /// <summary>
        /// 根据四点的角度值判断是否可用，如果可用则输出参数
        /// </summary>
        /// <param name="Ns"></param>
        /// <param name="No"></param>
        /// <param name="Cos_Min"></param>
        /// <returns></returns>
        static private double Fourth_Node_Check(ref Node[] Ns, int[] No, double Cos_Min)
        {
            if (No[0] == 0 || No[1] == 0)  /////如果没找到相邻节点就不要继续了
                return -1;
            ////第一个角
            double[] SC = SinCos_4Nodes(ref Ns, No[3], No[1], No[4], No[2]);
            if (SC[0] <= 0)        /////如果第一个角大于180度就不要继续了
                return -1;
            if (SC[1] < Cos_Min)   /////如果第一个角大于最大限制角就不要继续了
                return -1;
            ////第二个角
            SC = SinCos_4Nodes(ref Ns, No[3], No[1], No[2], No[4]);
            if (SC[0] <= 0)        /////如果第二个角大于180度就不要继续了
                return -1;
            if (SC[1] < Cos_Min)   /////如果第二个角大于最大限制角就不要继续了
                return -1;

            return Aspect_Compute(DistanceSqure_P2P(No[1], No[3], ref Ns),
                                  DistanceSqure_P2P(No[2], No[4], ref Ns));
        }

        /// <summary>
        /// 已知某单元的两节点求相邻单元编号及相应的第四节点
        /// </summary>
        /// <param name="N1">相邻边点1</param>
        /// <param name="N2">相邻边点2</param>
        /// <param name="N3">相邻边以外的一点</param>
        /// <param name="Em">母单元</param>
        /// <param name="Ns">节点集</param>
        /// <param name="Es">单元集</param>
        /// <returns></returns>
        static private int[] Fourth_Node_Find(int N1, int N2, int N3, int Em, ref Node[] Ns, ref Element[] Es)
        {
            int E = Intersec_Arrays(Ns[N1].EsB, Ns[N2].EsB, Em);

            if (E == 0)                   /////判断是否有相邻单元
                return new int[] { 0, 0 };
            if (Es[E].Valid == false)     /////判断该单元是否依旧有效
                return new int[] { 0, 0 };
            if (Es[E].Kind == 4)          /////判断该单元是否已经经过合并
                return new int[] { 0, 0 };
            if (Es[E].N1 != N1 && Es[E].N1 != N2)
                return new int[] { E, Es[E].N1, N2, N3, N1 };
            if (Es[E].N2 != N1 && Es[E].N2 != N2)
                return new int[] { E, Es[E].N2, N2, N3, N1 };
            if (Es[E].N3 != N1 && Es[E].N3 != N2)
                return new int[] { E, Es[E].N3, N2, N3, N1 };
            return new int[] { 0, 0 };
        }
        #endregion

        #region "参数计算"
        /// 求判断四边形两对角线的比例（大于1，越小越好)
        static private double Aspect_Compute(double L1, double L2)
        {
            if (L1 > L2)
                return L1 / L2;
            else
                return L2 / L1;
        }

        /// 已知两点，求两点的距离平方
        static private double DistanceSqure_P2P(int N1, int N2, ref Node[] Ns)
        {
            return (Ns[N2].x - Ns[N1].x) * (Ns[N2].x - Ns[N1].x) + (Ns[N2].y - Ns[N1].y) * (Ns[N2].y - Ns[N1].y);
        }

        /// 计算两个int数组的交叉数，用于已知两节点求相邻的单元编号
        static private int Intersec_Arrays(int[] Ns1, int[] Ns2, int Ne)
        {
            for (int i = 0; i < Ns1.Length; i++)
                for (int j = 0; j < Ns2.Length; j++)
                    if (Ns1[i] == Ns2[j] && Ns1[i] != Ne)
                        return Ns1[i];
            return 0;
        }

        /// 在节点的单元列表中增加某个单元编号
        static private void EsB_Adding(ref Node N, int E)
        {
            Array.Resize<int>(ref N.EsB, N.EsB.Length + 1);
            N.EsB[N.EsB.Length - 1] = E;
        }

        /// 从节点的连接单元列表中除去某个单元编号
        static private void EsB_Removing(ref Node N, int E)
        {
            for (int i = 0; i < N.EsB.Length; i++)
            {
                if (N.EsB[i] == E)
                {
                    for (int j = i; j < N.EsB.Length - 1; j++)
                        N.EsB[j] = N.EsB[j + 1];
                    Array.Resize<int>(ref N.EsB, N.EsB.Length - 1);
                    return;
                }
            }
        }

        /// 求NaNoNb角的Sin及Cos
        static private double[] SinCos_Compute(ref Node[] Ns, int Na, int No, int Nb)
        {
            double Xa = Ns[Na].x - Ns[No].x;
            double Ya = Ns[Na].y - Ns[No].y;
            double Xb = Ns[Nb].x - Ns[No].x;
            double Yb = Ns[Nb].y - Ns[No].y;
            double Dot = Xa * Xb + Ya * Yb;
            double La = Math.Sqrt(Xa * Xa + Ya * Ya);
            double Lb = Math.Sqrt(Xb * Xb + Yb * Yb);
            double Cos = Dot / (La * Lb);
            double Sin = Math.Sqrt(1 - Cos * Cos);
            return new double[] { Sin, Cos };
        }

        /// 已知四点求两角和的Sin和Cos
        static private double[] SinCos_4Nodes(ref Node[] Ns, int Na, int Nb, int No, int Nm)
        {
            double[] SCa = SinCos_Compute(ref Ns, Na, No, Nm);
            double[] SCb = SinCos_Compute(ref Ns, Nb, No, Nm);
            double Sin = SCa[0] * SCb[1] + SCa[1] * SCb[0];
            double Cos = SCa[1] * SCb[1] - SCa[0] * SCb[0];
            return new double[] { Sin, Cos };
        }
        #endregion

        #region "输入"
        static private int Node_Input(string Name, ref Node[] Ns)
        {
            StreamReader sr_Node = new StreamReader(Name);

            string t = sr_Node.ReadLine();
            string[] t_1 = t.Split(' ');
            int Quan = Convert.ToInt32(t_1[0]);
            Array.Resize<Node>(ref Ns, Quan + 1);

            double x, y;

            for (int i = 1; i <= Quan; i++)
            {
                t = sr_Node.ReadLine();
                t = t.Replace("  ", " ");
                t = t.Replace("  ", " ");
                t = t.Replace("  ", " ");
                t_1 = t.Split(' ');

                x = Convert.ToDouble(t_1[2]);
                y = Convert.ToDouble(t_1[3]);

                Ns[i] = new Node(x, y);
            }
            return Quan;
        }
        static private int Elem_Input(string Name, ref Element[] Es, ref Node[] Ns)
        {
            StreamReader sr_Elem = new StreamReader(Name);

            string t = sr_Elem.ReadLine();
            string[] t_1 = t.Split(' ');
            int Quan = Convert.ToInt32(t_1[0]);
            Array.Resize<Element>(ref Es, Quan + 1);

            int Num, N1, N2, N3, n;

            for (int i = 1; i <= Quan; i++)
            {
                t = sr_Elem.ReadLine();
                t = t.Replace("  ", " ");
                t = t.Replace("  ", " ");
                t = t.Replace("  ", " ");
                t = t.Replace("  ", " ");
                t_1 = t.Split(' ');

                n = 0;
                if (t_1[0] == "")
                    n = 1;
                Num = Convert.ToInt32(t_1[n + 0]);
                N1 = Convert.ToInt32(t_1[n + 1]);
                N2 = Convert.ToInt32(t_1[n + 2]);
                N3 = Convert.ToInt32(t_1[n + 3]);

                EsB_Adding(ref Ns[N1], i);
                EsB_Adding(ref Ns[N2], i);
                EsB_Adding(ref Ns[N3], i);

                Es[i] = new Element(Num, N1, N2, N3);
            }
            return Quan;
        }
        #endregion
    }
}
