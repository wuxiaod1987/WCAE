using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using TriangleNet;
using TriangleNet.IO;
using TriangleNet.Geometry;
using TriangleNet.Data;
using WCAE.Entities;
using WCAE.WGeos2D;
using WCAE.WMesh2D;
using WCAE.WMesh2D.Funcs;
using WCAE.WMesh2D.Entities;

namespace WCAE.WMesh2D.Funcs
{
    
    static class Mesh2D_TriangleMesh
    {
        /// <summary>
        /// 从一个Poly文件，Mesh后形成Mesh2D类
        /// </summary>
        /// <param name="MP"></param>
        /// <param name="Name">简短文件名</param>
        /// <returns></returns>
        public static WMesh2D_Mesh Triangle_Mesh(ref WMesh2D_Para MP, string Name)
        {
            Tiangle_Call New_Call = new Tiangle_Call(MP.Path + Name + ".poly", ref MP);
            #region 多线程
            ThreadStart start = new ThreadStart(New_Call.Triangulate_Refine);
            Thread thread = new Thread(start);
            thread.Name = "Tiangle_Call";
            thread.Start();

            while (true)
                if (thread.ThreadState == System.Threading.ThreadState.Stopped)
                    break;
            #endregion
            #region 单线程
            //New_Call.Triangulate_Refine();
            #endregion

            bool C = New_Call.Out;
            if (C == false)
                return null;
            Mesh mesh = New_Call.Mesh;

            Node[] Ns = new Node[0];          /////注意该数组是从1开始的，目的是为了Element使用方便
            Element[] Es = new Element[0];

            FileWriter.WritePoly(mesh, MP.Path + Name + "_.poly");
            FileWriter.Trans_Mesh(mesh, ref Ns, ref Es);
            Mesh2D_TriangleComb.Elements_Combine(ref Ns, ref Es, ref MP);
            /////
            WMesh2D_Mesh Mesh = new WMesh2D_Mesh(Name);
            Mesh.Q_FreeNs = MP.QN_Ini;
            for (int i = 1; i < Ns.Length; i++)
                Mesh.Nodes.Add(new WNode2D(i, Ns[i].x, Ns[i].y, 0));
            for (int i = 0; i < Es.Length; i++)
            {
                if (Es[i].Valid == false)
                    continue;
                Mesh.Elements.Add(new WElement2D(Es[i].Kind, Es[i].N1, Es[i].N2, Es[i].N3, Es[i].N4));
            }
            File.Delete(MP.Path + Name + ".poly");
            File.Delete(MP.Path + Name + "_.poly");
            Ns = null;
            Es = null;
            mesh = null;

            return Mesh;
        }

        /// <summary>
        /// 从一个Poly文件，Mesh后形成mesh文件
        /// </summary>
        /// <param name="Name">poly文件的完整文件名</param>
        /// <param name="MP"></param>
        /// <returns></returns>
        public static bool Triangle_Mesh(string Name, ref WMesh2D_Para MP)
        {
            Tiangle_Call New_Call = new Tiangle_Call(MP.Path + Name + ".poly", ref MP);
            #region 多线程
            ThreadStart start = new ThreadStart(New_Call.Triangulate_Refine);
            Thread thread = new Thread(start);
            thread.Name = "Tiangle_Call";
            thread.Start();

            while (true)
                if (thread.ThreadState == System.Threading.ThreadState.Stopped)
                    break;
            #endregion
            #region 单线程
            //New_Call.Triangulate_Refine();
            #endregion
            bool C = New_Call.Out;
            if (C == false)
                return false;

            Mesh mesh = New_Call.Mesh;

            Node[] Ns = new Node[0];          /////注意该数组是从1开始的，目的是为了Element使用方便
            Element[] Es = new Element[0];

            FileWriter.WritePoly(mesh, MP.Path + Name + "_.poly");
            FileWriter.Trans_Mesh(mesh, ref Ns, ref Es);
            Mesh2D_TriangleComb.Elements_Combine(ref Ns, ref Es, ref MP);

            Mesh_Output(MP.Path, Name, ref Ns, ref Es, MP.QN_Ini);
            File.Delete(MP.Path + Name + ".poly");
            File.Delete(MP.Path + Name + "_.poly");
            return true;
        }

