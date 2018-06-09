using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using WCAE.Entities;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;
using WCAE.WGeos2D.Funcs;
using WCAE.WMesh2D;
using WCAE.WMesh2D.Funcs;
using WCAE.WMesh2D.Entities;

namespace WCAE.WMesh2D.Funcs
{
    /// <summary>
    /// 使用Triangle划分自由区域的2D网格，可用于划分单通道及内部含有孔的区域
    /// </summary>
    public static class Mesh2D_MeshFree
    {
        #region Single Rim
        public static WMesh2D_Mesh Mesh_SingleRim(ref WEntity2D[] Rim, string Name, ref WMesh2D_Para MP)
        {
            WShapeRim2D Shape = Rim_Output(ref Rim, MP.Path, Name);
            if (Shape == null) return null;
            Poly_SingleRim(Shape.Count, MP.Path, Name);
            WMesh2D_Mesh Mesh = Mesh2D_TriangleMesh.Triangle_Mesh(ref MP, Name);      /////调用
            if (Mesh == null) return null;
            Mesh.Shapes.Add(Shape);
            Mesh.Input_Bound(Shape.Xmin, Shape.Xmax, Shape.Ymin, Shape.Ymax);
            return Mesh;
        }

        public static WElement2D[] Mesh_SingleRim(ref List<int> Nums,ref List<WPoint2D> Ps, ref WMesh2D_Para MP, string Name)
        {
            StreamWriter Sw = new System.IO.StreamWriter(MP.Path + Name + ".rim");
            List<int> Ns = new List<int>();
            int N = 0;
            N = WMFuncs2D.NsID_App(ref Ps, ref  Ns, N);                 /////形成节点编号
            WMFuncs2D.Ns_Out(ref Ps, ref  Ns, false, 0, ref  Sw);
            Sw.Close();
            Poly_SingleRim(Ps.Count, MP.Path, Name);
            WMesh2D_Mesh Mesh = Mesh2D_TriangleMesh.Triangle_Mesh(ref MP, Name);      /////调用
            if (Mesh == null) return null;
            for (int i = 0; i < Mesh.QEs; i++)
            {
                if (Mesh.Elements[i].Kind == 4)
                    Mesh.Elements[i] = new WElement2D(Mesh.Elements[i].Kind,
                              Nums[Mesh.Elements[i].N1], Nums[Mesh.Elements[i].N2],
                              Nums[Mesh.Elements[i].N3], Nums[Mesh.Elements[i].N4]);
                else if (Mesh.Elements[i].Kind == 3)
                    Mesh.Elements[i] = new WElement2D(Mesh.Elements[i].Kind,
                             Nums[Mesh.Elements[i].N1], Nums[Mesh.Elements[i].N2],
                             Nums[Mesh.Elements[i].N3], 0);
            }
            return Mesh.Elements.ToArray();
        }

        //public static int Mesh_SingleRim(ref WEntity2D[] Rim, ref WMesh2D_Para MP, string Name)
        //{
        //    WShapeRim2D Shape = Rim_Output(ref Rim, MP.Path, Name);
        //    if (Shape == null) return -1;
        //    Poly_SingleRim(Shape.Count, MP.Path, Name);
        //    WMesh2D_Mesh Mesh = Mesh2D_TriangleMesh.Triangle_Mesh(ref MP, Name);      /////调用
        //    if (Mesh == null) return -1;
        //    return Shape.Count;
        //}

