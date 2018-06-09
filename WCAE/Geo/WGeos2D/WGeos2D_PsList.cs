using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCAE.WGeos2D.Entities;

namespace WCAE.WGeos2D
{
    /// <summary>
    /// 所有Point的集合
    /// </summary>
    public class WGeos2D_PsList
    {
        int Step = 5;        /////搜寻时每次越过的数量

        #region Paras
        /// <summary>
        /// 点的数量，实际数组数量多1
        /// </summary>
        public int Count
        {
            get { return _Ps.Count - 1; }
        }

        /// <summary>
        /// 对应编号的点
        /// </summary>
        /// <param name="key">从0开始</param>
        /// <returns>若点集为空，返回null</returns>
        public WPoint2D this[int key]
        {
            get
            {
                if (Count == 0) return null;
                return this._Ps[key];
            }
        }
        #endregion

        #region 内部变量
        List<WPoint2D> _Ps;  ///// 点集合，不对外，从1开始
        List<double> Xs;     /////点集的X坐标按照从小到大排序，从1开始
        List<int> XsIndexs;  /////X坐标排序对应的点集的顺序号，从1开始
        List<double> Ys;     /////点集的Y坐标按照从小到大排序，从1开始
        List<int> YsIndexs;  /////Y坐标排序对应的点集的顺序号，从1开始
        #endregion

        public WGeos2D_PsList()
        {
            _Ps = new List<WPoint2D>();
            Xs = new List<double>();
            Ys = new List<double>();
            XsIndexs = new List<int>();
            YsIndexs = new List<int>();
            /////把0先占据
            _Ps.Add(null);
            Xs.Add(0);
            Ys.Add(0);
            XsIndexs.Add(0);
            YsIndexs.Add(0);
        }

        #region Functions
        List<int> YIstemp = new List<int>();
        List<int> XIstemp = new List<int>();

        #region 增加点
        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="HardCheck">true:硬点，false:软点</param>
        /// <returns></returns>
        public WPoint2D Add(double X, double Y, bool HardCheck)
        {
            //===原来没有点，直接创建===//
            if (Count == 0)
            {
                _Ps.Add(new WPoint2D(X, Y));
                Xs.Add(X);
                Ys.Add(Y);
                XsIndexs.Add(1);
                YsIndexs.Add(1);
                _Ps[1].Num = 1;
                return Add_HardCheck(_Ps[1], HardCheck);
            }            
            //======搜寻相同坐标值======//
            int NumX = Search_Cood(X, ref Xs);
            int NumY = Search_Cood(Y, ref Ys);
            
            //=====两坐标都有相同值=====//
            if (NumX <0 && NumY <0)
            {
                int I1 = XsIndexs[-1 * NumX];
                int I2 = YsIndexs[-1 * NumY];
                if (I1 == I2)
                    return Add_HardCheck(_Ps[I1], HardCheck);               
            }
            //=======没有相同点========//
            _Ps.Add(new WPoint2D(X, Y));
            int Num = _Ps.Count - 1;
            if (NumX < 0)
                NumX = -1 * NumX;
            else
                NumX = NumX + 1;
            if (NumY < 0)
                NumY = -1 * NumY;
            else
                NumY = NumY + 1;
            //
            Xs.Insert(NumX, X);
            Ys.Insert(NumY, Y);
            XsIndexs.Insert(NumX, Num);
            YsIndexs.Insert(NumY, Num);
            _Ps[Num].Num = Num;
            return Add_HardCheck(_Ps[Num], HardCheck);
        }

        /// <summary>
        /// 返回坐标集中小于输入坐标的最大坐标的编号
        /// </summary>
        /// <returns>负值表示相等坐标，正值表示不等坐标</returns>
        private int Search_Cood(double Coord, ref List<double> Coords)
        {
            int Quan = Coords.Count;
            int[] Ss, Es;
            Ss = new int[1]; Es = new int[1];
            StepDivide(ref Ss, ref Es, Quan);
            /////
            for (int i = 0; i < Ss.Length; i++)
            {
                if (Coords[Es[i]] >= Coord)
                {
                    for (int j = Ss[i]; j < Es[i]; j++)
                    {
                        if (Coords[j] >= Coord)
                        {
                            if (Math.Abs(Coords[j] - Coord) <= WGeos2D_Paras.E_Merge)
                                return -1 * j;
                            return j - 1;
                        }
                    }
                    if (Math.Abs(Coords[Es[i]] - Coord) <= WGeos2D_Paras.E_Merge)
                        return -1 * Es[i];
                    return Es[i] - 1;    /////如果扫描到比Coord大的数，输出前一个数，如1号就大，输出0
                }
            }
            if (Math.Abs(Coords[Quan - 1] - Coord) <= WGeos2D_Paras.E_Merge)
                return -1 * (Quan - 1);
            return Quan - 1;
        }
        #endregion

