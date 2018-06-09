using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.WGeos2D.Units
{
    public struct Degrees : IAngleUnit
    {
        public const double Conv = Math.PI / 180.0;
        internal const string Name = "\u00B0";

        public double Conversionfactor
        {
            get
            {
                return Conv;
            }
        }

        public string ShortName
        {
            get
            {
                return Name;
            }
        }

        public static Angle operator *(double left, Degrees right)
        {
            return new Angle(left, right);
        }
    }
}
