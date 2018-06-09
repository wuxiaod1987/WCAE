using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WCAE.WMesh2D.Funcs
{
    /// <summary>
    /// 网格文件合并，只扫描自由节点，效率较高
    /// </summary>
    public static class Mesh2D_Merge_File2File
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
        /// <param name="Path"></param>
        /// <param name="Mesh1"></param>
        /// <param name="Mesh2"></param>
        /// <param name="Mesh_Out"></param>
        /// <param name="EPSILON"></param>
        public static void MeshFile_Combine(string Path, string Mesh1, string Mesh2, string Mesh_Out, double EPSILON)
        {
            Initial();
            StreamReader sr1 = new StreamReader(Path + Mesh1 + ".mesh");    /////用以Mesh1的输入，通用变量，在各个函数中用Ref进行传递
            StreamReader sr2 = new StreamReader(Path + Mesh2 + ".mesh");    /////用以Mesh2的输入，通用变量，在各个函数中用Ref进行传递
            StreamWriter sw = new StreamWriter(Path + Mesh_Out + ".mesh");  /////用以文件输出，通用变量，在各个函数中用Ref进行传递
            FreeNodes_Combine(EPSILON, ref sw, ref sr1, ref sr2);           /////处理自由节点，形成两个Mesh的自由节点的新的编号

            Num = MeshFile_ONodesRead(ref Ns1, ref sr1, ref sw, Num);
            Num = MeshFile_ONodesRead(ref Ns2, ref sr2, ref sw, Num);
            sw.WriteLine("");
            sw.WriteLine("  Following Lines are Elements");
            sw.WriteLine("______________________________");

            MeshFile_ElementRead(ref Ns1, ref sr1, ref sw);
            MeshFile_ElementRead(ref Ns2, ref sr2, ref sw);
            sw.Close();

            Ns1 = new int[0];
            Ns2 = new int[0];
            sr1.Close();
            sr2.Close();
            sw.Dispose();
            sr1.Dispose();
            sr2.Dispose();
        }

        /// <summary>
        /// 读取单元，转换单元编号，输出
        /// </summary>
        /// <param name="Ns"></param>
        /// <param name="sr"></param>
        /// <param name="sw"></param>
        private static void MeshFile_ElementRead(ref int[] Ns, ref StreamReader sr, ref StreamWriter sw)
        {
            string[] t_1;
            string t;
            int N;
            sr.ReadLine();
            sr.ReadLine();
            while (sr.Peek() != -1)
            {
                t_1 = sr.ReadLine().Split('	');
                t = t_1[0] + "	";
                t_1 = t_1[1].Split(',');
                N = Convert.ToInt32(t_1[0]);
                N = Ns[N];
                t += Convert.ToString(N) + ",";
                N = Convert.ToInt32(t_1[1]);
                N = Ns[N];
                t += Convert.ToString(N) + ",";
                N = Convert.ToInt32(t_1[2]);
                N = Ns[N];
                t += Convert.ToString(N) + ",";
                N = Convert.ToInt32(t_1[3]);
                N = Ns[N];
                t += Convert.ToString(N);
                sw.WriteLine(t);
            }
        }

        /// <summary>
        /// 读取剩余节点并直接输出
        /// </summary>
        /// <param name="Ns">用Ref传递的节点编号数组，该数组会被逐渐扩充</param>
        /// <param name="sr">读取文件工具</param>
        /// <param name="sw">写文件工具</param>
        /// <param name="Num">用以记录节点数量的变量，输出值也为该参数</param>
        private static int MeshFile_ONodesRead(ref int[] Ns, ref StreamReader sr, ref StreamWriter sw, int Num)
        {
            string[] t_1;
            string t = sr.ReadLine();
            int Quan = Ns.Length;
            while (t != "")
            {
                Quan++;
                Array.Resize<int>(ref Ns, Quan);
                t_1 = t.Split('	');
                Num++;
                Ns[Quan - 1] = Num;
                sw.WriteLine(Convert.ToString(Num) + "	" + t_1[1]);
                t = sr.ReadLine();
            }
            return Num;
        }

        private static void FreeNodes_Combine(double EPSILON, ref StreamWriter sw, ref StreamReader sr_M1, ref StreamReader sr_M2)
        {
            double[] Fxs1 = new double[0];
            double[] Fys1 = new double[0];
            double[] Fxs2 = new double[0];
            double[] Fys2 = new double[0];
            int[] Fzs1 = new int[0];       /////不用于计算，用于标记电势加载点
            int[] Fzs2 = new int[0];       /////用于标记电势加载点

            QF1 = MeshFile_FreeRead(ref Fxs1, ref Fys1, ref Fzs1, ref sr_M1);
            QF2 = MeshFile_FreeRead(ref Fxs2, ref Fys2, ref Fzs2, ref sr_M2);

            Array.Resize<int>(ref Ns1, QF1 + 1);
            Array.Resize<int>(ref Ns2, QF2 + 1);
            for (int i = 0; i <= QF1; i++)
                Ns1[i] = 0;
            for (int i = 0; i <= QF2; i++)
                Ns2[i] = 0;

            for (int i = 1; i <= QF1; i++)
                for (int j = 1; j <= QF2; j++)
                    if (Near_Check(Fxs1[i], Fys1[i], Fxs2[j], Fys2[j], EPSILON) == true)
                    {
                        Ns1[i] = j * (-1);  /////用负数表示重合节点的编号
                        Ns2[j] = i * (-1);  /////用正数表示输出时的节点编号
                        Nc++;
                    }

            /////开始自由节点的输出
            sw.WriteLine("Parameters of the Mesh:");
            //sw.WriteLine(QF1 + QF2 - 2 * Nc);
            sw.WriteLine(QF1 + QF2 - Nc);
            sw.WriteLine("  Following Lines are Nodes");
            sw.WriteLine("______________________________");
            ///输出Mesh1的自由节点
            for (int i = 1; i <= QF1; i++)
                if (Ns1[i] == 0)
                {
                    Num++;
                    sw.WriteLine(Convert.ToString(Num) + "	" + 
                                 Convert.ToString(Fxs1[i]) + "," + 
                                 Convert.ToString(Fys1[i]) + "," + 
                                 Convert.ToString(Fzs1[i]));
                    Ns1[i] = Num;
                }
            ///输出Mesh2的自由节点
            for (int i = 1; i <= QF2; i++)
                if (Ns2[i] == 0)
                {
                    Num++;
                    sw.WriteLine(Convert.ToString(Num) + "	" +
                                 Convert.ToString(Fxs2[i]) + "," +
                                 Convert.ToString(Fys2[i]) + "," + 
                                 Convert.ToString(Fzs2[i]));
                    Ns2[i] = Num;
                }

            ///////////////////////
            ///处理重复节点
            string t = "";
            for (int i = 1; i <= QF1; i++)
                if (Ns1[i] < 0)
                {
                    Num++;
                    if (Fzs2[Ns1[i] * (-1)] != 0)
                        t = Convert.ToString(Fzs2[Ns1[i] * (-1)]);
                    else 
                        t=Convert.ToString(Fzs1[i]);

                    sw.WriteLine(Convert.ToString(Num) + "	" +
                                 Convert.ToString(Fxs1[i]) + "," +
                                 Convert.ToString(Fys1[i]) + "," + t);
                    Ns2[Ns1[i] * (-1)] = Num;  ///把Mesh2中相应的重复节点标上编号
                    Ns1[i] = Num;
                }
        }

        /// 读取文件的自由节点
        private static int MeshFile_FreeRead(ref double[] Fxs, ref double[] Fys, ref int[] Fzs, ref StreamReader sr)
        {
            sr.ReadLine();
            int Quan = Convert.ToInt32(sr.ReadLine());
            sr.ReadLine();
            sr.ReadLine();

            Array.Resize<double>(ref Fxs, Quan + 1);
            Array.Resize<double>(ref Fys, Quan + 1);
            Array.Resize<int>(ref Fzs, Quan + 1);

            string[] t_1;
            for (int i = 1; i <= Quan; i++)
            {
                t_1 = sr.ReadLine().Split('	');
                t_1 = t_1[1].Split(',');
                Fxs[i] = Convert.ToDouble(t_1[0]);
                Fys[i] = Convert.ToDouble(t_1[1]); 
                Fzs[i] = Convert.ToInt16(t_1[2]);
            }
            return Quan;
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
