using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using WCAE.Entities;
using WCAE.WMesh2D;
using WCAE.WMesh2D.Funcs;
using WCAE.WMesh2D.Entities;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Funcs;
using WCAE.WGeos2D.Entities;

namespace WCAE.WMesh2D.Funcs
{
    /// <summary>
    /// 自己编写的Mesh类，用于Mesh单层的四边形或三角形区域
    /// </summary>
    public static class Mesh2D_SingleLayerMesh
    {
        public static WElement2D[] Do_Mesh(ref WEntity2D[] Rim_Curves, string Name, ref WMesh2D_Para MP)
        {
            List<WPoint2D> Ps = new List<WPoint2D>(); ///// 节点集
            List<int> Ns = new List<int>();           ///// 节点编号集
            Get_Rim(ref Rim_Curves, ref Ps, ref Ns);


            int[] Corner = Find_Corner(ref Ps);

            WElement2D[] Es = new WElement2D[0];
            if (Corner.Length == 3)
                Es = Mesh_TriCorner(ref Corner, Ns, Ps);
            if (Corner.Length == 4)
                Es = Mesh_FourCorner(ref Corner, Ns, Ps);
            return Es;
        }

        private static WElement2D[] Mesh_TriCorner(ref int[] Corner, List<int> Ns, List<WPoint2D> Ps)
        {
            int Total = Ps.Count;
            int[] Length = new int[3];
            Length[0] = Corner[1] - Corner[0];
            Length[1] = Corner[2] - Corner[1];
            Length[2] = Corner[0] + Total - Corner[2];
            int Len1 = 0; int Len2 = 0;
            int Cor1 = 0; int Cor2 = 0; int Cor3 = 0;
            bool Pre_Check = true;
            int MaxID = Find_MaxID(ref Length);
            Len1 = Length[MaxID];
            if (Length[PreID(MaxID, ref Length)] == 1)
            {
                Cor1 = Corner[MaxID];
                Cor2 = Corner[PreID(MaxID, ref Corner)];
                Cor3 = Corner[NextID(MaxID, ref Corner)];
                Len2 = Length[PreID(PreID(MaxID, ref Length), ref Length)];
                Pre_Check = true;
            }
            else if (Length[NextID(MaxID, ref Length)] == 1)
            {
                Cor1 = Corner[MaxID + 1];
                Cor2 = Corner[NextID(MaxID + 1, ref Corner)];
                Cor3 = Corner[PreID(MaxID + 1, ref Corner)];
                Len2 = Length[NextID(NextID(MaxID, ref Length), ref Length)];
                Pre_Check = false;
            }
            else return null;
            /////
            int[] Ls1 = new int[Len1 + 1];
            int[] Ls2 = new int[Len2];
            /////
            if (Pre_Check == true)
            {
                Ls1[0] = Cor1;
                for (int i = 1; i < Ls1.Length; i++)
                    Ls1[i] = NextNum(Ls1[i - 1], Total - 1);
                Ls2[0] = Cor2;
                for (int i = 1; i < Ls2.Length; i++)
                    Ls2[i] = PreNum(Ls2[i - 1], Total - 1);
            }
            else
            {
                Ls1[0] = Cor1;
                Ls2[0] = Cor2;
                for (int i = 1; i < Ls1.Length; i++)
                    Ls1[i] = PreNum(Ls1[i - 1], Total - 1);
                Ls2[0] = Cor2;
                for (int i = 1; i < Ls2.Length; i++)
                    Ls2[i] = NextNum(Ls2[i - 1], Total - 1);
            }
            return Create_Elements(ref Ls1, ref Ls2, Ns, Ps, Pre_Check);
        }