        private static void Poly_SingleRim(int Q, string Path, string Name)
        {
            StreamWriter Sw = new System.IO.StreamWriter(Path + Name + ".poly");
            StreamReader Sr;
            Sw.WriteLine(Convert.ToString(Q) + "  2 1  1");
            String t;

            string[] t_1;
            Sr = new System.IO.StreamReader(Path + Name + ".rim");
            int N = 0;
            String Num;
            while (Sr.Peek() != -1)
            {
                t_1 = Sr.ReadLine().Split('	');
                Num = t_1[0];
                t_1 = t_1[1].Split(',');
                N++;
                t = Convert.ToString(N) + "  " + t_1[0] + " " + t_1[1] + "  0  " + Num;
                Sw.WriteLine(t);
            }
            Sr.Close();
            Sr.Dispose();
            ////////////////////////////////////////////////////////////////
            Sw.WriteLine(Convert.ToString(Q) + " 1");
            for (int i = 1; i <Q; i++)
            {
                t = Convert.ToString(i) + "  " + Convert.ToString(i) + " " + Convert.ToString(i + 1) + "  0";
                Sw.WriteLine(t);
            }
            t = Convert.ToString(Q) + "  " + Convert.ToString(1) + " " + Convert.ToString(Q) + "  0";
            Sw.WriteLine(t);
            Sw.WriteLine("0");
            Sw.Close();
            Sw.Dispose();

            System.IO.File.Delete(Path + Name + ".rim");
        }
        #endregion

        #region Double Rim
        public static int Mesh_DoubleRim(ref WEntity2D[] Rim_Outer, ref WEntity2D[] Rim_Inner, ref WMesh2D_Para MP, string Name)
        {
            int N = 0;
            int Quan_In = Rim_Output(ref Rim_Inner, MP.Path, Name + "_Inner", N);
            if (Quan_In == -1) return -1;
            N = Quan_In;
            int Quan_Out = Rim_Output(ref Rim_Outer, MP.Path, Name + "_Outer", N);
            if (Quan_Out == -1) return -1;
            N += Quan_Out;
            ///
            double xmin = double.MaxValue;
            double xmax = double.MinValue;
            double ymin = double.MaxValue;
            double ymax = double.MinValue;
            WCurve2D C;
            for (int i = 0; i < Rim_Inner.Length; i++)
            {
                C = (WCurve2D)Rim_Inner[i];
                xmin = Math.Min(xmin, C.Xmin);
                xmax = Math.Max(xmax, C.Xmax);
                ymin = Math.Min(ymin, C.Ymin);
                ymax = Math.Max(ymax, C.Ymax);
            }

            Poly_DoubleRim(Quan_In, Quan_Out, MP.Path, Name, new WPoint2D((xmin + xmax) / 2, (ymin + ymax) / 2));
            Mesh2D_TriangleMesh.Triangle_Mesh(Name, ref MP);      /////调用
            return N;
        }

        public static WMesh2D_Mesh Mesh_DoubleRim(ref WEntity2D[] Rim_Outer, ref WEntity2D[] Rim_Inner, string Name, ref WMesh2D_Para MP)
        {
            int N = 0;
            int Quan_In = Rim_Output(ref Rim_Inner, MP.Path, Name + "_Inner", N);
            if (Quan_In == -1) return null;
            N = Quan_In;
            int Quan_Out = Rim_Output(ref Rim_Outer, MP.Path, Name + "_Outer", N);
            if (Quan_Out == -1) return null;
            N += Quan_Out;
            ///
            double xmin = double.MaxValue;
            double xmax = double.MinValue;
            double ymin = double.MaxValue;
            double ymax = double.MinValue;
            WCurve2D C;
            for (int i = 0; i < Rim_Inner.Length ; i++)
            {
                C = (WCurve2D)Rim_Inner[i];
                xmin = Math.Min(xmin, C.Xmin);
                xmax = Math.Max(xmax, C.Xmax);
                ymin = Math.Min(ymin, C.Ymin);
                ymax = Math.Max(ymax, C.Ymax);
            }

            Poly_DoubleRim(Quan_In, Quan_Out, MP.Path, Name, new WPoint2D((xmin + xmax) / 2, (ymin + ymax) / 2));
            return Mesh2D_TriangleMesh.Triangle_Mesh(ref MP, Name);      /////调用
        }

