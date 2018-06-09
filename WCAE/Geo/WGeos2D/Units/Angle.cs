using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.WGeos2D.Units
{
    public struct Angle
    {
        /// <summary>
        /// The value in radians
        /// </summary>
        public readonly double Radians;

        public Angle(double radians)
        {
            Radians = radians;
        }

        /// <summary>
        /// Initializes a new instance of the Angle.
        /// </summary>
        /// <param name="radians"></param>
        /// <param name="unit"></param>
        public Angle(double radians, Radians unit)
        {
            Radians = radians;
        }

        /// <summary>
        /// Initializes a new instance of the Angle.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        public Angle(double value, Degrees unit)
        {
            Radians = UnitConverter.ConvertFrom(value, unit);
        }

        /// <summary>
        /// The value in degrees
        /// </summary>
        public double Degrees
        {
            get
            {
               return UnitConverter.ConvertTo(Radians, AngleUnit.Degrees);
            }
        }
        
        /// <summary>
        /// Creates a new instance of Angle.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        public static Angle From<T>(double value, T unit) where T : IAngleUnit
        {
            return new Angle(UnitConverter.ConvertFrom(value, unit));
        }

        /// <summary>
        /// Creates a new instance of Angle.
        /// </summary>
        /// <param name="value"></param>
        public static Angle FromDegrees(double value)
        {
            return new Angle(UnitConverter.ConvertFrom(value, AngleUnit.Degrees));
        }

        /// <summary>
        /// Creates a new instance of Angle.
        /// </summary>
        /// <param name="value"></param>
        public static Angle FromRadians(double value)
        {
            return new Angle(value);
        }

        #region "运算符"
        /// <summary>
        /// Indicates whether two <see cref="T:MathNet.Spatial.Units.Angle"/> instances are equal.
        /// </summary>
        /// <returns>
        /// true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.
        /// </returns>
        /// <param name="left">A <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">A <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator ==(Angle left, Angle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether two <see cref="T:MathNet.Spatial.Units.Angle"/> instances are not equal.
        /// </summary>
        /// <returns>
        /// true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.
        /// </returns>
        /// <param name="left">A <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">A <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator !=(Angle left, Angle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Indicates whether a specified <see cref="T:MathNet.Spatial.Units.Angle"/> is less than another specified <see cref="T:MathNet.Spatial.Units.Angle"/>.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is less than the value of <paramref name="right"/>; otherwise, false. 
        /// </returns>
        /// <param name="left">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator <(Angle left, Angle right)
        {
            return left.Radians < right.Radians;
        }

        /// <summary>
        /// Indicates whether a specified <see cref="T:MathNet.Spatial.Units.Angle"/> is greater than another specified <see cref="T:MathNet.Spatial.Units.Angle"/>.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is greater than the value of <paramref name="right"/>; otherwise, false. 
        /// </returns>
        /// <param name="left">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator >(Angle left, Angle right)
        {
            return left.Radians > right.Radians;
        }

        /// <summary>
        /// Indicates whether a specified <see cref="T:MathNet.Spatial.Units.Angle"/> is less than or equal to another specified <see cref="T:MathNet.Spatial.Units.Angle"/>.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is less than or equal to the value of <paramref name="right"/>; otherwise, false.
        /// </returns>
        /// <param name="left">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator <=(Angle left, Angle right)
        {
            return left.Radians <= right.Radians;
        }

        /// <summary>
        /// Indicates whether a specified <see cref="T:MathNet.Spatial.Units.Angle"/> is greater than or equal to another specified <see cref="T:MathNet.Spatial.Units.Angle"/>.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is greater than or equal to the value of <paramref name="right"/>; otherwise, false.
        /// </returns>
        /// <param name="left">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator >=(Angle left, Angle right)
        {
            return left.Radians >= right.Radians;
        }

        /// <summary>
        /// Multiplies an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="left"/> and returns the result.
        /// </summary>
        /// <param name="right">An instance of <see cref="T:MathNet.Spatial.Units.Angle"/></param>
        /// <param name="left">An instance of <seealso cref="T:System.Double"/></param>
        /// <returns>Multiplies an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="left"/> and returns the result.</returns>
        public static Angle operator *(double left, Angle right)
        {
            return new Angle(left * right.Radians);
        }

        /// <summary>
        /// Multiplies an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="right"/> and returns the result.
        /// </summary>
        /// <param name="left">An instance of <see cref="T:MathNet.Spatial.Units.Angle"/></param>
        /// <param name="right">An instance of <seealso cref="T:System.Double"/></param>
        /// <returns>Multiplies an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="right"/> and returns the result.</returns>
        public static Angle operator *(Angle left, double right)
        {
            return new Angle(left.Radians * right);
        }

        /// <summary>
        /// Divides an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="right"/> and returns the result.
        /// </summary>
        /// <param name="left">An instance of <see cref="T:MathNet.Spatial.Units.Angle"/></param>
        /// <param name="right">An instance of <seealso cref="T:System.Double"/></param>
        /// <returns>Divides an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="right"/> and returns the result.</returns>
        public static Angle operator /(Angle left, double right)
        {
            return new Angle(left.Radians / right);
        }

        /// <summary>
        /// Adds two specified <see cref="T:MathNet.Spatial.Units.Angle"/> instances.
        /// </summary>
        /// <returns>
        /// An <see cref="T:MathNet.Spatial.Units.Angle"/> whose value is the sum of the values of <paramref name="left"/> and <paramref name="right"/>.
        /// </returns>
        /// <param name="left">A <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">A TimeSpan.</param>
        public static Angle operator +(Angle left, Angle right)
        {
            return new Angle(left.Radians + right.Radians);
        }

        /// <summary>
        /// Subtracts an Angle from another Angle and returns the difference.
        /// </summary>
        /// <returns>
        /// An <see cref="T:MathNet.Spatial.Units.Angle"/> that is the difference
        /// </returns>
        /// <param name="left">A <see cref="T:MathNet.Spatial.Units.Angle"/> (the minuend).</param>
        /// <param name="right">A <see cref="T:MathNet.Spatial.Units.Angle"/> (the subtrahend).</param>
        public static Angle operator -(Angle left, Angle right)
        {
            return new Angle(left.Radians - right.Radians);
        }

        /// <summary>
        /// Returns an <see cref="T:MathNet.Spatial.Units.Angle"/> whose value is the negated value of the specified instance.
        /// </summary>
        /// <returns>
        /// An <see cref="T:MathNet.Spatial.Units.Angle"/> with the same numeric value as this instance, but the opposite sign.
        /// </returns>
        /// <param name="Angle">A <see cref="T:MathNet.Spatial.Units.Angle"/></param>
        public static Angle operator -(Angle Angle)
        {
            return new Angle(-1*Angle.Radians);
        }

        /// <summary>
        /// Returns the specified instance of <see cref="T:MathNet.Spatial.Units.Angle"/>.
        /// </summary>
        /// <returns>
        /// Returns <paramref name="Angle"/>.
        /// </returns>
        /// <param name="Angle">A <see cref="T:MathNet.Spatial.Units.Angle"/></param>
        public static Angle operator +(Angle Angle)
        {
            return Angle;
        }
        #endregion

        #region "计算"
        /// <summary>
        /// Compares this instance to a specified <see cref="T:MathNet.Spatial.Units.Angle"/> object and returns an integer that indicates whether this <see cref="instance"/> is shorter than, equal to, or longer than the <see cref="T:MathNet.Spatial.Units.Angle"/> object.
        /// </summary>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="value"/>.
        /// 
        ///                     Value
        /// 
        ///                     Description
        /// 
        ///                     A negative integer
        /// 
        ///                     This instance is smaller than <paramref name="value"/>.
        /// 
        ///                     Zero
        /// 
        ///                     This instance is equal to <paramref name="value"/>.
        /// 
        ///                     A positive integer
        /// 
        ///                     This instance is larger than <paramref name="value"/>.
        /// 
        /// </returns>
        /// <param name="value">A <see cref="T:MathNet.Spatial.Units.Angle"/> object to compare to this instance.</param>
        public int CompareTo(Angle value)
        {
            return this.Radians.CompareTo(value.Radians);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="T:MathNet.Spatial.Units.Angle"/> object.
        /// </summary>
        /// <returns>
        /// true if <paramref name="other"/> represents the same Angle as this instance; otherwise, false.
        /// </returns>
        /// <param name="other">An <see cref="T:MathNet.Spatial.Units.Angle"/> object to compare with this instance.</param>
        public bool Equals(Angle other)
        {
            return this.Radians.Equals(other.Radians);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="T:MathNet.Spatial.Units.Angle"/> object within the given tolerance.
        /// </summary>
        /// <returns>
        /// true if <paramref name="other"/> represents the same Angle as this instance; otherwise, false.
        /// </returns>
        /// <param name="other">An <see cref="T:MathNet.Spatial.Units.Angle"/> object to compare with this instance.</param>
        /// <param name="tolerance">The maximum difference for being considered equal</param>
        public bool Equals(Angle other, double tolerance)
        {
            return Math.Abs(this.Radians - other.Radians) < tolerance;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Angle && this.Equals((Angle)obj);
        }
        #endregion
    }
}
