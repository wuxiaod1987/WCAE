using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCAE.WGeos2D.Units;

namespace WCAE.WGeos2D.Entities
{
    public class WLine2D : WCurve2D
    {
        private WVector2D _direction;
        public WVector2D Direction
        {
            get
            {
                GetLength();
                return this._direction;
            }
        }
        public override void GetLength()
        {
            if (this._length <= 0)
            {
                var vectorBetween = this.StartPoint.VectorTo(this.EndPoint);
                this._length = vectorBetween.Length;
                this._direction = vectorBetween.Normalize();
            }

        }  ///  获得线的长度及方向
        
        public override void Out2Sw(System.IO.StreamWriter sw)
        {
            sw.WriteLine("Start");
            sw.WriteLine("Line");
            sw.WriteLine(Layer);
            sw.WriteLine(Color.R.ToString() + "," + Color.G.ToString() + "," + Color.B.ToString());
            sw.WriteLine(LineWidth.ToString());
            sw.WriteLine(_startpoint.X.ToString() + "," + _startpoint.Y.ToString());
            sw.WriteLine(_endpoint.X.ToString() + "," + _endpoint.Y.ToString());
            sw.WriteLine("End");
        }

        public WLine2D(WPoint2D startPoint, WPoint2D endPoint)
        {
            this._startpoint = startPoint;
            this._endpoint = endPoint;
            this.WGC = WGC;
            if (Math.Abs(this.StartPoint.X - this.EndPoint.X) <= WGeos2D_Paras.E_Merge &&
                Math.Abs(this.StartPoint.Y - this.EndPoint.Y) <= WGeos2D_Paras.E_Merge)
                throw new ArgumentException("该线段起点和终点重合！");
            ///
            _xmin = Math.Min(startPoint.X, endPoint.X);
            _xmax = Math.Max(startPoint.X, endPoint.X);
            _ymin = Math.Min(startPoint.Y, endPoint.Y);
            _ymax = Math.Max(startPoint.Y, endPoint.Y);
            ///
            GetLength();
            this.Nodes = new List<WPoint2D>();
            this.Nodes_num = new List<int>();
            this.Free_Check = true;
        }

        public WLine2D()
        {
            this.Nodes = new List<WPoint2D>();
            this.Nodes_num = new List<int>();
            this.Free_Check = true;
        }

        public override WEntity2D[] CutByPoint(WPoint2D P)
        {
            return new WEntity2D[0];
        }

        public WPoint2D ClosestPointTo(WPoint2D p, bool Extent)
        {
            WVector2D v = this.StartPoint.VectorTo(p);
            double dotProduct = v.DotProduct(this.Direction);
            if (!Extent)
            {
                if (dotProduct < 0)
                    dotProduct = 0;

                double l = this.Length;
                if (dotProduct > l)
                    dotProduct = l;
            }

            WVector2D alongVector = dotProduct * this.Direction;
            return this.StartPoint + alongVector;
        }

        /// <summary>
        /// 得到点到线的距离
        /// </summary>
        public override double GetDistance(WPoint2D P)
        {
            return P.DistanceTo(this.ClosestPointTo(P, false));
        }

        public WPoint2D IntersectWith(WLine2D other)
        {
            if (this.IsParallelTo(other))
                return null;

            WPoint2D p = this.StartPoint;
            WPoint2D q = other.StartPoint;
            WVector2D r = this.StartPoint.VectorTo(this.EndPoint);
            WVector2D s = other.StartPoint.VectorTo(other.EndPoint);
            double t = (q - p).CrossProduct(s) / (r.CrossProduct(s));

            return p + t * r;
        }

        /// 判断该线是否与另一条线平行
        /// Checks to determine whether or not two lines are parallel to each other, using the dot product within 
        /// the double precision specified in the MathNet.Numerics package.
        /// </summary>
        /// <param name="other">The other line to check this one against</param>
        /// <returns>True if the lines are parallel, false if they are not</returns>
        public bool IsParallelTo(WLine2D other)
        {
            return this.Direction.IsParallelTo(other.Direction, WGeos2D_Paras.E_Angle);
        }

        /// 判断该线是否与另一条线在一定的角度公差范围内平行
        /// Checks to determine whether or not two lines are parallel to each other within a specified angle tolerance
        /// </summary>
        /// <param name="other">The other line to check this one against</param>
        /// <param name="angleTolerance">If the angle between line directions is less than this value, the method returns true</param>
        /// <returns>True if the lines are parallel within the angle tolerance, false if they are not</returns>
        public bool IsParallelTo(WLine2D other, Angle angleTolerance)
        {
            return this.Direction.IsParallelTo(other.Direction, angleTolerance);
        }
    }
}