        private static void Poly_DoubleRim(int Q_In, int Q_Out, string Path, string Name, WPoint2D Pm)
        {
            StreamWriter Sw = new System.IO.StreamWriter(Path + Name + ".poly");
            StreamReader Sr;
            Sw.WriteLine(Convert.ToString(Q_Out + Q_In) + "  2 1  1");
            String t;

            string[] t_1;
            Sr = new System.IO.StreamReader(Path + Name + "_Inner.rim");
            int N = 0;
            String Num;
            while (Sr.Peek() != -1)
            {
                t_1 = Sr.ReadLine().Split('	');
                Num = t_1[0];
                t_1 = t_1[1].Split(',');
                N++;
                t = Convert.ToString(N) + "  " + t_1[0] + " " + t_1[1] + "  0  " + Num;
                Sw.WriteLine(t);
            }
            Sr.Close();


            Sr = new System.IO.StreamReader(Path + Name + "_Outer.rim");
            while (Sr.Peek() != -1)
            {
                t_1 = Sr.ReadLine().Split('	');
                Num = t_1[0];
                t_1 = t_1[1].Split(',');
                N = N + 1;
                t = Convert.ToString(N) + "  " + t_1[0] + " " + t_1[1] + "  0  " + Num;
                Sw.WriteLine(t);
            }
            Sr.Close();
            Sr.Dispose();
            ////////////////////////////////////////////////////////////////
            Sw.WriteLine(Convert.ToString(Q_In + Q_Out) + " 1");
            for (int i = 1; i <= Q_In - 1; i++)
            {
                t = Convert.ToString(i) + "  " + Convert.ToString(i) + " " + Convert.ToString(i + 1) + "  0";
                Sw.WriteLine(t);
            }
            t = Convert.ToString(Q_In) + "  " + Convert.ToString(1) + " " + Convert.ToString(Q_In) + "  0";
            Sw.WriteLine(t);
            for (int i = 1 + Q_In; i <= Q_In + Q_Out - 1; i++)
            {
                t = Convert.ToString(i) + "  " + Convert.ToString(i) + " " + Convert.ToString(i + 1) + "  0";
                Sw.WriteLine(t);
            }
            t = Convert.ToString(Q_In + Q_Out) + "  " + Convert.ToString(Q_In + 1) + " " + Convert.ToString(Q_In + Q_Out) + "  0";
            Sw.WriteLine(t);
            ////////////////////////////////////////////////////////////////
            int Xm = (int)Pm.X;
            int Ym = (int)Pm.Y;
            Sw.WriteLine(1);
            Sw.WriteLine("1 " + Convert.ToString(Xm) + " " + Convert.ToString(Ym));
            Sw.Close();
            Sw.Dispose();

            System.IO.File.Delete(Path + Name + "_Inner.rim");
            System.IO.File.Delete(Path + Name + "_Outer.rim");
        }
        #endregion

        /// 返回值为新的自由节点数量，用于DoubleRim
        private static int Rim_Output(ref WEntity2D[] Rim_Curves, string Path, string Name, int N)
        {
            for (int i = 0; i < Rim_Curves.Length; i++)
                if (((WCurve2D)Rim_Curves[i]).Meshed_Check == false) return -1;   /////如果有线没有Mesh则返回-1

            StreamWriter Sw = new System.IO.StreamWriter(Path + Name + ".rim");
            int[] Sorts = Geos2D_Other.Sort_Curves(Rim_Curves);

            int Num;
            bool Tc;                   /////判断某条线是否首尾颠倒,true为颠倒，false为不颠倒
            WCurve2D C;
            List<WPoint2D> Ps;
            List<int> Ns = new List<int>();
            int QuanI = N;
            ///////处理第一条线///////
            Num = 0;
            C = (WCurve2D)Rim_Curves[Num];
            Ps = new List<WPoint2D>();
            for (int i = 0; i < C.Nodes.Count; i++) Ps.Add(C.Nodes[i]); /////将Curve的Node复制出来至Ps
            Ps.RemoveAt(0);                                             /////去掉起点，因为最后一条线包含该点
            N = WMFuncs2D.NsID_App(ref Ps, ref  Ns, N);                 /////形成节点编号
            WMFuncs2D.Ns_Out(ref Ps, ref  Ns, false, 0, ref  Sw);

            //////处理中间线//////////
            for (int i = 1; i <= Sorts.Length - 1; i++)
            {
                Num = (int)(Math.Floor((double)(Sorts[i] / 2)));
                if (Sorts[i] - 2 * Num == 0)
                    Tc = false;
                else
                    Tc = true;         /////首尾颠倒则为true

                Ps = new List<WPoint2D>();
                C = (WCurve2D)Rim_Curves[Num];
                for (int j = 0; j < C.Nodes.Count; j++) Ps.Add(C.Nodes[j]); /////将Curve的Node复制出来至Ps
                WMFuncs2D.Points_Reverse(ref Ps, Tc);                       /////翻转点集
                Ps.RemoveAt(0);                                             /////去掉起点

                N = WMFuncs2D.NsID_App(ref Ps, ref  Ns, N);                 /////定义节点编号
                WMFuncs2D.Ns_Out(ref Ps, ref  Ns, false, 0, ref  Sw);          /////输出
            }
            Sw.Close();
            return (N - QuanI);
        }

