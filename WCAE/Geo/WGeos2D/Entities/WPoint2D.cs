using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.WGeos2D.Entities
{
    public class WPoint2D
    {
        public readonly double X;
        public readonly double Y;
        public int CheckNum;      /////用于存储一些点的附加信息
        /// <summary>
        /// true:硬点，false：软点
        /// </summary>
        public bool HardCheck;
        /// <summary>
        /// 点的编号
        /// </summary>
        public int Num;

        #region 构造函数
        public WPoint2D(double x, double y)
        {
            X = x;
            Y = y;
            CheckNum = 0;
        }
        public WPoint2D(int CheckNum)
        {
            X = 0;
            Y = 0;
            this.CheckNum = CheckNum;
        }
        public WPoint2D(double x, double y, int Check)
        {
            X = x;
            Y = y;
            CheckNum = Check;
        }
        public WPoint2D()
        {

        }
        #endregion

        #region "运算符"
        public static WPoint2D operator +(WPoint2D WPoint, WVector2D WVector2D)
        {
            return new WPoint2D(WPoint.X + WVector2D.X, WPoint.Y + WVector2D.Y);
        }

        public static WPoint2D operator -(WPoint2D WPoint, WVector2D WVector2D)
        {
            return new WPoint2D(WPoint.X - WVector2D.X, WPoint.Y - WVector2D.Y);
        }

        public static WVector2D operator -(WPoint2D P2, WPoint2D P1)
        {
            return new WVector2D(P2.X - P1.X, P2.Y - P1.Y);
        }
        #endregion

        #region "计算"
        public WVector2D VectorTo(WPoint2D otherPoint)
        {
            return otherPoint - this;
        }

        public double DistanceTo(WPoint2D otherPoint)
        {
            var vector = this.VectorTo(otherPoint);
            return vector.Length;
        }

        public bool Equals(WPoint2D other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.X - this.X) < tolerance &&
                   Math.Abs(other.Y - this.Y) < tolerance;
        }
        #endregion
    }
}
