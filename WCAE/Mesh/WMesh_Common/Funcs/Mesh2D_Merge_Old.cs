using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WCAE.WMesh2D.Funcs
{
    /// <summary>
    /// 文件合并，该类扫描所有节点，效率奇低，不推荐使用
    /// </summary>
    static class Mesh2D_Merge_Old
    {
        public static void MeshFile_Combine(string Path, string Mesh1, string Mesh2, string Mesh_Out, double EPSILON)
        {
            double[] xs1 = new double[0];
            double[] ys1 = new double[0];
            double[] xs2 = new double[0];
            double[] ys2 = new double[0];
            int[] Ns2 = new int[0];
            bool[] Cs2 = new bool[0];
            int N1 = MeshFile_Read_Mesh1(Path, Mesh1, ref  xs1, ref ys1);
            int N2 = MeshFile_Read_Mesh2(Path, Mesh2, ref  xs2, ref ys2, ref Ns2, ref Cs2);
            int[] Ns2_2 = new int[N2 + 1]; ///用于存储转换后的节点编号
            Ns2_2[0] = 0;
            for (int i = 1; i <= N1; i++)
                for (int j = 1; j <= N2; j++)
                    if (Near_Check(xs1[i], ys1[i], xs2[j], ys2[j], EPSILON) == true)
                    {
                        Cs2[j] = true;
                        Ns2_2[j] = i;
                        break;
                    }
            ///Mesh2节点编号转换
            
            int N2_2 = N1;
            for (int j = 1; j <= N2; j++)
                if (Cs2[j] == false)
                {
                    N2_2++;
                    Ns2_2[j] = N2_2;
                }
            ///////节点输出
            StreamWriter sw = new StreamWriter(Path + Mesh_Out + ".mesh");
            sw.WriteLine("Parameters of the Mesh:");
            sw.WriteLine("0");
            sw.WriteLine("  Following Lines are Nodes");
            sw.WriteLine("______________________________");
            for (int i = 1; i <= N1; i++)
                sw.WriteLine(Convert.ToString(i) + "	" + Convert.ToString(xs1[i]) + "," + Convert.ToString(ys1[i]));
            for (int i = 1; i <= N2; i++)
                if (Cs2[i] == false)
                    sw.WriteLine(Convert.ToString(Ns2_2[i]) + "	" + Convert.ToString(xs2[i]) + "," + Convert.ToString(ys2[i]));
            sw.WriteLine("");
            sw.WriteLine("  Following Lines are Elements");
            sw.WriteLine("______________________________");

            ///Mesh1单元读取
            StreamReader sr = new StreamReader(Path + Mesh1 + ".mesh");
            string t = sr.ReadLine();
            while (t != "")
                t = sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            while (sr.Peek() != -1)
                sw.WriteLine(sr.ReadLine());
            sr.Close();
            ///Mesh2d单元读取
            sr = new StreamReader(Path + Mesh2 + ".mesh");
            t = sr.ReadLine();
            string[] t_1;
            string Kind;
            while (t != "")
                t = sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            while (sr.Peek() != -1)
            {
                t_1 = sr.ReadLine().Split('	');
                Kind = t_1[0];
                t_1 = t_1[1].Split(',');
                t_1[0] = Convert.ToString(Ns2_2[Convert.ToInt32(t_1[0])]);
                t_1[1] = Convert.ToString(Ns2_2[Convert.ToInt32(t_1[1])]);
                t_1[2] = Convert.ToString(Ns2_2[Convert.ToInt32(t_1[2])]);
                t_1[3] = Convert.ToString(Ns2_2[Convert.ToInt32(t_1[3])]);

                sw.WriteLine(Kind + "	" + t_1[0] + "," + t_1[1] + "," + t_1[2] + "," + t_1[3]);
            }
            sw.Close();
            sr.Close();
            sw.Dispose();
            sr.Dispose();
        }

        private static int MeshFile_Read_Mesh2(string Path, string Name, ref double[] xs, ref double[] ys, ref int[] Ns, ref bool[] Cs)
        {
            StreamReader sr = new StreamReader(Path + Name + ".mesh");
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            string t = sr.ReadLine();
            string[] t_1;
            int N = 0;
            while (t != "")
            {
                N++;
                Array.Resize<int>(ref Ns, N + 1);
                Array.Resize<double>(ref xs, N + 1);
                Array.Resize<double>(ref ys, N + 1);
                Array.Resize<bool>(ref Cs, N + 1);
                t_1 = t.Split('	');
                t_1 = t_1[1].Split(',');
                Ns[N] = N;
                xs[N] = Convert.ToDouble(t_1[0]);
                ys[N] = Convert.ToDouble(t_1[1]);
                Cs[N] = false;
                t = sr.ReadLine();
            }
            sr.Close();
            sr.Dispose();
            return N;
        }

        private static int MeshFile_Read_Mesh1(string Path, string Name, ref double[] xs, ref double[] ys)
        {
            StreamReader sr = new StreamReader(Path + Name + ".mesh");
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            string t = sr.ReadLine();
            string[] t_1;
            int N = 0;
            while (t != "")
            {
                N++;
                Array.Resize<double>(ref xs, N + 1);
                Array.Resize<double>(ref ys, N + 1);
                t_1 = t.Split('	');
                t_1 = t_1[1].Split(',');
                xs[N] = Convert.ToDouble(t_1[0]);
                ys[N] = Convert.ToDouble(t_1[1]);
                t = sr.ReadLine();
            }
            sr.Close();
            sr.Dispose();
            return N;
        }

        private static bool Near_Check(double x1,double y1,double x2,double y2, double EPSILON)
        {
            if ((Math.Abs(x1 - x2) < EPSILON) && (Math.Abs(y1 - y2) < EPSILON))
                return true;
            else
                return false;
        }
    }
}
