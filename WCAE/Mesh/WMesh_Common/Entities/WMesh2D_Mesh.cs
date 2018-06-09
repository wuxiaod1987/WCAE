using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using WCAE.Entities;
using WCAE.WGeos2D.Entities;

namespace WCAE.WMesh2D.Entities
{
    /// 2D网格实体
    /// <summary>
    /// 2D网格实体
    /// </summary>
    public class WMesh2D_Mesh
    {
        #region 基本参数
        /// 名称
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// 自由节点数量
        /// <summary>
        /// 自由节点数量
        /// </summary>
        public int Q_FreeNs;
        /// 节点集合
        /// <summary>
        /// 节点集合，从1开始，0号是无效元素
        /// </summary>
        public List<WNode2D> Nodes;
        /// 单元集合
        /// <summary>
        /// 单元集合
        /// </summary>
        public List<WElement2D> Elements;
        public int Trace;
        /// 节点数量
        /// <summary>
        /// 节点数量，注意该数量比Nodes集的长度少1
        /// </summary>
        public int QNs
        {
            get { return Nodes.Count - 1; }
        }
        /// 单元数量
        /// <summary>
        /// 单元数量
        /// </summary>
        public int QEs
        {
            get { return Elements.Count; }
        }

        /// <summary>
        /// 总面积，计算量大，请酌情使用
        /// </summary>
        public double Area()
        {
            double area = 0;
            double V1x, V2x, V1y, V2y;
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i].Kind == 3)
                {
                    V1x = Nodes[Elements[i].N1].X - Nodes[Elements[i].N2].X;
                    V1y = Nodes[Elements[i].N1].Y - Nodes[Elements[i].N2].Y;
                    V2x = Nodes[Elements[i].N3].X - Nodes[Elements[i].N2].X;
                    V2y = Nodes[Elements[i].N3].Y - Nodes[Elements[i].N2].Y;
                    area += Math.Abs(V1x * V2y - V1y * V2x) / 2;
                }
                else if (Elements[i].Kind == 4)
                {
                    V1x = Nodes[Elements[i].N1].X - Nodes[Elements[i].N2].X;
                    V1y = Nodes[Elements[i].N1].Y - Nodes[Elements[i].N2].Y;
                    V2x = Nodes[Elements[i].N3].X - Nodes[Elements[i].N2].X;
                    V2y = Nodes[Elements[i].N3].Y - Nodes[Elements[i].N2].Y;
                    area += Math.Abs(V1x * V2y - V1y * V2x) / 2;
                    V1x = Nodes[Elements[i].N1].X - Nodes[Elements[i].N4].X;
                    V1y = Nodes[Elements[i].N1].Y - Nodes[Elements[i].N4].Y;
                    V2x = Nodes[Elements[i].N3].X - Nodes[Elements[i].N4].X;
                    V2y = Nodes[Elements[i].N3].Y - Nodes[Elements[i].N4].Y;
                    area += Math.Abs(V1x * V2y - V1y * V2x) / 2;
                }
                else continue;
            }
            return area;
        }        
        #endregion

        #region 显示参数
        /// 用于显示的外框
        /// <summary>
        /// 用于显示的外框，用该外框显示在绘制单元边界可以提高绘图效率，但只有单通道的Mesh能用，组合后不能用
        /// </summary>
        public List<WShapeRim2D> Shapes;

        /// 用于显示的颜色
        /// <summary>
        /// 用于显示的颜色
        /// </summary>
        public Color Color;
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
        /// <summary>
        /// 从外直接输入Mesh的四个边界
        /// </summary>
        /// <param name="Xmin"></param>
        /// <param name="Xmax"></param>
        /// <param name="Ymin"></param>
        /// <param name="Ymax"></param>
        public void Input_Bound(double Xmin, double Xmax, double Ymin, double Ymax)
        {
            _bound[0] = Xmin;
            _bound[1] = Xmax;
            _bound[2] = Ymin;
            _bound[3] = Ymax;
        }
        #endregion

        #region 初始化
        public WMesh2D_Mesh()
        {
            Initial_Bound();
            Nodes = new List<WNode2D>();
            Nodes.Add(new WNode2D());
            Elements = new List<WElement2D>();
            Shapes = new List<WShapeRim2D>();
        }

        public WMesh2D_Mesh(string Name)
        {
            this.Name = Name;
            Initial_Bound();
            Nodes = new List<WNode2D>();
            Nodes.Add(new WNode2D());
            Elements = new List<WElement2D>();
            Shapes = new List<WShapeRim2D>();
        }

        public WMesh2D_Mesh(string Name, string FileName)
        {
            this.Name = Name;
            Initial_Bound();
            Nodes = new List<WNode2D>();
            Nodes.Add(new WNode2D());
            Elements = new List<WElement2D>();
            Shapes = new List<WShapeRim2D>();
            Input_FromFile(FileName);
        }
        #endregion

        #region 节点之间的连接关系
        public WNsPair[] NsPairs;     /////节点之间的连接

        /// 找到点与点之间的连接
        /// <summary>
        /// 找到点与点之间的连接
        /// </summary>
        public void Compute_NodesPair()
        {
            int[][] Ns = new int[Nodes.Count][];
            int[] Nst = new int[0];
            bool Check = true;
            for (int i = 0; i < Elements.Count; i++)
            {
                /////
                if (Elements[i].Kind < 3) continue;
                Nst = new int[Elements[i].Kind + 2];
                Nst[1] = Elements[i].N1;
                Nst[2] = Elements[i].N2;
                Nst[3] = Elements[i].N3;
                if (Elements[i].Kind == 4)
                {
                    Nst[4] = Elements[i].N4;
                    Nst[5] = Elements[i].N1;
                    Nst[0] = Elements[i].N4;
                }
                else
                {
                    Nst[4] = Elements[i].N1;
                    Nst[0] = Elements[i].N3;
                }
                //////
                for (int j = 1; j < Nst.Length - 1; j++)
                {
                    if (Ns[Nst[j]] == null) Ns[Nst[j]] = new int[0];
                    Check = true;
                    for (int k = 0; k < Ns[Nst[j]].Length; k++)
                        if (Ns[Nst[j]][k] == Nst[j + 1])
                        {
                            Check = false;
                            break;
                        }
                    if (Check == true)
                    {
                        Array.Resize<int>(ref Ns[Nst[j]], Ns[Nst[j]].Length + 1);
                        Ns[Nst[j]][Ns[Nst[j]].Length - 1] = Nst[j + 1];
                    }
                    /////
                    Check = true;
                    for (int k = 0; k < Ns[Nst[j]].Length; k++)
                        if (Ns[Nst[j]][k] == Nst[j - 1])
                        {
                            Check = false;
                            break;
                        }
                    if (Check == true)
                    {
                        Array.Resize<int>(ref Ns[Nst[j]], Ns[Nst[j]].Length + 1);
                        Ns[Nst[j]][Ns[Nst[j]].Length - 1] = Nst[j - 1];
                    }
                }
            }
            /////形成点对
            NsPairs = new WNsPair[0];
            for (int i = 1; i < Nodes.Count; i++)
            {
                if (Ns[i] == null) continue;
                for (int j = 0; j < Ns[i].Length; j++)
                {
                    if (Ns[i][j] > i)
                    {
                        Array.Resize<WNsPair>(ref NsPairs, NsPairs.Length + 1);
                        NsPairs[NsPairs.Length - 1].N1 = i;
                        NsPairs[NsPairs.Length - 1].N2 = Ns[i][j];
                    }
                }
            }
        }

        /// 在节点编号集之间架桥，用于显示
        /// <summary>
        /// 在节点编号集之间架桥，用于显示，先Run该函数后得到NodesPair
        /// </summary>
        public void AddCross_NodesPair(ref List<int> Ns1, ref List<int> Ns2)
        {
            if (NsPairs == null) NsPairs = new WNsPair[0];
            for (int i = 1; i < Ns1.Count - 1; i++)
            {
                Array.Resize<WNsPair>(ref NsPairs, NsPairs.Length + 1);
                NsPairs[NsPairs.Length - 1].N1 = Ns1[i];
                NsPairs[NsPairs.Length - 1].N2 = Ns2[i];
            }
        }
        #endregion

        #region 节点输入
        public void Add_N(WPoint2D P, int Num)
        {
            Nodes.Add(new WNode2D(Num, P.X, P.Y, P.CheckNum));
            Update_Bound(P.X, P.Y);
        }

        public void Add_N(WNode2D Node)
        {
            Nodes.Add(Node);
            Update_Bound(Node.X, Node.Y);
        }

        public void Add_Ns(ref List<WPoint2D> Ps, ref List<int> Nums, bool Mid_Check, bool HT_Check)
        {
            if (HT_Check == true)
                Add_N(Ps[0], Nums[0]);
            if (Mid_Check == true)
                for (int i = 1; i < Ps.Count - 1; i++)
                    Add_N(Ps[i], Nums[i]);
            if (HT_Check == true)
                Add_N(Ps[Ps.Count - 1], Nums[Ps.Count - 1]);
        }

        public void Add_Ns(ref List<WPoint2D> Ps, ref List<int> Nums, bool Reverse_Check, int Num_Pre)
        {
            int Quan = Ps.Count;
            if (Reverse_Check == false)
            {
                if (Nums[0] != Num_Pre)
                    Add_N(Ps[0], Nums[0]);
                for (int i = 1; i < Quan; i++)
                    Add_N(Ps[i], Nums[i]);
            }
            else
            {
                if (Nums[Quan - 1] != Num_Pre)
                    Add_N(Ps[Quan - 1], Nums[Quan - 1]);
                for (int i = Quan - 2; i >= 0; i--)
                    Add_N(Ps[i], Nums[i]);
            }
        }
        #endregion

        #region 单元输入
        public void Add_E(WElement2D E)
        {
            this.Elements.Add(E);
        }
        /// 单元集输出
        public void Add_Es(ref WElement2D[] Es)
        {
            for (int i = 0; i < Es.Length; i++)
                this.Elements.Add(Es[i]);
        }

        /// 单元输出，Kind_0为True表示输出Kind为负数的单元
        public void Add_Es(ref WElement2D[] Es, bool Kind_M)
        {
            int Quan = Es.Length;
            for (int i = 0; i < Quan; i++)
                if (Kind_M == true)
                    this.Elements.Add(new WElement2D(Es[i].Kind * (-1), Es[i].N1, Es[i].N2, Es[i].N3, Es[i].N4));
                else
                    this.Elements.Add(new WElement2D(Es[i].Kind, Es[i].N1, Es[i].N2, Es[i].N3, Es[i].N4));
        }
        #endregion

        #region IO
        /// <summary>
        /// 从Mesh文件输入Mesh
        /// </summary>
        /// <param name="Filename"></param>
        public void Input_FromFile(string FileName)
        {
            StreamReader sr = new StreamReader(FileName);
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            string t = sr.ReadLine();
            string[] t_1;
            int No = 0;
            ///获取节点信息
            t = t.Split('	')[1];
            t_1 = t.Split(',');
            No++;
            Add_N(new WNode2D(Convert.ToDouble(t_1[0]),
                            Convert.ToDouble(t_1[1])));
            t = sr.ReadLine();
            while (t != "")
            {
                t = t.Split('	')[1];
                t_1 = t.Split(',');
                No++;
                Add_N(new WNode2D(Convert.ToDouble(t_1[0]),
                                Convert.ToDouble(t_1[1])));
                t = sr.ReadLine();
            }
            ///获取单元信息
            sr.ReadLine();
            sr.ReadLine();
            int K;
            while (sr.Peek() != -1)
            {
                t_1 = sr.ReadLine().Split('	');
                if (t_1[0] == "0")
                    continue;
                K = Convert.ToInt16(t_1[0]);
                t_1 = t_1[1].Split(',');
                Add_E(new WElement2D(K,
                       Convert.ToInt32(t_1[0]),
                       Convert.ToInt32(t_1[1]),
                       Convert.ToInt32(t_1[2]),
                       Convert.ToInt32(t_1[3])));
            }
            sr.Close();
            sr.Dispose();
        }

        /// <summary>
        /// 将Mesh输出至Mesh文件
        /// </summary>
        /// <param name="Path"></param>
        public void Output_ToFile(string Path)
        {
            StreamWriter Sw = new StreamWriter(Path + this.Name + ".mesh");
            Sw.WriteLine("Parameters of the Mesh:");
            Sw.WriteLine(this.Q_FreeNs);
            Sw.WriteLine("  Following Lines are Nodes");
            Sw.WriteLine("______________________________");
            for (int i = 1; i < this.Nodes.Count; i++)
                Sw.WriteLine(Convert.ToString(i) + "	" +
                             Convert.ToString(this.Nodes[i].X) + "," +
                             Convert.ToString(this.Nodes[i].Y) + "," +
                             Convert.ToString(this.Nodes[i].Trace));
            Sw.WriteLine("");
            Sw.WriteLine("  Following Lines are Elements");
            Sw.WriteLine("______________________________");
            for (int i = 0; i < this.Elements.Count; i++)
                Sw.WriteLine(Convert.ToString(this.Elements[i].Kind) + "	" +
                             Convert.ToString(this.Elements[i].N1) + "," +
                             Convert.ToString(this.Elements[i].N2) + "," +
                             Convert.ToString(this.Elements[i].N3) + "," +
                             Convert.ToString(this.Elements[i].N4));
            Sw.Close();
        }
        #endregion
    }
}
