using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCAE.WGeos2D.Entities;
using System.IO;

namespace WCAE.WGeos2D
{
    public class WGeometry2D
    {
        #region "几何实例"
        public List<string> Layers;        /////所有图层名的集合
        public List<WEntity2D> Entities;   /////所有的几何实例
        public WBoundingBox Bounds;        /////整个模型的范围
        public WGeos2D_PsList PsList;
        #endregion

        public double E_Merge
        {
            set { WGeos2D_Paras.E_Merge = value; }
            get {return  WGeos2D_Paras.E_Merge; }
        }

        /// 初始化
        public WGeometry2D()
        {
            Entities = new List<WEntity2D>();
            Layers = new List<string>();
            Bounds = new WBoundingBox();
            PsList = new WGeos2D_PsList();
        }

        /// 增加几何实例
        /// <summary>
        /// 增加几何实例
        /// </summary>
        public void Add_Geo(WEntity2D E)
        {
            int N;
            if (this.Entities.Count == 0)
                N = 0;
            else
                N = this.Entities[this.Entities.Count - 1].ID + 1;
            E.ID = N;
            if (Layers.Contains(E.Layer) == false)
                Layers.Add(E.Layer);
            Entities.Add(E);

            switch (E.Kind)
            {
                case GeoKind.Line:
                    WLine2D L = (WLine2D)E;
                    Bounds.Update(L.StartPoint.X, L.StartPoint.Y);
                    Bounds.Update(L.EndPoint.X, L.EndPoint.Y);
                    break;
                case GeoKind.PolyLine:
                    WPolyLine2D P = (WPolyLine2D)E;
                    for (int i = 0; i < P.Count; i++)
                        Bounds.Update(P[i].X, P[i].Y);
                    break;
                case GeoKind.Spline:
                    break;
                case GeoKind.Arc:
                    break;
                case GeoKind.Circle:
                    WCircle2D C = (WCircle2D)E;
                    Bounds.Update(C.Center.X - C.R, C.Center.Y - C.R);
                    Bounds.Update(C.Center.X + C.R, C.Center.Y + C.R);
                    Bounds.Update(C.Center.X - C.R, C.Center.Y + C.R);
                    Bounds.Update(C.Center.X + C.R, C.Center.Y - C.R);
                    break;
                default:
                    break;
            }
        }

        /// 删除几何体
        /// <summary>
        /// 删除几何体
        /// </summary>
        /// <param name="E"></param>
        public void Del_Geo(WEntity2D E)
        {
            this.Entities.Remove(E);
        }
    }
}
