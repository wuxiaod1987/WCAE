using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCAE.WGeos2D.Units;

namespace WCAE.WGeos2D.Entities
{
    public struct WVector2D
    {
        public readonly double X;
        public readonly double Y;
        public WVector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        #region "运算符"
        public static WVector2D operator +(WVector2D v1, WVector2D v2)
        {
            return v1.Add(v2);
        }

        public static WVector2D operator -(WVector2D v1, WVector2D v2)
        {
            return v1.Subtract(v2);
        }

        public static WVector2D operator -(WVector2D v)
        {
            return v.Negate();
        }

        public static WVector2D operator *(double d, WVector2D v)
        {
            return new WVector2D(d * v.X, d * v.Y);
        }

        public static WVector2D operator *(WVector2D v, double d)
        {
            return d * v;
        }

        public static WVector2D operator /(WVector2D v, double d)
        {
            return new WVector2D(v.X / d, v.Y / d);
        }
        #endregion

        #region "计算"
        public WVector2D Subtract(WVector2D v)
        {
            return new WVector2D(this.X - v.X, this.Y - v.Y);
        }

        public WVector2D Add(WVector2D v)
        {
            return new WVector2D(this.X + v.X, this.Y + v.Y);
        }

        public WVector2D Negate()
        {
            return new WVector2D(-1 * this.X, -1 * this.Y);
        }

        public double Length
        {
            get { return Math.Sqrt((this.X * this.X) + (this.Y * this.Y)); }
        }

        public WVector2D Normalize()
        {
            var l = this.Length;
            return new WVector2D(this.X / l, this.Y / l);
        }

        public WVector2D ScaleBy(double d)
        {
            return new WVector2D(d * this.X, d * this.Y);
        }

        public double DotProduct(WVector2D other)
        {
            return (this.X * other.X) + (this.Y * other.Y);
        }

        /// <summary>
        /// Performs the 2D 'cross product' as if the 2D vectors were really 3D vectors in the z=0 plane, returning
        /// the scalar magnitude and direction of the resulting z value.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double CrossProduct(WVector2D other)
        {
            // Though the cross product is undefined in 2D space, this is a useful mathematical operation to 
            // determine angular direction and to compute the area of 2D shapes
            return this.X * other.Y - this.Y * other.X;
        }

        /// <summary>
        /// Computes whether or not this vector is parallel to another vector using the dot product method
        /// and comparing to within a specified tolerance
        /// </summary>
        /// <param name="othervector"></param>
        /// <param name="tolerance"></param>
        /// <returns>True if the vector dot product is within the given double tolerance of unity, false if not</returns>
        public bool IsParallelTo(WVector2D othervector, double tolerance = 1e-10)
        {
            var @this = this.Normalize();
            var other = othervector.Normalize();
            var dp = Math.Abs(@this.DotProduct(other));
            return Math.Abs(1 - dp) <= tolerance;
        }

        /// <summary>
        /// Computes whether or not this vector is parallel to another vector within a given angle tolerance.
        /// </summary>
        /// <param name="othervector"></param>
        /// <param name="angleTolerance"></param>
        /// <returns>True if the vectors are parallel within the angle tolerance, false if they are not</returns>
        public bool IsParallelTo(WVector2D othervector, Angle angleTolerance)
        {
            // Compute the angle between these vectors 
            var angle = this.AngleTo(othervector);

            // Compute the 180° opposite of the angle
            var opposite = Angle.FromDegrees(180) - angle;

            // Check against the smaller of the two
            return ((angle < opposite) ? angle : opposite) < angleTolerance;
        }

        public bool IsPerpendicularTo(WVector2D othervector, double tolerance = 1e-10)
        {
            var @this = this.Normalize();
            var other = othervector.Normalize();
            return Math.Abs(@this.DotProduct(other)) < tolerance;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="v2"></param>
        /// <param name="clockWise">Positive in clockwisedirection</param>
        /// <param name="returnNegative">If angle is > 180° a negative value is returned</param>
        /// <returns></returns>
        public Angle SignedAngleTo(WVector2D v2, bool clockWise, bool returnNegative = false)
        {
            int sign = clockWise ? -1 : 1;
            double a1 = Math.Atan2(this.Y, this.X);
            if (a1 < 0)
            {
                a1 += 2 * Math.PI;
            }

            double a2 = Math.Atan2(v2.Y, v2.X);
            if (a2 < 0)
            {
                a2 += 2 * Math.PI;
            }

            double a = sign * (a2 - a1);
            if (a < 0 && !returnNegative)
            {
                a += 2 * Math.PI;
            }

            if (a > Math.PI && returnNegative)
            {
                a -= 2 * Math.PI;
            }

            return new Angle(a, AngleUnit.Radians);
        }

        /// <summary>
        /// Compute the angle between this vector and another using the arccosine of the dot product.
        /// </summary>
        /// <param name="toVector2D"></param>
        /// <returns>The angle between vectors, with a range between 0° and 180°</returns>
        public Angle AngleTo(WVector2D toVector2D)
        {
            var @this = this.Normalize();
            var other = toVector2D.Normalize();
            return new Angle(Math.Acos(@this.DotProduct(other)), AngleUnit.Radians);
        }

        /// <summary>
        /// 从该向量到另一向量之间的夹角，顺时针旋转，以弧度制输出
        /// </summary>
        /// <param name="toVector2D"></param>
        /// <returns></returns>
        public double AngleAntiCrossWiseTo(WVector2D toVector2D)
        {
            double C = this.CrossProduct(toVector2D);
            double D = this.DotProduct(toVector2D);
            double Sin = Math.Abs(C / (this.Length * toVector2D.Length));
            double Cos = Math.Abs(D / (this.Length * toVector2D.Length));
            double Angle = Math.Asin(Sin);
            if (C >= 0 && D < 0)
                Angle = (Math.PI) - Angle;
            if (C < 0 && D >= 0)
                Angle = (2 * Math.PI) - Angle;
            if (C < 0 && D < 0)
                Angle = (Math.PI) + Angle;
            return Angle;
        }
        #endregion
    }
}
