using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCAE;

namespace WCAE.WGeos2D.Entities
{
    public abstract class WCurve2D : WEntity2D
    {
        protected WPoint2D _startpoint;      /////起点
        public WPoint2D StartPoint
        {
            get
            { return _startpoint; }
        }

        protected WPoint2D _endpoint;        /////终点
        public WPoint2D EndPoint
        {
            get
            { return _endpoint; }
        }

        protected double _length;           /////线的长度
        public double Length 
        {
            get
            {
                GetLength();
                return this._length;
            }
        }
        public abstract void GetLength();

        /// 获取点到线的距离
        /// <summary>
        /// 获取点到线的距离
        /// </summary>
        public abstract double GetDistance(WPoint2D P);

        /// 切割线
        /// <summary>
        /// 切割线
        /// </summary>
        public abstract WEntity2D[] CutByPoint(WPoint2D P);

        public override void Out2Sw(System.IO.StreamWriter sw) { }

        public virtual WVector2D Direction_Head
        { get { return _startpoint.VectorTo(_endpoint); } }

        public virtual WVector2D Direction_Tail
        { get { return _endpoint.VectorTo(_startpoint); } }

        #region 坐标范围
        protected double _xmin;
        public double Xmin
        {
            get { return _xmin; }
        }
        protected double _xmax;
        public double Xmax
        {
            get { return _xmax; }
        }
        protected double _ymin;
        public double Ymin
        {
            get { return _ymin; }
        }
        protected double _ymax;
        public double Ymax
        {
            get { return _ymax; }
        }
        public bool Contains(WPoint2D pt)
        {
            return ((pt.X >= _xmin) && (pt.X <= _xmax) && (pt.Y >= _ymin) && (pt.Y <= _ymax));
        }
        /// <summary>
        /// 判断点是否在线的范围内
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="Range">阈值，应该大于0</param>
        /// <returns></returns>
        public bool Contains(WPoint2D pt, Single Range)
        {
            return ((pt.X >= (_xmin - Range)) && (pt.X <= (_xmax + Range)) && (pt.Y >= (_ymin - Range)) && (pt.Y <= (_ymax + Range)));
        }
        #endregion

        public float LineWidth;

        #region 网格信息
        /// <summary>
        /// 进行网格划分后的节点
        /// </summary>
        public List<WPoint2D> Nodes;     /////进行网格划分后的节点
        /// <summary>
        /// 进行网格划分后的节点编号
        /// </summary>
        public List<int> Nodes_num;      /////进行网格划分后的节点编号
        /// <summary>
        /// 判断是否已经进行网格划分
        /// </summary>
        public bool Meshed_Check         /////判断是否已经进行网格划分
        {
            get
            {
                if (Nodes.Count == 0)
                    return false;
                else
                    return true;
            }
        }
        public bool Free_Check;          /////判断是否是自由边
        #endregion
    }
}
