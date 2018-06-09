using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.WGeos2D.Units
{
    public struct Radians
    {
        public const double Conv = 1.0;
        internal const string Name = "rad";

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

        public static Angle operator *(double left, Radians right)
        {
            return new Angle(left, right);
        }
    }
}
