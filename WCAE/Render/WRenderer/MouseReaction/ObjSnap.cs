using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;

namespace WCAE.WRenderer
{
    public class ObjSnap
    {
        public List<int> Geo2D_Entities = new List<int>(); /////选择的几何实体及Point都会放在这里
        public List<int> Geo2D_Rims = new List<int>();
        /// <summary>
        /// 捕捉关闭时鼠标单击的位置
        /// </summary>
        public WPoint2D PointFree;
        /// <summary>
        /// 捕捉点的坐标范围
        /// </summary>
        public int SnapPRange = 6;                         /////捕捉点的坐标范围

        #region 临时变量
        List<int> Geo2D_Temp = new List<int>();            /////临时变量
        int NumMin;
        double DisMin = double.MaxValue;
        double Dis;
        double[] Diss;
        #endregion

        public void Snap(RenderData data, WPoint2D P, List<SnapMode> Modes)
        {
            if (data == null)
            {
                Geo2D_Entities.Clear();
                return;
            }
            if (Modes.Contains(SnapMode.Geo2DEntity))
                Snap_Geo2DEntity(P, data);
            if (Modes.Contains(SnapMode.Geo2DRim))
                Snap_Geo2DRim(P, data);
            if (Modes.Contains(SnapMode.Geo2DPoint))
                Snap_Geo2DPoint(P, data);
        }

        private void Snap_Geo2DPoint(WPoint2D P, RenderData data)
        {
            Geo2D_Entities.Clear();
            Geo2D_Temp.Clear();
            try
            {
                if (data.PsList.Count == 0 || data.PsList == null) return;
            }
            catch { }
            /////
            Geo2D_Temp = data.PsList.Point_Check(P, SnapPRange);
            if (Geo2D_Temp.Count == 0) return;
            /////
            NumMin = Geo2D_Temp[0];
            DisMin = double.MaxValue;
            for (int i = 0; i < Geo2D_Temp.Count; i++)
            {
                Dis = data.PsList[Geo2D_Temp[i]].DistanceTo(P);
                if (Dis < DisMin)
                {
                    DisMin = Dis;
                    NumMin = Geo2D_Temp[i];
                }
            }
            Geo2D_Entities.Add(NumMin);
        }

        /// <summary>
        /// 拾取2D几何曲线
        /// </summary>
        /// <param name="P"></param>
        /// <param name="data"></param>
        private void Snap_Geo2DEntity(WPoint2D P, RenderData data)
        {
            Geo2D_Entities.Clear();
            Geo2D_Temp.Clear();
            try
            {
                if (data.Entities.Count == 0 || data.Entities == null) return;
            } catch { }
            ///////
            
            for (int i = 0; i < data.Entities.Count; i++)
            {
                if (!(data.Entities[i] is WCurve2D))
                    continue;
                if (((WCurve2D)data.Entities[i]).Contains(P, 2f) == true)
                    Geo2D_Temp.Add(i);
            }
            if (Geo2D_Temp.Count == 0) return;
            /////
            NumMin = Geo2D_Temp[0];
            DisMin = double.MaxValue;
            Diss = new double[Geo2D_Temp.Count];
            for (int i = 0; i < Geo2D_Temp.Count; i++)
            {
                Dis=((WCurve2D)(data.Entities[Geo2D_Temp[i]])).GetDistance(P);
                Diss[i] = Dis;
                if (Dis < DisMin)
                {
                    DisMin = Dis;
                    NumMin = Geo2D_Temp[i];
                }
            }
            Geo2D_Entities.Add(NumMin);
        }

        /// <summary>
        /// 捕捉Rim
        /// </summary>
        /// <param name="P"></param>
        /// <param name="data"></param>
        private void Snap_Geo2DRim(WPoint2D P, RenderData data)
        {
            int N, NM;
            Geo2D_Rims.Clear();
            /////
            try
            {
                if (data.Rims.Count == 0 || data.Rims == null) return;
            }
            catch { }
            /////初步筛选
            for (int i = 0; i < data.Rims.Count; i++)
                if ((data.Rims[i]).Contains(P, 2f) == true)
                    Geo2D_Rims.Add(i);
            /////通过点与Rim关系进行再次筛选
            for (int i = 0; i < Geo2D_Rims.Count; i++)
            {
                if (Check_PointInside(data.Rims[Geo2D_Rims[i]], P) == false)
                {
                    Geo2D_Rims.RemoveAt(i);
                    i--;
                }
                else
                {
                    /////若找到一个最小级别的Rim则直接输出
                    if (data.Rims[Geo2D_Rims[i]].Smaller.Count == 0)
                    {
                        N = Geo2D_Rims[i];
                        Geo2D_Rims.Clear();
                        Geo2D_Rims.Add(N);
                        break;
                    }
                }
            }
            /////通过Smaller进行最后筛选
            if(Geo2D_Rims.Count > 1 )
            {
                NM = int.MaxValue;
                N = 0;
                for (int i = 0; i < Geo2D_Rims.Count; i++)
                {
                    if(data.Rims[Geo2D_Rims[i]].Smaller.Count <NM )
                    {
                        NM = data.Rims[Geo2D_Rims[i]].Smaller.Count;
                        N = i;
                    }
                }
                N = Geo2D_Rims[N];
                Geo2D_Rims.Clear();
                Geo2D_Rims.Add(N);
            }
        }

        /// <summary>
        /// 判断一个点是否在Rim内部
        /// </summary>
        /// <param name="Rim"></param>
        /// <param name="P"></param>
        /// <returns></returns>
        private bool Check_PointInside(WRim2D Rim, WPoint2D P)
        {
            int polySides = Rim.Shape.Count;

            int j = polySides - 1;
            bool oddNodes = false;

            for (int i = 0; i < polySides; i++)
            {
                if ((Rim.Shape[i].Y < P.Y && Rim.Shape[j].Y >= P.Y
                || Rim.Shape[j].Y < P.Y && Rim.Shape[i].Y >= P.Y)
                && (Rim.Shape[i].X <= P.X || Rim.Shape[j].X <= P.X))
                {
                    oddNodes ^= (Rim.Shape[i].X + (P.Y - Rim.Shape[i].Y) / (Rim.Shape[j].Y - Rim.Shape[i].Y) * (Rim.Shape[j].X - Rim.Shape[i].X) < P.X);
                }
                j = i;
            }
            return oddNodes;
        }
    }
}
