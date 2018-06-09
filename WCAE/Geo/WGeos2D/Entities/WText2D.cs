using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WCAE.WGeos2D.Entities
{
    public class WText2D : WEntity2D
    {
        public string Text;

        public WPoint2D Position;

        public int Height;

        public WText2D(string T, WPoint2D P, int H)
        {
            Text = T;
            Position = P;
            Height = H;
            this.Kind = GeoKind.DText;
            this.Color = Color.White;
        }

        public override void Out2Sw(System.IO.StreamWriter sw)
        {
            throw new NotImplementedException();
        }
    }
}
