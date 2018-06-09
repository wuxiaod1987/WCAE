using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCAE.WGeos2D.Units;

namespace WCAE.WGeos2D.Entities
{
    public class WCircle2D : WCurve2D
    {
        private WPoint2D _center;
        public WPoint2D Center
        {
            get { return _center; }
        }

        private double _r;
        public double R
        {
            get { return _r; }
        }

        public override void GetLength()
        {
            _length = Math.PI * 2 * _r;
        }  ///  获得线的长度及方向

        public override WEntity2D[] CutByPoint(WPoint2D P)
        {
            return new WEntity2D[0];
        }

        public override double GetDistance(WPoint2D P)
        {
            return Math.Abs(Math.Sqrt((P.X - _center.X) * (P.X - _center.X) + 
                                      (P.Y - _center.Y) * (P.Y - _center.Y)) - 
                                      _r);
        }

        public override void Out2Sw(System.IO.StreamWriter sw)
        {
            sw.WriteLine("Start");
            sw.WriteLine("Circle");
            sw.WriteLine(Layer);
            sw.WriteLine(Color.R.ToString() + "," + Color.G.ToString() + "," + Color.B.ToString());
            sw.WriteLine(LineWidth.ToString());
            sw.WriteLine(_center.X.ToString() + "," + _center.Y.ToString());
            sw.WriteLine(_r.ToString());
            sw.WriteLine("End");
        }

        public WCircle2D(WPoint2D Center, double R, ref WGeometry2D WGC)
        {
            this.WGC = WGC;
            _center = Center;
            _startpoint = new WPoint2D(0,0);
            _endpoint = new WPoint2D(0, 0);
            _r = R;
            GetLength();

            _xmin = Center.X - R;
            _xmax = Center.X + R;
            _ymin = Center.Y - R;
            _ymax = Center.Y + R;
        }
    }
}