        private static WElement2D[] Mesh_FourCorner(ref int[] Corner, List<int> Ns, List<WPoint2D> Ps)
        {
            int Total = Ps.Count;
            int[] Length = new int[4];
            Length[0] = Corner[1] - Corner[0];
            Length[1] = Corner[2] - Corner[1];
            Length[2] = Corner[3] - Corner[2];
            Length[3] = Corner[0] + Total - Corner[3];
            int Len1 = 0; int Len2 = 0; int Len4 = 0;
            int Cor1 = 0; int Cor2 = 0; int Cor3 = 0;
            bool Pre_Check = true;
            int MaxID = Find_MaxID(ref Length);
            Len1 = Length[MaxID];
            if (Length[PreID(MaxID, ref Length)] == 1)
            {
                Cor1 = Corner[MaxID];
                Cor2 = Corner[PreID(MaxID, ref Corner)];
                Cor3 = Corner[NextID(MaxID, ref Corner)];
                Len2 = Length[PreID(PreID(MaxID, ref Length), ref Length)];
                Len4 = Length[NextID(MaxID, ref Length)];
                Pre_Check = true;
            }
            else if (Length[NextID(MaxID, ref Length)] == 1)
            {
                Cor1 = Corner[MaxID + 1];
                Cor2 = Corner[NextID(MaxID + 1, ref Corner)];
                Cor3 = Corner[PreID(MaxID + 1, ref Corner)];
                Len2 = Length[NextID(NextID(MaxID, ref Length), ref Length)];
                Len4 = Length[PreID(MaxID, ref Length)];
                Pre_Check = false;
            }
            else
                return null;
            /////
            int[] Ls1 = new int[Len1 + 1];
            int[] Ls2 = new int[Len2 + Len4];
            /////
            if (Pre_Check == true)
            {
                Ls1[0] = Cor1;
                for (int i = 1; i < Ls1.Length; i++)
                    Ls1[i] = NextNum(Ls1[i - 1], Total - 1);
                Ls2[0] = Cor2;
                for (int i = 1; i < Ls2.Length; i++)
                    Ls2[i] = PreNum(Ls2[i - 1], Total - 1);
            }
            else
            {
                Ls1[0] = Cor1;
                Ls2[0] = Cor2;
                for (int i = 1; i < Ls1.Length; i++)
                    Ls1[i] = PreNum(Ls1[i - 1], Total - 1);
                Ls2[0] = Cor2;
                for (int i = 1; i < Ls2.Length; i++)
                    Ls2[i] = NextNum(Ls2[i - 1], Total - 1);
            }
            return Create_Elements(ref Ls1, ref Ls2, Ns, Ps, Pre_Check);
        }

        private static WElement2D[] Create_Elements(ref int[] Ls1, ref int[] Ls2, List<int> Nums, List<WPoint2D> Ps, bool PreCheck)
        {
            //bool ClockCheck = false;
            //if ((Ps[Ls1[0]].X > Ps[Ls2[0]].X) && (Ps[Ls1[0]].Y > Ps[Ls1[Ls1.Length - 1]].Y))
            //    ClockCheck = true;
            //if ((Ps[Ls1[0]].X < Ps[Ls2[0]].X) && (Ps[Ls1[0]].Y < Ps[Ls1[Ls1.Length - 1]].Y))
            //    ClockCheck = true;
            //if ((Ps[Ls1[0]].Y < Ps[Ls2[0]].Y) && (Ps[Ls1[0]].X > Ps[Ls1[Ls1.Length - 1]].X))
            //    ClockCheck = true;
            //if ((Ps[Ls1[0]].Y > Ps[Ls2[0]].Y) && (Ps[Ls1[0]].X < Ps[Ls1[Ls1.Length - 1]].X))
            //    ClockCheck = true;
            ///////

            WElement2D[] Es = new WElement2D[0];
            int L1 = Ls1.Length;
            int L2 = Ls2.Length;
            int Lm = L1;
            if (L2 < Lm) Lm = L2;

            bool RevCheck = Check_Reverse(Ls1[L1 - 1], Ls1[L1 - 2], Ls2[L2 - 1], Ps);

            Add_Element(ref Es, 3, Ls1[L1 - 1], Ls1[L1 - 2], Ls2[L2 - 1], 0, RevCheck, ref Nums);   //////最后先用三角形封起来
            Array.Resize<int>(ref Ls1, L1 - 1);
            L1--;

            while (L1 != L2)
            {
                if (L1 > L2)
                {
                    Add_Element(ref Es, 3, Ls1[L1 - 1], Ls1[L1 - 2], Ls2[L2 - 1], 0, RevCheck, ref Nums);
                    Array.Resize<int>(ref Ls1, L1 - 1);
                    L1--;
                }
                else
                {
                    Add_Element(ref Es, 3, Ls2[L2 - 2], Ls2[L2 - 1], Ls1[L1 - 1], 0, RevCheck, ref Nums);
                    Array.Resize<int>(ref Ls2, L2 - 1);
                    L2--;
                }
            }

            for (int i = L1 - 1; i > 0; i--)
                Add_Element(ref Es, 4, Ls1[i], Ls1[i - 1], Ls2[i - 1], Ls2[i], RevCheck, ref Nums);

            return Es;
        }

