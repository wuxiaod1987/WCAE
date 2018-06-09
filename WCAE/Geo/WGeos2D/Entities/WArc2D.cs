using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCAE.WGeos2D.Units;

namespace WCAE.WGeos2D.Entities
{
    public class WArc2D : WCurve2D
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

        private Angle _angle;
        public Angle Angle
        {
            get { return _angle; }
        }

        private bool _direction;     /////true表示小于180度，false表示大于180度
        public bool Direction
        {
            get { return _direction; }
        }

        public override void GetLength()
        {
            if (this._length <= 0)
            {
                WVector2D V1 = this._center.VectorTo(this._startpoint);
                WVector2D V2 = this._center.VectorTo(this._endpoint);
                _r = V1.Length;
                _angle = V1.AngleTo(V2);
                _length = Math.PI * 2 * _r * _angle.Degrees / 360;
                if (_direction == false)
                    _length = Math.PI * 2 * _r - _length;
            }
        }  ///  获得线的长度及方向

        public override WEntity2D[] CutByPoint(WPoint2D P)
        {
            return new WEntity2D[0];
        }

        public override double GetDistance(WPoint2D P)
        {
            return double.MaxValue;
        }

        public override void Out2Sw(System.IO.StreamWriter sw)
        {
            sw.WriteLine("Start");
            sw.WriteLine("Arc");
            sw.WriteLine(Layer);
            sw.WriteLine(Color.R.ToString() + "," + Color.G.ToString() + "," + Color.B.ToString());
            sw.WriteLine(LineWidth.ToString());
            sw.WriteLine(_center.X.ToString() + "," + _center.Y.ToString());
            sw.WriteLine(_r.ToString());
            sw.WriteLine(_startpoint.X.ToString() + "," + _startpoint.Y.ToString());
            sw.WriteLine(_endpoint.X.ToString() + "," + _endpoint.Y.ToString());
            sw.WriteLine("End");
        }

        public WArc2D(WPoint2D Center, WPoint2D StartPoint, WPoint2D EndPoint, bool Direction, ref WGeometry2D WGC)
        {
            this.WGC = WGC;
            _center = Center;
            _startpoint = StartPoint;
            _endpoint = EndPoint;
            _direction = Direction;
            GetLength();        
        }
    }
}