        /// 返回值为自由节点组成的圈圈，用于显示，用于SingleRim
        private static WShapeRim2D Rim_Output(ref WEntity2D[] Rim_Curves, string Path, string Name)
        {
            for (int i = 0; i < Rim_Curves.Length; i++)
                if (((WCurve2D)Rim_Curves[i]).Meshed_Check == false) return null;   /////如果有线没有Mesh则返回-1

            StreamWriter Sw = new System.IO.StreamWriter(Path + Name + ".rim");
            int[] Sorts = Geos2D_Other.Sort_Curves(Rim_Curves);

            int Num;
            bool Tc;                   /////判断某条线是否首尾颠倒,true为颠倒，false为不颠倒
            WCurve2D C;
            List<WPoint2D> Ps;
            List<int> Ns = new List<int>();
            WShapeRim2D Shape = new WShapeRim2D(Color.Black, 1f);
            int N = 0;
            int QuanI = N;
            ///////处理第一条线///////
            Num = 0;
            C = (WCurve2D)Rim_Curves[Num];
            Ps = new List<WPoint2D>();
            for (int i = 0; i < C.Nodes.Count; i++) Ps.Add(C.Nodes[i]); /////将Curve的Node复制出来至Ps
            Ps.RemoveAt(0);                                             /////去掉起点，因为最后一条线包含该点
            N = WMFuncs2D.NsID_App(ref Ps, ref  Ns, N);                 /////形成节点编号
            WMFuncs2D.Ns_Out(ref Ps, ref  Ns, false, 0, ref  Sw);
            Shape.Add_Ps(ref Ps, ref Ns, false, 0);

            //////处理中间线//////////
            for (int i = 1; i <= Sorts.Length - 1; i++)
            {
                Num = (int)(Math.Floor((double)(Sorts[i] / 2)));
                if (Sorts[i] - 2 * Num == 0)
                    Tc = false;
                else
                    Tc = true;         /////首尾颠倒则为true

                Ps = new List<WPoint2D>();
                C = (WCurve2D)Rim_Curves[Num];
                for (int j = 0; j < C.Nodes.Count; j++) Ps.Add(C.Nodes[j]); /////将Curve的Node复制出来至Ps
                WMFuncs2D.Points_Reverse(ref Ps, Tc);                       /////翻转点集
                Ps.RemoveAt(0);                                             /////去掉起点

                N = WMFuncs2D.NsID_App(ref Ps, ref  Ns, N);                 /////定义节点编号
                WMFuncs2D.Ns_Out(ref Ps, ref  Ns, false, 0, ref  Sw);          /////输出
                Shape.Add_Ps(ref Ps, ref Ns, false, 0);
            }
            Sw.Close();
            return Shape;
        }
    }
}
