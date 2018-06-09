using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.Entities
{
    /// <summary>
    /// 2D单元
    /// </summary>
    public struct WElement2D
    {
        /// <summary>
        /// 单元种类，表示为三节点或四节点，可以用负数表示不同的类型
        /// </summary>
        public int Kind;
        public int N1;
        public int N2;
        public int N3;
        public int N4;
        public float Area;
        public int Trace;
        public WElement2D(int Kind, int N1, int N2, int N3, int N4, float Area)
        {
            this.Kind = Kind;
            this.N1 = N1;
            this.N2 = N2;
            this.N3 = N3;
            this.N4 = N4;
            this.Area=Area;
            this.Trace = 0;
        }

        public WElement2D(int Kind, int N1, int N2, int N3, int N4)
        {
            this.Kind = Kind;
            this.N1 = N1;
            this.N2 = N2;
            this.N3 = N3;
            this.N4 = N4;
            this.Area = 0f;
            this.Trace = 0;
        }
    }
}