        private static void MeshNum_Update(ref Node[] Ns, ref Element[] Es, ref WMesh2D_Para MP)
        {
            int[] Ns_U = new int[Ns.Length];
            Ns_U[0] = 0;
            for (int i = 1; i <= MP.QN_Ini; i++)
                Ns_U[i] = Ns[i].ID;

            for (int i = MP.QN_Ini + 1; i < Ns.Length; i++)
            {
                Ns[i].ID = MP.Num_NodeLatest + i - MP.QN_Ini;
                Ns_U[i] = Ns[i].ID;
            }

            for (int i = 0; i < Es.Length; i++)
            {
                Es[i].ID = MP.Num_EleLatest + i + 1;
                //Es[i].N1t = Ns_U[Es[i].N1];
                //Es[i].N2t = Ns_U[Es[i].N2];
                //Es[i].N3t = Ns_U[Es[i].N3];
                //Es[i].N4t = Ns_U[Es[i].N4];
            }
        }

        private static void Mesh_Output(string Path, string Name, ref Node[] Ns, ref Element[] Es, int Q_ini)
        {
            StreamWriter sw = new StreamWriter(Path + Name + ".mesh");
            sw.WriteLine("Parameters of the Mesh:");
            sw.WriteLine(Convert.ToString(Q_ini)); /////原有的节点数量，也可以认为是边界节点的数量
            /////尽管可能会有多，因为有纯粹自由边
            sw.WriteLine("  Following Lines are Nodes");
            sw.WriteLine("______________________________");

            string t;
            for (int i = 1; i < Ns.Length; i++)
            {
                t = Convert.ToString(i) + "	" +
                    Convert.ToString(Math.Round(Ns[i].x, WGeos2D_Paras.Round)) + "," +
                    Convert.ToString(Math.Round(Ns[i].y, WGeos2D_Paras.Round) + ",0");
                sw.WriteLine(t);
            }
            /////////////////////////////////////////
            sw.WriteLine("");
            sw.WriteLine("  Following Lines are Elements");
            sw.WriteLine("______________________________");
            for (int i = 0; i < Es.Length; i++)
            {
                if (Es[i].Valid == false)
                    continue;
                t = "";
                t = Convert.ToString(Es[i].Kind) + "	" + Convert.ToString(Es[i].N1) + "," + Convert.ToString(Es[i].N2) + "," + Convert.ToString(Es[i].N3) + "," + Convert.ToString(Es[i].N4);
                sw.WriteLine(t);
            }
            sw.Close();
        }
    }

    /// <summary>
    /// 调用Triangle进行计算
    /// </summary>
    class Tiangle_Call
    {
        Mesh _mesh;
        string File_Name;
        WMesh2D_Para MP;
        public bool Out;
        public Mesh Mesh { get { return this._mesh; } }

        public Tiangle_Call(string File_Name, ref WMesh2D_Para MP)
        {
            this.File_Name = File_Name;
            this.MP = MP;
            Out = false;
        }

        #region "Core Triangle Call"
        /// <summary>
        /// 调用该函数进行Triangle网格划分
        /// </summary>
        public void Triangulate_Refine()
        {
            Out = false;
            _mesh = new Mesh();
            InputGeometry input = FileReader.ReadPolyFile(File_Name);
            MP.QN_Ini = input.Count;

            if (MP.MinAngle_Tri > 0 && MP.MinAngle_Tri < 180)
                _mesh.Behavior.MinAngle = MP.MinAngle_Tri;
            try
            {
                _mesh.Triangulate(input);
                Out = true;
            }
            catch { }
            if (Out == false)
                return;
            else
                Out = Refine(MP.MaxArea_Tri, MP.MaxAngle_Tri, MP.MinAngle_Tri, ref _mesh);
            
            return;
        }

        private static bool Refine(double MaxArea, double MaxAngle, double MinAngle, ref Mesh mesh)
        {
            bool Out = false;
            if (mesh == null) return false;

            if (MaxArea > 0)
                mesh.Behavior.MaxArea = MaxArea;

            if (MinAngle > 0 && MinAngle < 180)
                mesh.Behavior.MinAngle = MinAngle;

            if (MinAngle < 180 && MinAngle > 0)
                mesh.Behavior.MaxAngle = MaxAngle;

            try
            {
                mesh.Refine();
                Out = true;
            }
            catch (System.Exception ex) { }
            return Out;
        }
        #endregion
    }
}