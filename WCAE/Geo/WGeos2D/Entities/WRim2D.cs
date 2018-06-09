using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WCAE.WGeos2D.Funcs;

namespace WCAE.WGeos2D.Entities
{
    public class WRim2D : WEntity2D
    {
        #region Paras & Properties
        /// <summary>
        /// 围成该Rim的线的编号
        /// </summary>
        public List<int> Nums;
        /// <summary>
        /// 围城该Rim的线的实例
        /// </summary>
        public WEntity2D[] Curves;
        public List<WPoint2D> Shape;
        public Color Color_Shape;
        /// <summary>
        /// Rim含有的曲线的数量
        /// </summary>
        public int Count
        {
            get { return this.Nums.Count; }
        }
        /// <summary>
        /// 编号为key的曲线编号
        /// </summary>
        public int this[int key]
        {
            get { return this.Nums[key]; }
        }

        public int Trace;
        public bool Meshed;
        #endregion

        #region 坐标范围
        double[] _bound = new double[4];
        public double Xmin
        { get { return _bound[0]; } }
        public double Xmax
        { get { return _bound[1]; } }
        public double Ymin
        { get { return _bound[2]; } }
        public double Ymax
        { get { return _bound[3]; } }
        private void Initial_Bound()
        {
            _bound[0] = double.MaxValue;
            _bound[1] = double.MinValue;
            _bound[2] = double.MaxValue;
            _bound[3] = double.MinValue;
        }
        private void Update_Bound(double X, double Y)
        {
            if (X < _bound[0]) _bound[0] = X;
            if (X > _bound[1]) _bound[1] = X;
            if (Y < _bound[2]) _bound[2] = Y;
            if (Y > _bound[3]) _bound[3] = Y;
        }
        public bool BiggerThan(WRim2D OtherRim)
        {
            if (this.Xmax < OtherRim.Xmax) return false;
            if (this.Ymax < OtherRim.Ymax) return false;

            if (this.Xmin > OtherRim.Xmin) return false;
            if (this.Ymin > OtherRim.Ymin) return false;
            return true;
        }
        public bool Contains(WPoint2D pt)
        {
            return ((pt.X >= _bound[0]) && (pt.X <= _bound[1]) && (pt.Y >= _bound[2]) && (pt.Y <= _bound[3]));
        }
        /// <summary>
        /// 判断点是否在线的范围内
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="Range">阈值，应该大于0</param>
        /// <returns></returns>
        public bool Contains(WPoint2D pt, Single Range)
        {
            return ((pt.X >= (_bound[0] - Range)) && (pt.X <= (_bound[1] + Range)) && (pt.Y >= (_bound[2] - Range)) && (pt.Y <= (_bound[3] + Range)));
        }

        public List<int> Smaller;   ////表示可以被该Rim完全包络的Rim编号

        /// <summary>
        /// 标示该Rim在坐标范围上是否包含了其他Rim
        /// </summary>
        public bool Flag
        {
            get
            {
                if (Smaller.Count == 0)
                    return false;
                else
                    return true;
            }
        }     /////标示该Rim是否包含了其他Rim
        #endregion

        /// <summary>
        /// 判断点是否在Rim内
        /// </summary>
        public bool Check_PointInside(double x, double y)
        {
            int polySides = Shape.Count;

            int j = polySides - 1;
            bool oddNodes = false;

            for (int i = 0; i < polySides; i++)
            {
                if ((Shape[i].Y < y && Shape[j].Y >= y
                || Shape[j].Y < y && Shape[i].Y >= y)
                && (Shape[i].X <= x || Shape[j].X <= x))
                {
                    oddNodes ^= (Shape[i].X + (y - Shape[i].Y) / (Shape[j].Y - Shape[i].Y) * (Shape[j].X - Shape[i].X) < x);
                }
                j = i;
            }
            return oddNodes;
        }

        public WRim2D(ref WEntity2D[] Cs, List<int> Rim)
        {
            this.Meshed = false;
            this.Nums = Rim;
            this.Trace = 0;
            this.Smaller = new List<int>();
            this.Curves = new WEntity2D[Rim.Count];
            this.Shape = new List<WPoint2D>();
            this.Layer = "Rims";
            this.Color = Color.AliceBlue;
            this.Color_Shape = Color.Black;
            this.Kind = GeoKind.Rim;
            Initial_Bound();
            ///
            WCurve2D C;
            WPoint2D P0 = new WPoint2D();
            WPoint2D P1 = new WPoint2D();
            WPoint2D P2 = new WPoint2D();
            for (int i = 0; i < Rim.Count; i++)
            {
                this.Curves[i] = Cs[Rim[i]];
                C = (WCurve2D)Cs[Rim[i]];
                Update_Bound(C.Xmax, C.Ymax);
                Update_Bound(C.Xmin, C.Ymin);
                /////形成边界
                if (i == 0)
                {
                    P1 = ((WCurve2D)(Cs[Rim[Rim.Count - 1]])).StartPoint;
                    P2 = ((WCurve2D)(Cs[Rim[Rim.Count - 1]])).EndPoint;
                    if (Geos2D_Other.Check_PsMerge(P1, C.StartPoint) == true || 
                        Geos2D_Other.Check_PsMerge(P2, C.StartPoint) == true)
                        P0 = C.StartPoint;
                    if (Geos2D_Other.Check_PsMerge(P1, C.EndPoint) == true ||
                        Geos2D_Other.Check_PsMerge(P2, C.EndPoint) == true)
                        P0 = C.EndPoint;
                }
                P0 = Add_Curve2Shape(C, P0);
            }
        }

        /// 将某条线上的所有点加入到Rim边界中
        private WPoint2D Add_Curve2Shape(WCurve2D C, WPoint2D P0)
        {
            bool Reverse = true;
            if (Math.Abs(C.StartPoint.X - P0.X) < WGeos2D_Paras.E_Merge &&
                Math.Abs(C.StartPoint.Y - P0.Y) < WGeos2D_Paras.E_Merge)
                Reverse = false;

            switch (C.Kind)
            {
                case GeoKind.Line:
                    if (Reverse == false)
                    {
                        Shape.Add(C.EndPoint);
                        return C.EndPoint;
                    }
                    else
                    {
                        Shape.Add(C.StartPoint);
                        return C.StartPoint;
                    }
                case GeoKind.PolyLine:
                    WPolyLine2D PL = (WPolyLine2D)C;
                    if (Reverse == false)
                    {
                        for (int i = 1; i < PL.Count - 1; i ++)
                            Shape.Add(PL[i]);
                        Shape.Add(PL[PL.Count - 1]);
                        return PL[PL.Count - 1];
                    }
                    else
                    {
                        for (int i = PL.Count - 2; i > 0; i --)
                            Shape.Add(PL[i]);
                        Shape.Add(PL[0]);
                        return PL[0];
                    }
                default:
                    break;
            }
            return new WPoint2D();
        }

        public override void Out2Sw(System.IO.StreamWriter sw)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 在几何空间中增加一个Rim
        /// </summary>
        public static void Add_Rim(List<WRim2D> Rims, WRim2D Rim)
        {
            int N;
            if (Rims.Count == 0)
                N = 0;
            else
                N = Rims[Rims.Count - 1].ID + 1;
            Rim.ID = N;
            Rims.Add(Rim);
        }
    }
}
