using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.Entities
{
    /// <summary>
    /// 节点对，用节点编号表示
    /// </summary>
    public struct WNsPair
    {
        public int N1;
        public int N2;
        public WNsPair(int N1, int N2)
        {
            this.N1 = N1;
            this.N2 = N2;
        }
    }
}
