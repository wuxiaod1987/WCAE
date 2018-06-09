using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TriangleNet;
using TriangleNet.Tools;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;

namespace WCAE.WRenderer
{
    /// Renders a WGeo
    /// <summary>
    /// Renders a WGeo
    /// </summary>
    public class WGeoRenderer
    {
        RenderData data;
        RenderParameter RParas;
        Graphics G;

        List<int>[] RenderSort = new List<int>[6];  /////显示顺序

        public WGeoRenderer(RenderData data, RenderParameter RParas)
        {
            this.data = data;
            this.RParas = RParas;
            for (int i = 0; i < 6; i++) 
                RenderSort[i] = new List<int>();
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render(Graphics g, ScreenTrans zoom)
        {
            this.G = g;
            try
            {
                if (data.Entities.Count == 0 || data.Entities == null) return;
            }
            catch { }
            //////
            RenderPoints(zoom, data.PsList);
            for (int i = 0; i < data.Entities.Count; i++)
                RenderSingleEntity(zoom, data.Entities[i], false);
            if (RParas.SwitchSnap == false) return;  /////如果对象捕捉没有打开则直接返回
            RenderSnapEntities(g, zoom, RParas.SModes);
            RenderSnapPoints(g, zoom, RParas.SModes);
        }

        #region Render Snap
        public void RenderSnapPoints(Graphics g, ScreenTrans zoom, List<SnapMode> SModes)
        {
            try
            {
                if (data.objSnap.Geo2D_Entities.Count == 0 || data.objSnap.Geo2D_Entities == null) return;
            }
            catch { }
            /////
            if (SModes.Contains(SnapMode.Geo2DPoint))
                if (data.objSnap.Geo2D_Entities.Count != 0)
                    for (int i = 0; i < data.objSnap.Geo2D_Entities.Count; i++)
                    {
                        Numt = data.objSnap.Geo2D_Entities[i];
                        p0 = new PointF((float)data.PsList[Numt].X, (float)data.PsList[Numt].Y);
                        p0 = zoom.WorldToScreen(p0.X, p0.Y);
                        G.FillEllipse(RParas.BrushSnapP, p0.X - RParas.RSnapP / 2, p0.Y - RParas.RSnapP / 2, RParas.RSnapP, RParas.RSnapP);

                    }
        }

        public void RenderSnapEntities(Graphics g, ScreenTrans zoom, List<SnapMode> SModes)
        {
            try
            {
                if (data.objSnap.Geo2D_Entities.Count == 0 || data.objSnap.Geo2D_Entities == null) return;
            }
            catch { }
            /////渲染捕捉到的几何体
            if (SModes.Contains(SnapMode.Geo2DEntity))
                if (data.objSnap.Geo2D_Entities.Count != 0)
                    for (int i = 0; i < data.objSnap.Geo2D_Entities.Count; i++)
                        RenderSingleEntity(zoom, data.Entities[data.objSnap.Geo2D_Entities[i]], true);
        }
        #endregion

        private void RenderSingleEntity(ScreenTrans zoom, WEntity2D E, bool SnapCheck)
        {
            switch (E.Kind)
            {
                case GeoKind.Line:
                    RenderLines(zoom, (WLine2D)E, SnapCheck);
                    break;
                case GeoKind.PolyLine:
                    RenderPolyLines(zoom, (WPolyLine2D)E, SnapCheck);
                    break;
                case GeoKind.Circle:
                    RenderCircles(zoom, (WCircle2D)E, SnapCheck);
                    break;
                case GeoKind.DText:
                    RenderText(zoom, (WText2D)E);
                    break;
                default:
                    break;
            }
        }

        #region 临时变量
        PointF p0, p1;
        PointF[] Ps;
        Brush tB;
        Pen tPen;
        int Numt;
        #endregion

        #region Render Entity
        private void RenderPoints(ScreenTrans zoom, WGeos2D_PsList PsList)
        {
            Ps = new PointF[PsList.Count + 1];
            for (int i = 1; i <= PsList.Count; i++)
            {
                if (PsList[i] == null)
                    Ps[i] = new PointF(float.MaxValue, float.MaxValue);
                Ps[i] = new PointF((float)PsList[i].X, (float)PsList[i].Y);
                Ps[i] = zoom.WorldToScreen(Ps[i].X, Ps[i].Y);

                if (RParas.SwitchHP == true && PsList[i].HardCheck==true )
                    G.FillEllipse(RParas.BrushHP, Ps[i].X - RParas.Rhp / 2, Ps[i].Y - RParas.Rhp / 2, RParas.Rhp, RParas.Rhp);
                if (RParas.SwitchSP == true && PsList[i].HardCheck == false)
                    G.FillEllipse(RParas.BrushSP, Ps[i].X - RParas.Rsp / 2, Ps[i].Y - RParas.Rsp / 2, RParas.Rsp, RParas.Rsp);
            }
        }

        private void RenderLines(ScreenTrans zoom, WLine2D Line, bool SnapCheck)
        {
            if (SnapCheck == false)
                tPen = new Pen(Line.Color, Line.LineWidth);
            else
                tPen = RParas.SnapGeo2DPen;
            p0 = Ps[Line.StartPoint.Num];
            p1 = Ps[Line.EndPoint.Num];

            G.DrawLine(tPen, p0, p1);
        }

        private void RenderPolyLines(ScreenTrans zoom, WPolyLine2D PL, bool SnapCheck)
        {
            if (SnapCheck == false)
                tPen = new Pen(PL.Color, PL.LineWidth);
            else
                tPen = RParas.SnapGeo2DPen;

            for (int j = 0; j < PL.Count - 1; j++)
            {
                p0 = Ps[PL[j].Num];
                p1 = Ps[PL[j + 1].Num];

                G.DrawLine(tPen, p0, p1);
            }
        }

        private void RenderCircles(ScreenTrans zoom, WCircle2D Circle, bool SnapCheck)
        {
            if (SnapCheck == false)
                tPen = new Pen(Circle.Color, Circle.LineWidth);
            else
                tPen = RParas.SnapGeo2DPen;
            
            float R;
            p0 = Ps[Circle.Center.Num];

            R = (float)Circle.R;
            R = zoom.WorldToScreen(R);

            G.DrawArc(tPen, new Rectangle((int)(p0.X - R), (int)(p0.Y - R), (int)(R * 2), (int)(R * 2)), 0, 360);
        }

        private void RenderText(ScreenTrans zoom, WText2D Text)
        {
            p0 = Ps[Text.Position.Num];

            float H = (float)Text.Height;
            H = zoom.WorldToScreen(H);

            G.DrawString(Text.Text, new Font("宋体", H), Brushes.White, p0);
        }
        #endregion
    }
}
