using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WCAE.WGeos2D.Entities;

namespace WCAE.WGeos2D.Funcs
{
    public static class Geos2D_Selection
    {
        /// 根据图层选择
        public static WEntity2D[] SelectLines(string layername, ref WGeometry2D WGC)
        {
            WEntity2D[] Out = new WEntity2D[0];
            for (int i = 0; i < WGC.Entities.Count; i++)
                if (WGC.Entities[i].Layer == layername)
                {
                    Array.Resize<WEntity2D>(ref Out, Out.Length + 1);
                    Out[Out.Length - 1] = WGC.Entities[i];
                }
            return Out;
        }

        /// 根据图层和颜色选择
        public static WEntity2D[] SelectLines(string layername, Color color, ref WGeometry2D WGC)
        {
            WEntity2D[] Out = new WEntity2D[0];
            for (int i = 0; i < WGC.Entities.Count; i++)
                if (WGC.Entities[i].Layer == layername && WGC.Entities[i].Color.R == color.R && WGC.Entities[i].Color.G == color.G && WGC.Entities[i].Color.B == color.B)
                {
                    Array.Resize<WEntity2D>(ref Out, Out.Length + 1);
                    Out[Out.Length - 1] = WGC.Entities[i];
                }
            return Out;
        }
    }
}
