using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.WGeos2D.Entities
{
    public class WPolyLine2D : WCurve2D
    {
        public readonly List<WPoint2D> Vertexs;      /////如为PolyLine表示其上的所有节点
        public readonly List<double> Vertexs_Len;    /////PolyLine每个节点之间的距离

        #region Properties
        public override void GetLength()
        {
            if (this._length == 0)
                for (int i = 0; i < this.Vertexs.Count - 1; i++)
                    this._length += this.Vertexs_Len[i];
        }

        /// <summary>
        /// 求最近点
        /// </summary>
        public WPoint2D ClosestPointTo(WPoint2D P)
        {
            WVector2D v; /////P1到P的向量
            WVector2D d; /////P1到P2向量
            double dotProduct;
            WVector2D alongVector;
            double L;
            WPoint2D Pn = Vertexs[0];
            double DisMin = double.MaxValue;
            WPoint2D PointMin = Vertexs[0];
            double Dis;
            for (int i = 0; i < Vertexs.Count - 1; i++)
            {
                v = Vertexs[i].VectorTo(P);
                d = Vertexs[i].VectorTo(Vertexs[i + 1]);
                L = d.Length;
                d = d.Normalize();
                dotProduct = v.DotProduct(d);
                /////
                if (dotProduct < 0)
                    dotProduct = 0;

                if (dotProduct > L)
                    dotProduct = L;

                alongVector = dotProduct * d;
                Pn = Vertexs[i] + alongVector;
                Dis = P.DistanceTo(Pn);
                if (Dis < DisMin)
                {
                    DisMin = Dis;
                    PointMin = Pn;
                }
            }
            return Pn;
        }

        /// <summary>
        /// 求点到线的最近距离
        /// </summary>
        public override double GetDistance(WPoint2D P)
        {
            WVector2D v; /////P1到P的向量
            WVector2D d; /////P1到P2向量
            double dotProduct;
            WVector2D alongVector;
            double L;
            WPoint2D Pn;
            double DisMin = double.MaxValue;
            double Dis;
            for (int i = 0; i < Vertexs.Count - 1; i++)
            {
                v = Vertexs[i].VectorTo(P);
                d = Vertexs[i].VectorTo(Vertexs[i + 1]);
                L = d.Length;
                d = d.Normalize();
                dotProduct = v.DotProduct(d);
                /////
                if (dotProduct < 0)
                    dotProduct = 0;

                if (dotProduct > L)
                    dotProduct = L;

                alongVector = dotProduct * d;
                Pn = Vertexs[i] + alongVector;
                Dis = P.DistanceTo(Pn);
                if ( Dis<DisMin ) DisMin=Dis;
            }
            return DisMin;
        }

        public override WEntity2D[] CutByPoint(WPoint2D P)
        {
            return new WEntity2D[0];
        }

        public int Count
        {
            get { return this.Vertexs.Count; }
        }

        public WPoint2D this[int key]
        {
            get { return this.Vertexs[key]; }
        }

        public override WVector2D Direction_Head
        {
            get
            {
                return Vertexs[0].VectorTo(Vertexs[1]);
            }
        }

        public override WVector2D Direction_Tail
        {
            get
            {
                return Vertexs[Count - 1].VectorTo(Vertexs[Count - 2]);
            }
        }
        #endregion
        
        public WPolyLine2D(List<WPoint2D> Ps, ref WGeometry2D WGC)
        {
            this._startpoint = Ps[0];
            this._endpoint = Ps[Ps.Count - 1];
            this.WGC = WGC;
            this.Vertexs = Ps;
            if (this.Vertexs.First().Equals(this.Vertexs.Last(), WGeos2D_Paras.E_Merge))
                this.Vertexs.RemoveAt(0);
            this.Vertexs_Len = new List<double>();
            this._length = 0;
            for (int i = 0; i < this.Vertexs.Count - 1; i++)
            {
                this.Vertexs_Len.Add(this.Vertexs[i].DistanceTo(this.Vertexs[i + 1]));
                this._length += this.Vertexs_Len[i];
            }

            _xmin = double.MaxValue;
            _xmax = double.MinValue;
            _ymin = double.MaxValue;
            _ymax = double.MinValue;
            for (int i = 0; i < this.Vertexs.Count; i++)
            {
                _xmin = Math.Min(_xmin, Vertexs[i].X);
                _xmax = Math.Max(_xmax, Vertexs[i].X);
                _ymin = Math.Min(_ymin, Vertexs[i].Y);
                _ymax = Math.Max(_ymax, Vertexs[i].Y);
            }

            this.Nodes = new List<WPoint2D>();
            this.Nodes_num = new List<int>();
            this.Free_Check = true;
        }

        public override void Out2Sw(System.IO.StreamWriter sw)
        {
            sw.WriteLine("Start");
            sw.WriteLine("PolyLine");
            sw.WriteLine(Layer);
            sw.WriteLine(Color.R.ToString() + "," + Color.G.ToString() + "," + Color.B.ToString());
            sw.WriteLine(LineWidth.ToString());
            sw.WriteLine(Count.ToString());
            for (int i = 0; i < Count;i++ )
                sw.WriteLine(Vertexs[i].X.ToString() + "," + Vertexs[i].Y.ToString());
            sw.WriteLine("End");
        }
    }
}
