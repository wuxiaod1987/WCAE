using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WCAE.Entities;
using WCAE.WMesh2D.Entities;
using WCAE.WGeos2D;

namespace WCAE.WMesh2D.Funcs
{
    /// <summary>
    /// 2D网格类合并，返回网格类，只扫描自由节点，效率较高
    /// </summary>
    public static class Mesh2D_Merge_Mesh2Mesh
    {
        static int[] Ns1;         /////存储Mesh1的节点编号，首先进行自由节点合并再继续输入
        static int[] Ns2;         /////存储Mesh2的节点编号，首先进行自由节点合并再继续输入
        static int Nc;            /////重合点数量
        static int Num;           /////输出的节点的数量
        static int QF1;           /////Mesh1的Free点集数量
        static int QF2;           /////Mesh2的Free点集数量

        private static void Initial()
        {
            Ns1 = new int[0];
            Ns2 = new int[0];
            Nc = 0;
            Num = 0;
            QF1 = 0;
            QF2 = 0;
        }

        /// 网格合并
        /// <summary>
        /// 网格合并
        /// </summary>
        /// <param name="Mesh1"></param>
        /// <param name="Mesh2"></param>
        /// <param name="Name_New">新网格的名称</param>
        /// <returns></returns>
        public static WMesh2D_Mesh Mesh_Combine(WMesh2D_Mesh Mesh1, WMesh2D_Mesh Mesh2, string Name_New)
        {
            Initial();
            WMesh2D_Mesh Mesh_Out = new WMesh2D_Mesh(Name_New);
            Combine_FreeNodes(ref Mesh1, ref Mesh2, ref Mesh_Out);           /////处理自由节点，形成两个Mesh的自由节点的新的编号

            Num = Read_OtherNodes(ref Ns1, ref Mesh1, ref Mesh_Out, Num);
            Num = Read_OtherNodes(ref Ns2, ref Mesh2, ref Mesh_Out, Num);

            MeshFile_ElementRead(ref Ns1, ref Mesh1, ref Mesh_Out);
            MeshFile_ElementRead(ref Ns2, ref Mesh2, ref Mesh_Out);

            Ns1 = new int[0];
            Ns2 = new int[0];

            return Mesh_Out;
        }

        /// 读取单元，转换单元编号，输出
        private static void MeshFile_ElementRead(ref int[] Ns, ref WMesh2D_Mesh Mesh_in, ref WMesh2D_Mesh Mesh_Out)
        {
            WElement2D E;
            for (int i = 0; i < Mesh_in.Elements.Count; i++)
            {
                E = Mesh_in.Elements[i];
                E.N1 = Ns[E.N1];
                E.N2 = Ns[E.N2];
                E.N3 = Ns[E.N3];
                E.N4 = Ns[E.N4];
                Mesh_Out.Elements.Add(E); 
            }
        }

        /// 读取剩余节点并直接输出
        /// <param name="Ns">用Ref传递的节点编号数组，该数组会被逐渐扩充</param>
        /// <param name="Num">用以记录节点数量的变量，输出值也为该参数</param>
        private static int Read_OtherNodes(ref int[] Ns, ref WMesh2D_Mesh Mesh_in, ref WMesh2D_Mesh Mesh_Out, int Num)
        {
            int Quan = Ns.Length;
            for (int i = Mesh_in.Q_FreeNs + 1; i < Mesh_in.Nodes.Count; i++)
            {
                Quan++;
                Array.Resize<int>(ref Ns, Quan);
                Num++;
                Ns[Quan - 1] = Num;
                Mesh_Out.Add_N(Mesh_in.Nodes[i]);
            }
            return Num;
        }

        private static void Combine_FreeNodes(ref WMesh2D_Mesh Mesh1, ref WMesh2D_Mesh Mesh2, ref WMesh2D_Mesh Mesh_Out)
        {
            QF1 = Mesh1.Q_FreeNs;
            QF2 = Mesh2.Q_FreeNs;

            Array.Resize<int>(ref Ns1, QF1 + 1);
            Array.Resize<int>(ref Ns2, QF2 + 1);
            for (int i = 0; i <= QF1; i++)
                Ns1[i] = 0;
            for (int i = 0; i <= QF2; i++)
                Ns2[i] = 0;

            for (int i = 1; i <= QF1; i++)
                for (int j = 1; j <= QF2; j++)
                    if (Near_Check(Mesh1.Nodes[i].X, Mesh1.Nodes[i].Y, Mesh2.Nodes[j].X, Mesh2.Nodes[j].Y, WGeos2D_Paras.E_Merge) == true)
                    {
                        Ns1[i] = j * (-1);  /////用负数表示重合节点的编号
                        Ns2[j] = i * (-1);  /////用正数表示输出时的节点编号
                        Nc++;
                    }

            Mesh_Out.Q_FreeNs = QF1 + QF2 - Nc;
            ///输出Mesh1的自由节点
            for (int i = 1; i <= QF1; i++)
                if (Ns1[i] == 0)
                {
                    Num++;
                    Mesh_Out.Add_N(new WNode2D(Num, Mesh1.Nodes[i].X, Mesh1.Nodes[i].Y, Mesh1.Nodes[i].Trace));
                    Ns1[i] = Num;
                }
            ///输出Mesh2的自由节点
            for (int i = 1; i <= QF2; i++)
                if (Ns2[i] == 0)
                {
                    Num++;
                    Mesh_Out.Add_N(new WNode2D(Num, Mesh2.Nodes[i].X, Mesh2.Nodes[i].Y, Mesh2.Nodes[i].Trace));
                    Ns2[i] = Num;
                }
            ///////////////////////
            ///处理重复节点
            string t = "";
            int Trace_T;
            int T1, T2;
            int Nt = 0;
            for (int i = 1; i <= QF1; i++)
                if (Ns1[i] < 0)
                {
                    Num++;
                    //if (Mesh2.Nodes[Ns1[i] * (-1)].Trace != 0)
                    //    Trace_T = Mesh2.Nodes[Ns1[i] * (-1)].Trace;
                    //else
                    //    Trace_T = Mesh1.Nodes[i].Trace;

                    T1 = Mesh2.Nodes[Ns1[i] * (-1)].Trace;
                    T2 = Mesh1.Nodes[i].Trace;
                    if (T1 == T2) Trace_T = T1;
                    else
                    {
                        if (T1 < 0 && T2 > 0) Trace_T = T1 - T2;
                        else if (T1 > 0 && T2 < 0) Trace_T = T2 - T1;
                        else Trace_T = T1 + T2;
                    }

                    Mesh_Out.Add_N(new WNode2D(Num, Mesh1.Nodes[i].X, Mesh1.Nodes[i].Y, Trace_T));
                    Nt = -1 * i;
                    for (int j = 1; j <= QF2; j++)
                        if (Ns2[j] == Nt)
                            Ns2[j] = Num;
                    Ns1[i] = Num;
                }
        }

        private static bool Near_Check(double x1, double y1, double x2, double y2, double EPSILON)
        {
            if ((Math.Abs(x1 - x2) < EPSILON) && (Math.Abs(y1 - y2) < EPSILON))
                return true;
            else
                return false;
        }
    }
}
