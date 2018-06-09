using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WCAE.WGeos2D.Funcs;

namespace WCAE.WGeos2D.Entities
{
    public class WShapeRim2D
    {
        List<WPoint2D> Rim;

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
        #endregion

        public WPoint2D this[int key]
        {
            get { return this.Rim[key]; }
        }
        public int Count
        {
            get { return this.Rim.Count; }
        }
        public Color ColorEdge;
        public float LineWidth;

        public WShapeRim2D(List<WPoint2D> Rim, Color ColorEdge, float LineWidth)
        {
            this.Rim = Rim;
            this.ColorEdge = ColorEdge;
            this.LineWidth = LineWidth;
            Initial_Bound();
        }

        public WShapeRim2D(Color ColorEdge, float LineWidth)
        {
            this.Rim = new List<WPoint2D>();
            this.ColorEdge = ColorEdge;
            this.LineWidth = LineWidth;
            Initial_Bound();
        }

        #region 节点输入
        public void Add_P(WPoint2D P)
        {
            Rim.Add(new WPoint2D(P.X, P.Y));
            Update_Bound(P.X, P.Y);
        }

        public void Add_P(double X, double Y)
        {
            Rim.Add(new WPoint2D(X, Y));
            Update_Bound(X, Y);
        }

        public void Add_Ps(ref List<WPoint2D> Ps, bool Mid_Check, bool HT_Check)
        {
            if (HT_Check == true)
                Add_P(Ps[0]);
            if (Mid_Check == true)
                for (int i = 1; i < Ps.Count - 1; i++)
                    Add_P(Ps[i]);
            if (HT_Check == true)
                Add_P(Ps[Ps.Count - 1]);
        }

        public void Add_Ps(ref List<WPoint2D> Ps, ref List<int> Nums, bool Reverse_Check, int Num_Pre)
        {
            int Quan = Ps.Count;
            if (Reverse_Check == false)
            {
                if (Nums[0] != Num_Pre)
                    Add_P(Ps[0]);
                for (int i = 1; i < Quan; i++)
                    Add_P(Ps[i]);
            }
            else
            {
                if (Nums[Quan - 1] != Num_Pre)
                    Add_P(Ps[Quan - 1]);
                for (int i = Quan - 2; i >= 0; i--)
                    Add_P(Ps[i]);
            }
        }

        public void Merge_Duplicate()
        {
            Geos2D_Other.Merge_Points(ref Rim);  /////去掉多余节点
        }
        #endregion
    }
}
