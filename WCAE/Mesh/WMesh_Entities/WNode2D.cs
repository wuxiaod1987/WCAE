using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.Entities
{
    /// <summary>
    /// 节点
    /// </summary>
    public struct WNode2D
    {
        /// <summary>
        /// 节点编号
        /// </summary>
        public int Num;
        public double X;
        public double Y;
        /// <summary>
        /// 节点标记符号
        /// </summary>
        public int Trace;
        public WNode2D(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
            this.Trace = 0;
            this.Num = 0;
        }

        public WNode2D(int Num, double X, double Y, int Trace)
        {
            this.Num = Num;
            this.X = X;
            this.Y = Y;
            this.Trace = Trace;
        }
    }
}