        private static bool Check_Reverse(int N1, int N2, int N3, List<WPoint2D> Ps)
        {
            WPoint2D O = new WPoint2D((Ps[N1].X + Ps[N2].X + Ps[N3].X) / 3, (Ps[N1].Y + Ps[N2].Y + Ps[N3].Y) / 3, 0);
            WVector2D V1 = O.VectorTo(Ps[N1]);
            WVector2D V2 = O.VectorTo(Ps[N2]);
            WVector2D V3 = O.VectorTo(Ps[N3]);
            double A2 = V1.AngleAntiCrossWiseTo(V2);
            double A3 = V1.AngleAntiCrossWiseTo(V3);
            if (A2 > A3) return true;
            else return false; 
        }

        private static void Add_Element(ref WElement2D[] Es, int Kind,
                                        int N1, int N2, int N3, int N4,
                                        bool RevCheck, ref List<int> Nums)
        {
            Array.Resize<WElement2D>(ref Es, Es.Length + 1);
            /////
            if (Kind == 4)
            {
                if (RevCheck == false)
                    Es[Es.Length - 1] = new WElement2D(Kind, Nums[N1], Nums[N2], Nums[N3], Nums[N4]);
                else
                    Es[Es.Length - 1] = new WElement2D(Kind, Nums[N4], Nums[N3], Nums[N2], Nums[N1]);
            }
            if (Kind == 3)
            {
                if (RevCheck == false)
                    Es[Es.Length - 1] = new WElement2D(Kind, Nums[N1], Nums[N2], Nums[N3], 0);
                else
                    Es[Es.Length - 1] = new WElement2D(Kind, Nums[N3], Nums[N2], Nums[N1], 0);
            }
        }

        private static void Add_Element2(ref WElement2D[] Es, int Kind,
                                        int N1, int N2, int N3, int N4,
                                        bool PreCheck, bool ClockCheck, ref List<int> Nums)
        {
            Array.Resize<WElement2D>(ref Es, Es.Length + 1);
            if (ClockCheck == false)
                if (PreCheck == true) PreCheck = false;
                else PreCheck = true;
            /////
            if (Kind == 4)
            {
                if (PreCheck == true)
                    Es[Es.Length - 1] = new WElement2D(Kind, Nums[N1], Nums[N2], Nums[N3], Nums[N4]);
                else
                    Es[Es.Length - 1] = new WElement2D(Kind, Nums[N4], Nums[N3], Nums[N2], Nums[N1]);
            }
            if (Kind == 3)
            {
                if (PreCheck == true)
                    Es[Es.Length - 1] = new WElement2D(Kind, Nums[N1], Nums[N2], Nums[N3], 0);
                else
                    Es[Es.Length - 1] = new WElement2D(Kind, Nums[N3], Nums[N2], Nums[N1], 0);
            }
        }

