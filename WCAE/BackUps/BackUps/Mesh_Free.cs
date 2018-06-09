using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;
using WCAE.WGeos2D.Funcs;
using WCAE.WMesh2D.Funcs;
using WCAE.WMesh2D.IO;
using WCAE.WMesh2D.Entities;
using WCAE.WMesh2D;
using GL.FEM.Mesh;
using GL.FEM;
using GL;

namespace BackUps
{
    /// <summary>
    /// 整面夹丝计算网格划分时，使用共节点方法时，Mesh自由区域的类，因改为Embed方法暂时不用
    /// </summary>
    static class Mesh_Free
    {
        public static void Do_Mesh(ref GLCommon GLC)
        {
            int Num_Start = 1;
            WMesh2D_Mesh Mesh_t;
            if ((int)(GLC.Rims[0].Trace / 100) == 20)
            {
                Mesh_t = Mesh_OuterFree(GLC.Rims[0], GLC.Rims[1], ref GLC, "Free_Out");
                if (Mesh_t != null)
                    GLC.Meshs.Add(Mesh_t);
                Num_Start = 2;
            }

            for (int i = Num_Start; i < GLC.Rims.Count; i++)
            {

                if (GLC.Rims[i].Trace % 10 == 3)
                {
                    Mesh_t = Mesh_InnerFree(GLC.Rims[i], ref GLC, Convert.ToString(i));
                    if (Mesh_t != null)
                        GLC.Meshs.Add(Mesh_t);
                }
            }
        }

        /// <summary>
        /// Mesh最外自由区域
        /// </summary>
        private static WMesh2D_Mesh Mesh_OuterFree(WRim2D Rim_Out, WRim2D Rim_In, ref GLCommon GLC, string Name)
        {
            WMFuncs2D.Mesh_Curves(ref Rim_Out.Curves, GLC.Mesh_Length);  /////Mesh每条线
            WMFuncs2D.Mesh_Curves(ref Rim_In.Curves, GLC.Mesh_Length);   /////Mesh每条线

            WMesh2D_Mesh Mesh = Mesh2D_MeshFree.Mesh_DoubleRim(ref Rim_Out.Curves, ref Rim_In.Curves, Name, ref GLC.MP);
            if (Mesh == null) return null;
            int R = (int)(Rim_Out.Color.R * 0.8);
            int G = (int)(Rim_Out.Color.G * 0.8);
            int B = (int)(Rim_Out.Color.B * 0.8);
            Mesh.Color = Color.FromArgb(255, R, G, B);
            Mesh.Trace = 3;
            return Mesh;
        }

        /// <summary>
        /// Mesh内部区域
        /// </summary>
        private static WMesh2D_Mesh Mesh_InnerFree(WRim2D Rim, ref GLCommon GLC, string Name)
        {
            WMFuncs2D.Mesh_Curves(ref Rim.Curves, GLC.Mesh_Length);  /////Mesh每条线
            WMesh2D_Mesh Mesh = Mesh2D_MeshFree.Mesh_SingleRim(ref Rim.Curves, Name, ref GLC.MP);
            if (Mesh == null) return null;
            int R = (int)(Rim.Color.R * 0.8);
            int G = (int)(Rim.Color.G * 0.8);
            int B = (int)(Rim.Color.B * 0.8);
            Mesh.Color = Color.FromArgb(255, R, G, B);
            Mesh.Trace = 3;
            return Mesh;
        }

        /// <summary>
        /// 在世界中加入线的编号
        /// </summary>
        /// <param name="Curves"></param>
        /// <param name="WGC"></param>
        private static void Add_CurveNum(ref WEntity2D[] Curves, ref WGeometry2D WGC)
        {
            string N;
            WPoint2D S, E;
            int A;
            WPoint2D P = new WPoint2D();
            for (int i = 0; i < Curves.Length; i++)
            {
                N = Convert.ToString(i);
                if (Curves[i].Kind == GeoKind.Line)
                {
                    S = ((WCurve2D)Curves[i]).StartPoint;
                    E = ((WCurve2D)Curves[i]).EndPoint;
                    P = new WPoint2D((S.X + E.X) / 2, (S.Y + E.Y) / 2);
                }
                if (Curves[i].Kind == GeoKind.PolyLine)
                {
                    A = ((WPolyLine2D)Curves[i]).Count / 2;
                    P = ((WPolyLine2D)Curves[i])[A];
                }
                WGC.Add_Geo(new WText2D(N, P, 10));
            }
        }

        private static void Output_Rims(ref WRim2D[] Rims, ref StreamWriter Sw)
        {
            Sw.WriteLine("&&&&&&");
            for (int i = 0; i < Rims.Length; i++)
            {
                Sw.WriteLine("No." + Convert.ToString(i));
                Sw.WriteLine("---------");
                Sw.WriteLine("Smaller:" + Convert.ToString(Rims[i].Smaller.Count));
                Sw.WriteLine("---");
                Sw.WriteLine("Trace:" + Rims[i].Trace.ToString());
                Sw.WriteLine("---");
                for (int j = 0; j < Rims[i].Count; j++)
                    Sw.WriteLine(Rims[i][j]);
                Sw.WriteLine("##");
            }
            Sw.WriteLine("&&&&&&");
        }
        private static void Output_Curves(ref WEntity2D[] Free_Curves, ref StreamWriter Sw)
        {

            for (int i = 0; i < Free_Curves.Length; i++)
            {
                Sw.Write(Convert.ToString(i));
                Sw.Write("\t");
                Sw.Write(Convert.ToString(((WCurve2D)Free_Curves[i]).StartPoint.X) + "," +
                             Convert.ToString(((WCurve2D)Free_Curves[i]).StartPoint.Y));
                Sw.Write("\t");
                Sw.Write(Convert.ToString(((WCurve2D)Free_Curves[i]).EndPoint.X) + "," +
                             Convert.ToString(((WCurve2D)Free_Curves[i]).EndPoint.Y));
                Sw.Write("\n");
            }
            Sw.WriteLine("@@@@@@");
        }
    }
}
