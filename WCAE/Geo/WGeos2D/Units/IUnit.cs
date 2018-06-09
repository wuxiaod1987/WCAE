using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.WGeos2D.Units
{
    public interface IUnit
    {
        double Conversionfactor { get; }

        string ShortName { get; }
    }
}
