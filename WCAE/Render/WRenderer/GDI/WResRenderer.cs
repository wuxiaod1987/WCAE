using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;

namespace WCAE.WRenderer
{
    public class WResRenderer
    {
        public void Render(Graphics G, RenderParameter RParas, int Width, int Height)
        {
            G.Clear(Color.White);
            Image I = Image.FromFile(RParas.Pic_FileNeme);
            float width, height;
            float x, y;
            if (I.Width / Width > I.Height / Height)
            {
                width = Width;
                height = I.Height / (I.Width / Width);
                x = 0;
                y = (Height - height) / 2;
            }
            else
            {
                height = Height;
                width = I.Width / (I.Height / Height);
                x = (Width - width) / 2;
                y = 0;
            }
            G.DrawImage(I, x, y, width, height);
        }
    }
}
