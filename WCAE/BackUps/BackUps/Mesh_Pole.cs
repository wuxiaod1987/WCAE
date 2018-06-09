using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using WCAE.Entities;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;
using WCAE.WGeos2D.Funcs;
using WCAE.WMesh2D;
using WCAE.WMesh2D.Funcs;
using WCAE.WMesh2D.Entities;
using GL.FEM.Mesh;
using GL.FEM;
using GL;

namespace BackUps
{
    /// <summary>
    /// 整面夹丝计算网格划分时，使用共节点方法时，Mesh电极区域的类，因改为Embed方法暂时不用
    /// </summary>
    static class Mesh_Pole
    {
        public static void Do_Mesh(ref GLCommon GLC)
        {
            /////电势处理
            WEntity2D[] Ls = Geos2D_Selection.SelectLines(GLC.Layer_Pole, GLC.Color_PotentL, ref GLC.WGC); /////找到所有零电势的圆
            WEntity2D[] Hs = Geos2D_Selection.SelectLines(GLC.Layer_Pole, GLC.Color_PotentH, ref GLC.WGC); /////找到所有正电势的圆
            List<WPoint2D>[] Ptts = new List<WPoint2D>[2];        /////零电势及正电势的点集位置
            Ptts[0] = new List<WPoint2D>();
            for (int i = 0; i < Ls.Length; i++)
                Ptts[0].Add(((WCircle2D)Ls[i]).Center);
            Ptts[1] = new List<WPoint2D>();
            for (int i = 0; i < Ls.Length; i++)
                Ptts[1].Add(((WCircle2D)Hs[i]).Center);
            /////
            WMesh2D_Mesh Mesh_t;
            for (int i = 0; i < GLC.Rims.Count; i++)    /////GLC.Rims.Count
            {
                if (GLC.Rims[i].Trace >= 1000) continue;
                if (GLC.Rims[i].Trace % 10 == 2)
                {
                    Mesh_t = Mesh_SinglePole(GLC.Rims[i], ref GLC, ref Ptts, Convert.ToString(i));
                    if (Mesh_t != null)
                        GLC.Meshs.Add(Mesh_t);
                }
            }
        }

        private static WMesh2D_Mesh Mesh_SinglePole(WRim2D Rim, ref GLCommon GLC, ref List<WPoint2D>[] Ptts, string Name)
        {
            WMFuncs2D.Mesh_Curves(ref Rim.Curves, GLC.Mesh_Length);  /////Mesh每条线
            WMesh2D_Mesh Mesh = Mesh2D_MeshFree.Mesh_SingleRim(ref Rim.Curves, Name, ref GLC.MP);
            if (Mesh == null) return null;
            int R = (int)(Rim.Color.R * 0.8);
            int G = (int)(Rim.Color.G * 0.8);
            int B = (int)(Rim.Color.B * 0.8);
            Mesh.Color = Color.FromArgb(255, R, G, B);
            Mesh.Trace = 2;
            PotentNs_App(ref Mesh.Nodes, ref Ptts, GLC.Potent_Lmax);
            int Quan_E = Mesh.QEs;
            for (int i = 0; i < Quan_E; i++)
                Mesh.Elements.Add(new WElement2D(Mesh.Elements[i].Kind * (-1),
                                                 Mesh.Elements[i].N1,
                                                 Mesh.Elements[i].N2,
                                                 Mesh.Elements[i].N3,
                                                 Mesh.Elements[i].N4));
            return Mesh;
        }

        private static void Out_toMesh(ref List<int>[] Rims, ref WMesh2D_Para MP, ref WEntity2D[] Poles, ref WMesh2D_Mesh Mesh)
        {
            WEntity2D[] Rim_Curves;
            WMesh2D_Mesh[] Meshs_t = new WMesh2D_Mesh[Rims.Length];
            for (int i = 0; i < Rims.Length; i++)
            {
                Rim_Curves = new WEntity2D[Rims[i].Count];
                for (int j = 0; j < Rims[i].Count; j++)
                    Rim_Curves[j] = (WEntity2D)Poles[Rims[i][j]];
                Meshs_t[i] = Mesh2D_MeshFree.Mesh_SingleRim(ref Rim_Curves, Convert.ToString(i), ref MP);
            }

            Mesh = Meshs_t[0];
            for (int i = 1; i < Rims.Length; i++)
                Mesh = Mesh2D_Merge_Mesh2Mesh.Mesh_Combine(Mesh, Meshs_t[i], "Pole");
            Meshs_t = null;

        }

        private static void PotentNs_App(ref List<WNode2D> Ns, ref List<WPoint2D>[] Ptts, double Potent_Lmax)
        {
            for (int i = 0; i < Ns.Count; i++)
            {
                /////低电势节点的标记
                for (int j = 0; j <= Ptts[0].Count - 1; j++)
                    if (Math.Abs(Ns[i].X - Ptts[0][j].X) < Potent_Lmax && Math.Abs(Ns[i].Y - Ptts[0][j].Y) < Potent_Lmax)
                        Ns[i] = new WNode2D(Ns[i].Num, Ns[i].X, Ns[i].Y, -2000);
                //////高电势节点的标记
                for (int j = 0; j <= Ptts[1].Count - 1; j++)
                    if (Math.Abs(Ns[i].X - Ptts[1][j].X) < Potent_Lmax && Math.Abs(Ns[i].Y - Ptts[1][j].Y) < Potent_Lmax)
                        Ns[i] = new WNode2D(Ns[i].Num, Ns[i].X, Ns[i].Y, 2000);
            }
        }
    }
}
