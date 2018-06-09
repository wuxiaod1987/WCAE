using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.WGeos2D.Entities
{
    public class WBoundingBox
    {
        #region "参数及属性"
        double xmin, ymin, xmax, ymax;

        public double Xmin
        {
            get { return xmin; }
        }
        public double Ymin
        {
            get { return ymin; }
        }
        public double Xmax
        {
            get { return xmax; }
        }
        public double Ymax
        {
            get { return ymax; }
        }
        public double Width
        {
            get { return xmax - xmin; }
        }
        public double Height
        {
            get { return ymax - ymin; }
        }
        #endregion

        #region "初始化"
        public WBoundingBox()
        {
            xmin = double.MaxValue;
            ymin = double.MaxValue;
            xmax = -double.MaxValue;
            ymax = -double.MaxValue;
        }

        public WBoundingBox(double xmin, double ymin, double xmax, double ymax)
        {
            this.xmin = xmin;
            this.ymin = ymin;
            this.xmax = xmax;
            this.ymax = ymax;
        }
        #endregion
        public void Update(double x, double y)
        {
            xmin = Math.Min(xmin, x);
            ymin = Math.Min(ymin, y);
            xmax = Math.Max(xmax, x);
            ymax = Math.Max(ymax, y);
        }

        public void Scale(double dx, double dy)
        {
            xmin -= dx;
            xmax += dx;
            ymin -= dy;
            ymax += dy;
        }

        public bool Contains(WPoint2D pt)
        {
            return ((pt.X >= xmin) && (pt.X <= xmax) && (pt.Y >= ymin) && (pt.Y <= ymax));
        }
    }
}
