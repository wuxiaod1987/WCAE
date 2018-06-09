using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Text;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;

namespace WCAE.WRenderer
{
    public class WGeoRimRenderer
    {
        RenderData data;

        public WGeoRimRenderer(RenderData data)
        {
            this.data = data;
        }
        int C = 0;
        public void Render(Graphics g, ScreenTrans zoom, RenderParameter RParas)
        {
            if (RParas.Show_WRim2D == false) return;
            try
            {
                if (data.Rims == null || data.Rims.Count == 0) return;
            }
            catch { }
            /////
            for (int i = 0; i < data.Rims.Count; i++)
                RenderRim(g, zoom, RParas, data.Rims[i]);
            if (RParas.SwitchSnap == false) return;  /////如果对象捕捉没有打开则直接返回
            RenderSnap(g, zoom, RParas, RParas.SModes);
        }

        public void RenderSnap(Graphics g, ScreenTrans zoom, RenderParameter RParas, List<SnapMode> SModes)
        {
            /////渲染捕捉到的几何体
            if (!(SModes.Contains(SnapMode.Geo2DRim))) return;
            try
            {
                if (data.objSnap.Geo2D_Rims == null || data.objSnap.Geo2D_Rims.Count == 0) return;
            }
            catch { }
            /////
            for (int i = 0; i < data.objSnap.Geo2D_Rims.Count; i++)
            {
                RimT = data.Rims[data.objSnap.Geo2D_Rims[i]];
                if (RimT.Count == 0) continue;
                Ps = new PointF[RimT.Shape.Count];
                for (int j = 0; j < RimT.Shape.Count; j++)
                    Ps[j] = zoom.WorldToScreen((float)(RimT.Shape[j].X), (float)(RimT.Shape[j].Y));
                g.DrawPolygon(RParas.SnapGeo2DPen, Ps);
            }
        }

        #region 临时变量
        PointF[] Ps;
        Brush tB;
        Pen tPen;
        WRim2D RimT;
        #endregion

        private void RenderRim(Graphics g, ScreenTrans zoom, RenderParameter RParas, WRim2D Rim)
        {
            tB = new SolidBrush(Rim.Color);
            tPen = new Pen(Rim.Color_Shape, 2);
            Ps = new PointF[Rim.Shape.Count];
            for (int i = 0; i < Rim.Shape.Count; i++)
                Ps[i] = zoom.WorldToScreen((float)(Rim.Shape[i].X), (float)(Rim.Shape[i].Y));
            g.FillPolygon(tB, Ps);
            g.DrawPolygon(tPen, Ps);
        }

        private void RenderHighLights(Graphics g, ScreenTrans zoom, RenderParameter RParas, ref List<WShapeRim2D> HighLights)
        {
            if (HighLights == null || HighLights.Count == 0) return;
            for (int i = 0; i < HighLights.Count; i++)
            {
                if (HighLights[i].Count == 0) continue;
                Ps = new PointF[HighLights[i].Count];
                for (int j = 0; j < HighLights[i].Count; j++)
                    Ps[j] = zoom.WorldToScreen((float)(HighLights[i][j].X), (float)(HighLights[i][j].Y));
                g.DrawPolygon(RParas.SnapGeo2DPen, Ps);
            }
        }
    }
}
