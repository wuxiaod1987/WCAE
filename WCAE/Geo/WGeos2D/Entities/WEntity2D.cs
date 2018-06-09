using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace WCAE.WGeos2D.Entities
{
    public abstract class WEntity2D
    {
        public GeoKind Kind;        /////几何实例的种类
        public int ID;
        public ShowSort Sort;       /////显示顺序，越小越上层

        public int Trace;           /////用于显示附加消息，2：表示该线需要划分为Beam单元

        public string Layer;        /////几何实例图层的ID号
        public Color Color;         /////几何实例线的颜色

        protected WGeometry2D WGC;  /////父对象

        /// <summary>
        /// 向文件输出流中输出该对象
        /// </summary>
        public abstract void Out2Sw(StreamWriter sw);
    }
}