        /// <summary>
        /// 得到在Range范围内的坐标的编号
        /// </summary>
        public int[] Search_Coord_Range(double CSmall, double CLarge, ref List<double> Coords)
        {
            int Quan = Coords.Count;
            int[] Ss = new int[1];
            int[] Es = new int[1];
            StepDivide(ref Ss, ref Es, Quan);
            /////
            int[] Out = new int[2];
            for (int i = 0; i < Ss.Length; i++)
            {
                if (Coords[Es[i]] >= CSmall)
                {
                    for (int j = Ss[i]; j < Es[i]; j++)
                    {
                        if (Coords[j] >= CSmall)
                        {
                            Out[0] = j;
                            break;
                        }
                    }
                    if (Out[0] == 0) 
                        Out[0] = Es[i];    /////如果扫描到比Coord大的数，输出前一个数，如1号就大，输出0
                    break;
                }
            }
            if (Out[0] == 0) Out[0] = Quan - 1;
            /////
            for (int i = Ss.Length - 1; i >= 0; i--)
            {
                if (Coords[Ss[i]] <= CLarge)
                {
                    for (int j = Es[i]; j > Ss[i]; j--)
                    {
                        if (Coords[j] <= CLarge)
                        {
                            Out[1] = j;
                            break;
                        }
                    }
                    if (Out[1] == 0)
                        Out[1] = Ss[i];    /////如果扫描到比Coord大的数，输出前一个数，如1号就大，输出0
                    break;
                }
            }
            /////
            //System.Diagnostics.Debugger.Break();
            return Out;
        }

        /// 判断点的Range区域内是否有其他点
        /// <summary>
        /// 判断点的Range区域内是否有其他点
        /// </summary>
        public List<int> Point_Check(WPoint2D P, double Range)
        {
            int[] Xts = Search_Coord_Range(P.X - Range, P.X + Range, ref Xs);
            int[] Yts = Search_Coord_Range(P.Y - Range, P.Y + Range, ref Ys);
            int[] XIs = new int[Xts[1] - Xts[0] + 1];
            int[] YIs = new int[Yts[1] - Yts[0] + 1];

            for (int i = 0; i <= (Xts[1] - Xts[0]); i++)
                XIs[i] = XsIndexs[Xts[0] + i];
            for (int i = 0; i <= (Yts[1] - Yts[0]); i++)
                YIs[i] = YsIndexs[Yts[0] + i];

            return Index_Inter(ref XIs, ref YIs);
        }

        /// <summary>
        /// 在Point上添加HardCheck属性
        /// </summary>
        private WPoint2D Add_HardCheck(WPoint2D P, bool HardCheck)
        {
            if (P.HardCheck != true)
                P.HardCheck = HardCheck;
            return P;
        }

        /// 将长数组分割为几个短数组
        /// <summary>
        /// 将长数组分割为几个短数组
        /// </summary>
        private void StepDivide(ref int[] Ss, ref int[] Es, int Quan)
        {
            if (Quan <= Step + 1)
            {
                Ss = new int[1]; Es = new int[1];
                Ss[0] = 1; Es[0] = Quan - 1;
            }
            else
            {
                int s = (int)Math.Floor((double)((Quan - 1) / Step));  /////求出每个Step之间的间隔
                Ss = new int[Step];
                Es = new int[Step];
                for (int i = 0; i < Step - 1; i++)
                {
                    Ss[i] = 1 + i * s;
                    Es[i] = Ss[i] + s;
                }
                Ss[Step - 1] = Es[Step - 2];
                Es[Step - 1] = Quan - 1;
            }
        }

        /// 找到两个List之间的相同数值
        /// <summary>
        /// 找到两个List之间的相同数值
        /// </summary>
        private List<int> Index_Inter(ref int[] Is1, ref int[] Is2)
        {
            List<int> Out = new List<int>();
            for (int i = 0; i < Is1.Length; i++)
                for (int j = 0; j < Is2.Length; j++)
                    if (Is1[i] == Is2[j]) Out.Add(Is1[i]);
            //System.Diagnostics.Debugger.Break();
            return Out;
        }
        #endregion        
    }
}
