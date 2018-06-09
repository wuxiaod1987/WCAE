using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TriangleNet;
using TriangleNet.Tools;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;
using WCAE.WMesh2D;
using WCAE.WMesh2D.Entities;

namespace WCAE.WRenderer
{
    /// Renders a WGeo
    /// <summary>
    /// Renders a WGeo
    /// </summary>
    public class WMeshRenderer
    {
        RenderData data;
        
        public WMeshRenderer(ref RenderData data)
        {
            this.data = data;
        }

        public void Render(Graphics g, ScreenTrans zoom, RenderParameter RParas)
        {
            if (RParas.Show_WMesh2D == false) return;
            try
            {
                if (data.Meshs == null || data.Meshs.Count == 0) return;
            }
            catch { }
            /////
            for (int i = 0; i < data.Meshs.Count; i++)
                RenderSingleMesh(g, zoom, RParas, data.Meshs[i]);
        }

        public void RenderSingleMesh(Graphics g, ScreenTrans zoom, RenderParameter RParas, WMesh2D_Mesh Mesh)
        {
            PointF P;
            PointF[] Ps;
            
            Brush tB = new SolidBrush(Mesh.Color);
            
            int Num;
            int Kind;
            /////节点转换
            PointF[] Ns = new PointF[Mesh.QNs + 1];
            for (int i = 1; i <= Mesh.QNs; i++)
            {
                Ns[i] = new PointF((float)Mesh.Nodes[i].X, (float)Mesh.Nodes[i].Y);
                Ns[i] = zoom.WorldToScreen(Ns[i].X, Ns[i].Y);
            }
            /////使用Shape输出
            bool Check_Fill = false;
            //if (Mesh.Shapes != null && Mesh.Shapes.Count != 0)
            //{
            //    for (int i = 0; i < Mesh.Shapes.Count; i++)
            //    {
            //        Ps = new PointF[Mesh.Shapes[i].Count];
            //        for (int j = 0; j < Mesh.Shapes[i].Count; j++)
            //        {
            //            Ps[j] = new PointF((float)Mesh.Shapes[i][j].X, (float)Mesh.Shapes[i][j].Y);
            //            Ps[j] = zoom.WorldToScreen(Ps[j].X, Ps[j].Y);
            //        }
            //        g.FillPolygon(tB, Ps);
            //    }
            //    Check_Fill = true;
            //}
            /////
            bool Check_Frame = false;
            if (Mesh.NsPairs != null && Mesh.NsPairs.Length > 0)
            {
                Ps = new PointF[2];
                for (int i = 0; i < Mesh.NsPairs.Length; i++)
                    g.DrawLine(RParas.PenES, Ns[Mesh.NsPairs[i].N1], Ns[Mesh.NsPairs[i].N2]);
                Check_Frame = true;
            }
            /////单元输出
            if (Check_Fill == false || Check_Frame == false)
            {
                for (int i = 0; i < Mesh.QEs; i++)
                {
                    Kind = Mesh.Elements[i].Kind;
                    if (Kind < 3)
                        continue;
                    /////
                    Ps = new PointF[Kind + 1];

                    Num = Mesh.Elements[i].N1;
                    if (Num < 0) continue;
                    Ps[0] = Ns[Num];
                    Num = Mesh.Elements[i].N2;
                    if (Num < 0) continue;
                    Ps[1] = Ns[Num];
                    Num = Mesh.Elements[i].N3;
                    if (Num < 0) continue;
                    Ps[2] = Ns[Num];
                    if (Kind == 4)
                    {
                        Num = Mesh.Elements[i].N4;
                        if (Num < 0) continue;
                        Ps[3] = Ns[Num];
                    }
                    Ps[Kind] = Ps[0];
                    /////
                    if (Check_Fill == false)
                        g.FillPolygon(tB, Ps);
                    if( Check_Frame ==false )
                        g.DrawPolygon(RParas.PenES, Ps);
                }
            }
            /////
            for (int i = 0; i < Mesh.Elements.Count; i++)
            {
                Kind = Mesh.Elements[i].Kind;
                if (Kind == 2)
                {
                    Ps = new PointF[2];
                    Num = Mesh.Elements[i].N1;
                    Ps[0] = Ns[Num];
                    Num = Mesh.Elements[i].N2;
                    Ps[1] = Ns[Num];
                    g.DrawLine(RParas.PenEB, Ps[0], Ps[1]);
                    continue;
                }
            }

            if (RParas.SwitchN == false)
                return;
            for (int i = 1; i < Mesh.Nodes.Count; i++)
            {
                P = new PointF((float)Mesh.Nodes[i].X, (float)Mesh.Nodes[i].Y);
                P = zoom.WorldToScreen(P.X, P.Y);
                g.FillEllipse(RParas.BrushN, P.X - RParas.Rn / 2, P.Y - RParas.Rn / 2, RParas.Rn, RParas.Rn);
            }
        }
    }
}