        #region 其他计算
        /// 找角点
        private static int[] Find_Corner(ref List<WPoint2D> Ps)
        {
            double Check_Level = WGeos2D_Paras.Split_Angle / 180 * Math.PI;
            Check_Level = Math.Cos(Check_Level);
            WVector2D V1, V2;
            int[] Corner = new int[0];

            double Check;
            /////
            V1 = Ps[0].VectorTo(Ps[1]);
            V2 = Ps[0].VectorTo(Ps[Ps.Count - 1]);
            Check = (V1.X * V2.X + V1.Y * V2.Y) / (V1.Length * V2.Length);
            if (Check > Check_Level)
            {
                Array.Resize<int>(ref Corner, Corner.Length + 1);
                Corner[Corner.Length - 1] = 0;
            }
            /////
            for (int i = 1; i < Ps.Count - 1; i++)
            {
                V1 = Ps[i].VectorTo(Ps[i - 1]);
                V2 = Ps[i].VectorTo(Ps[i + 1]);
                Check = (V1.X * V2.X + V1.Y * V2.Y) / (V1.Length * V2.Length);
                if (Check > Check_Level)
                {
                    Array.Resize<int>(ref Corner, Corner.Length + 1);
                    Corner[Corner.Length - 1] = i;
                }
            }
            /////
            V1 = Ps[Ps.Count - 1].VectorTo(Ps[0]);
            V2 = Ps[Ps.Count - 1].VectorTo(Ps[Ps.Count - 2]);
            Check = (V1.X * V2.X + V1.Y * V2.Y) / (V1.Length * V2.Length);
            if (Check > Check_Level)
            {
                Array.Resize<int>(ref Corner, Corner.Length + 1);
                Corner[Corner.Length - 1] = Ps.Count - 1;
            }
            return Corner;
        }

        /// 找到0节点和节点编号的圈圈
        private static void Get_Rim(ref WEntity2D[] Rim_Curves, ref List<WPoint2D> Ps_Out, ref List<int> Ns_Out)
        {
            for (int i = 0; i < Rim_Curves.Length; i++)
                if (((WCurve2D)Rim_Curves[i]).Meshed_Check == false) return;   /////如果有线没有Mesh则返回-1

            int[] Sorts = Geos2D_Other.Sort_Curves(Rim_Curves);

            int Num;
            bool Tc;                   /////判断某条线是否首尾颠倒,true为颠倒，false为不颠倒
            ///////处理第一条线///////
            Num = 0;
            WCurve2D C = (WCurve2D)Rim_Curves[Num];
            List<int> Ns = new List<int>();
            List<WPoint2D> Ps = new List<WPoint2D>();
            for (int i = 1; i < C.Nodes.Count; i++)
            {
                Ps.Add(C.Nodes[i]);      /////将Curve的Node复制出来至Ps
                Ns.Add(C.Nodes_num[i]);
            }
            Ps_Out = Ps;
            Ns_Out = Ns;

            //////处理中间线//////////
            for (int i = 1; i <= Sorts.Length - 1; i++)
            {
                Num = (int)(Math.Floor((double)(Sorts[i] / 2)));
                if (Sorts[i] - 2 * Num == 0)
                    Tc = false;
                else
                    Tc = true;         /////首尾颠倒则为true

                Ps = new List<WPoint2D>();
                Ns = new List<int>();
                C = (WCurve2D)Rim_Curves[Num];
                for (int j = 0; j < C.Nodes.Count; j++)
                {
                    Ps.Add(C.Nodes[j]); /////将Curve的Node复制出来至Ps
                    Ns.Add(C.Nodes_num[j]);
                }
                WMFuncs2D.Points_Reverse(ref Ps, Tc);                       /////翻转点集
                WMFuncs2D.Nums_Reverse(ref Ns, Tc);                         /////翻转点集
                Ps.RemoveAt(0);                                             /////去掉起点
                Ns.RemoveAt(0);                                             /////去掉起点

                for (int j = 0; j < Ps.Count; j++)
                {
                    Ps_Out.Add(Ps[j]);
                    Ns_Out.Add(Ns[j]);
                }
            }
        }
        #endregion

        #region 下标计算
        private static int NextNum(int N, int Last)
        {
            if (N == Last) return 0;
            return N + 1;
        }

        private static int PreNum(int N, int Last)
        {
            if (N == 0) return Last;
            return N - 1;
        }

        private static int NextID(int N, ref int[] Array)
        {
            if (N == Array.Length - 1) return 0;
            return N + 1;
        }

        private static int PreID(int N, ref int[] Array)
        {
            if (N == 0) return Array.Length - 1;
            return N - 1;
        }

        private static int Find_MaxID(ref int[] Array)
        {
            int max = int.MinValue;
            int N = 0;
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i] > max)
                {
                    max = Array[i];
                    N = i;
                }
            }
            return N;
        }
        #endregion
    }
}
