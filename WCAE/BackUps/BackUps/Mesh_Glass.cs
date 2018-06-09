using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using WCAE.Entities;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;
using WCAE.WGeos2D.Funcs;
using WCAE.WMesh2D.Funcs;
using WCAE.WMesh2D;
using WCAE.WMesh2D.Entities;
using GL.FEM.Mesh;
using GL.FEM;
using GL;

namespace BackUps
{
    /// <summary>
    /// 整面夹丝计算网格划分时，使用共节点方法时，Mesh玻璃区域的类，因改为Embed方法暂时不用
    /// </summary>
    static class Mesh_Glass
    {
        static void Do_Mesh(ref GLCommon GLC)
        {
            ///////得到各种线的ID
            WLine2D AxisX = GLC.Get_Line_XAxis();                                                             /////中间X轴线
            WEntity2D[] Ws = Geos2D_Selection.SelectLines(GLC.Layer_Wires, GLC.Color_Wires, ref GLC.WGC);     /////夹丝线
            WEntity2D[] Ws_Sorted = Sort_Wires(AxisX, ref  Ws, ref GLC);                                      /////将线的ID按照X值从小到大排列
            Ws = null;
            MeshPara_Compute(ref Ws_Sorted, ref GLC);
            /////
            WMesh2D_Mesh Mesh_Glass;
            for (int i = 0; i < GLC.Rims.Count; i++)
            {
                if (GLC.Rims[i].Trace % 10 == 1)
                {
                    Mesh_Glass = Mesh_SingleRim(ref AxisX, ref GLC.Rims[i].Curves, ref Ws_Sorted, ref GLC);
                    if (Mesh_Glass == null) continue;
                    GLC.Rims[i].Meshed = true;
                    Mesh_Glass.Trace = 1;
                    Mesh_Glass.Color = GLC.Rims[i].Color;
                    GLC.Meshs.Add(Mesh_Glass);
                }
            }
            for (int i = 0; i < GLC.Rims.Count; i++) //////GLC.Rims.Count
            {
                if (GLC.Rims[i].Trace % 10 == 1 && GLC.Rims[i].Meshed == false)
                {
                    Mesh_Glass = Mesh_FreeRim(ref GLC.Rims[i].Curves, ref Ws_Sorted, ref GLC, i.ToString());
                    if (Mesh_Glass == null) continue;
                    Mesh_Glass.Trace = 1;
                    Mesh_Glass.Color = GLC.Rims[i].Color;
                    GLC.Meshs.Add(Mesh_Glass);
                    //Mesh_Glass.Output_ToFile("G:\\");
                }
            }
        }

        #region 处理自由的夹丝区域2#
        private static WMesh2D_Mesh Mesh_FreeRim2(ref WEntity2D[] WBs, ref WEntity2D[] Ws, ref GLCommon GLC, string Name)
        {
            /////找到基础线
            WEntity2D Bas = WBs[0];
            for (int i = 0; i < WBs.Length; i++)
                if (((WCurve2D)WBs[i]).Meshed_Check == true)
                {
                    Bas = WBs[i];
                    break;
                }
            //////找到与Bas相交的夹丝线
            WEntity2D[] Wsc = new WEntity2D[0];
            for (int i = 0; i < Ws.Length; i++)
            {
                if (Geos2D_Intersection.Intersection_L2C((WLine2D)Ws[i], Bas).Count > 0)
                {
                    Array.Resize<WEntity2D>(ref Wsc, Wsc.Length + 1);
                    Wsc[Wsc.Length - 1] = Ws[i];
                }
            }
            /////

            return null;
        }
        #endregion

        #region 处理自由的夹丝区域
        private static WMesh2D_Mesh Mesh_FreeRim(ref WEntity2D[] WBs, ref WEntity2D[] Ws, ref GLCommon GLC, string Name)
        {
            WMesh2D_Mesh Mesh_Glass = new WMesh2D_Mesh(Name);
            /////
            WEntity2D[] Bds = new WEntity2D[0];                                              /////所有的内部边界线
            List<int>[] Rims = Find_BdsRims(ref WBs, ref Ws, ref GLC, ref Bds, Mesh_Glass);  /////找到所有的边界线及所有的区域
            /////逐个Rim分别Mesh
            WElement2D[] Es = new WElement2D[0];
            WEntity2D[] Rim;
            List<int> Ns = new List<int>();
            List<WPoint2D> Ps = new List<WPoint2D>();
            /////
            for (int i = 0; i < Rims.Length; i++)
            {
                Rim = new WEntity2D[Rims[i].Count];
                for (int j = 0; j < Rim.Length; j++)
                    Rim[j] = Bds[Rims[i][j]];

                Es = Mesh2D_SingleLayerMesh.Do_Mesh(ref Rim, i.ToString(), ref GLC.MP);
                if (Es == null || Es.Length == 0)
                {
                    for (int j = 0; j < Rim.Length; j++)
                    {
                        for (int k = 0; k < ((WCurve2D)Rim[j]).Nodes.Count - 1; k++)
                        {
                            Ps.Add(((WCurve2D)Rim[j]).Nodes[k]);
                            Ns.Add(((WCurve2D)Rim[j]).Nodes_num[k]);
                        }
                    }
                    Es = Mesh2D_MeshFree.Mesh_SingleRim(ref Ns, ref Ps, ref GLC.MP, i.ToString());
                }
                if (Es == null || Es.Length == 0) continue;
                Mesh_Glass.Add_Es(ref Es);

            }
            if (Mesh_Glass.QEs == 0) return null;
            else
            {
                for (int i = 0; i < Bds.Length; i++)
                {
                    if (Bds[i].Trace != -2) continue;
                    Es = new WElement2D[0];
                    WMFuncs2D.E2sID_App(ref ((WCurve2D)Bds[i]).Nodes_num, ref  Es, 0, true); /////Beam单元生成
                    if (Es == null || Es.Length == 0) continue;
                    Mesh_Glass.Add_Es(ref Es);                                               /////Beam单元输出
                }
            }
            /////
            int Nt = 0;
            for (int i = 0; i < WBs.Length; i++)
            {
                if (((WCurve2D)WBs[i]).Meshed_Check == true) continue;
                ((WCurve2D)WBs[i]).Nodes = new List<WPoint2D>();
                for (int j = 0; j < Bds.Length; j++)
                {
                    Nt = ((WCurve2D)Bds[j]).Trace;
                    if (Nt != i) continue;
                    for (int k = 0; k < ((WCurve2D)Bds[j]).Nodes.Count - 1; k++)
                        ((WCurve2D)WBs[i]).Nodes.Add(((WCurve2D)Bds[j]).Nodes[k]);
                }
                ((WCurve2D)WBs[i]).Nodes.Add(new WPoint2D(((WCurve2D)WBs[i]).EndPoint.X, ((WCurve2D)WBs[i]).EndPoint.Y));
            }
            //////
            return Mesh_Glass;
        }

        /// 找到所有的边界线及所有的区域
        private static List<int>[] Find_BdsRims(ref WEntity2D[] WBs, ref WEntity2D[] Ws, ref GLCommon GLC, ref WEntity2D[] Bds, WMesh2D_Mesh Mesh)
        {
            int N = 0;              /////记录节点编号
            List<WPoint2D> Pts;                /////辅助点集
            int Q, Q2;
            /////找到每条边界线与夹丝线的交点
            WCurve2D C;
            for (int i = 0; i < WBs.Length; i++)
            {
                C = (WCurve2D)(WBs[i]);
                if (C.Meshed_Check == true)
                {
                    Geos2D_Other.Merge_Points(ref C.Nodes);  /////去掉多余节点
                    Pts = C.Nodes;
                    if (WMFuncs2D.Length_P2P(C.StartPoint, Pts[0]) > WMFuncs2D.Length_P2P(C.EndPoint, Pts[0]))
                        WMFuncs2D.Points_Reverse(ref Pts, true);
                    if (WMFuncs2D.Near_Check(Pts[0], C.StartPoint) == false) Pts.Insert(0, C.StartPoint);
                    if (WMFuncs2D.Near_Check(Pts[Pts.Count - 1], C.EndPoint) == false) Pts.Add(C.EndPoint);
                    C.Nodes = Pts;
                    /////
                    Divide_Ps(Pts, ref N, ref  Bds, -1, Mesh, true, GLC.Mesh_Length);   /////分割点集为线段集合
                }
                else
                {
                    Pts = Geos2D_Intersection.Intersection_Cv2Ls(WBs[i], ref Ws); /////找与上下边线的交点作为划分的边界
                    if (Pts.Count < 2)
                    {
                        Array.Resize<WEntity2D>(ref Bds, Bds.Length + 1);
                        Bds[Bds.Length - 1] = WBs[i];
                        ((WCurve2D)Bds[Bds.Length - 1]).Trace = -1;
                        WMFuncs2D.Mesh_Curve(Bds[Bds.Length - 1], GLC.Mesh_Length);
                        ((WCurve2D)Bds[Bds.Length - 1]).Nodes_num = Get_NodeNums(((WCurve2D)Bds[Bds.Length - 1]).Nodes, Mesh, ref N);
                        continue;
                    }
                    else
                        Geos2D_Other.Merge_Points(ref Pts);  /////去掉多余节点

                    if (WMFuncs2D.Length_P2P(C.StartPoint, Pts[0]) > WMFuncs2D.Length_P2P(C.EndPoint, Pts[0]))
                        WMFuncs2D.Points_Reverse(ref Pts, true);
                    if (WMFuncs2D.Near_Check(Pts[0], C.StartPoint) == false) Pts.Insert(0, C.StartPoint);
                    if (WMFuncs2D.Near_Check(Pts[Pts.Count - 1], C.EndPoint) == false) Pts.Add(C.EndPoint);
                    /////
                    Divide_Ps(Pts, ref N, ref  Bds, i, Mesh, false, GLC.Mesh_Length);   /////分割点集为线段集合
                }
            }
            Mesh.Q_FreeNs = N;
            /////找到所有夹丝线的交点，形成新的直线加入
            for (int i = 0; i < Ws.Length; i++)
            {
                Pts = Geos2D_Intersection.Intersection_L2Cvs((WLine2D)Ws[i], ref WBs); /////找与上下边线的交点作为划分的边界
                if (Pts.Count == 0) continue;                                          /////夹丝线与上下边界不一定会有交点
                Geos2D_Other.Merge_Points(ref Pts);                                    /////去掉多余节点
                if (Pts.Count == 1) continue;
                /////
                Array.Resize<WEntity2D>(ref Bds, Bds.Length + 1);
                Q = Bds.Length - 1;
                Bds[Q] = new WLine2D(Pts[0], Pts[1]);
                Bds[Q].Trace = -2;
                WMFuncs2D.Mesh_Curve(Bds[Q], GLC.Mesh_Length);                                                  /////生成该线的节点
                N = WMFuncs2D.NsID_App(ref ((WCurve2D)Bds[Q]).Nodes, ref ((WCurve2D)Bds[Q]).Nodes_num, false, N); /////Mesh该线,注意头尾不要
                Mesh.Add_Ns(ref ((WCurve2D)Bds[Q]).Nodes, ref ((WCurve2D)Bds[Q]).Nodes_num, true, false);        /////将节点编号输入至Mesh中,注意头尾不要
                ((WCurve2D)Bds[Q]).Nodes_num[0] = Get_NodeNum(((WCurve2D)Bds[Q]).Nodes[0], Mesh, ref N);
                Q2 = ((WCurve2D)Bds[Q]).Nodes_num.Count - 1;
                ((WCurve2D)Bds[Q]).Nodes_num[Q2] = Get_NodeNum(((WCurve2D)Bds[Q]).Nodes[Q2], Mesh, ref N);
            }

            return Geo2D_CurvesRims.Get_Rims(ref Bds, 1);
        }

        /// 分割点集为线段集合
        private static void Divide_Ps(List<WPoint2D> Ps, ref int N, ref WEntity2D[] Bds, int Trace, WMesh2D_Mesh Mesh, bool Meshed, double Mesh_Length)
        {
            List<int> Nums = Get_NodeNums(Ps, Mesh, ref N);               /////得到Pts的节点编号集合，从0开始
            int Q;
            for (int i = 0; i < Ps.Count - 1; i++)
            {
                Array.Resize<WEntity2D>(ref Bds, Bds.Length + 1);
                Q = Bds.Length - 1;
                Bds[Q] = new WLine2D(Ps[i], Ps[i + 1]);
                ((WLine2D)(Bds[Q])).Trace = Trace;
                if (Meshed == true)
                {
                    ((WLine2D)(Bds[Q])).Nodes.Add(Ps[i]);
                    ((WLine2D)(Bds[Q])).Nodes.Add(Ps[i + 1]);
                    ((WLine2D)(Bds[Q])).Nodes_num.Add(Nums[i]);
                    ((WLine2D)(Bds[Q])).Nodes_num.Add(Nums[i + 1]);
                }
                else
                {
                    WMFuncs2D.Mesh_Curve(Bds[Q], Mesh_Length);
                    ((WLine2D)(Bds[Q])).Nodes_num.Add(Nums[i]);
                    for (int j = 1; j < ((WLine2D)(Bds[Q])).Nodes.Count - 1; j++)
                    {
                        N++;
                        Mesh.Add_N(((WLine2D)(Bds[Q])).Nodes[j], N);
                        ((WLine2D)(Bds[Q])).Nodes_num.Add(N);
                    }
                    ((WLine2D)(Bds[Q])).Nodes_num.Add(Nums[i + 1]);
                }

            }
        }

        /// 寻找某个点集的节点编号，若节点不存在则创造节点
        private static List<int> Get_NodeNums(List<WPoint2D> Ps, WMesh2D_Mesh Mesh, ref int N)
        {
            List<int> Nums = new List<int>();     /////注意节点编号是从0开始的
            bool Check = false;
            for (int i = 0; i < Ps.Count; i++)
            {
                Check = false;
                for (int j = 1; j < Mesh.Nodes.Count; j++)
                {
                    if (WMFuncs2D.Near_Check(Ps[i], Mesh.Nodes[j]) == true)
                    {
                        Nums.Add(j);
                        Check = true;
                        break;
                    }
                }
                if (Check == true) continue;
                N++;
                Mesh.Add_N(Ps[i], N);
                Nums.Add(N);
            }
            return Nums;
        }

        /// 寻找某个已有节点编号，不存在则
        private static int Get_NodeNum(WPoint2D P, WMesh2D_Mesh Mesh, ref int N)
        {
            for (int j = 1; j < Mesh.Nodes.Count; j++)
                if (WMFuncs2D.Near_Check(P, Mesh.Nodes[j]) == true)
                    return j;
            N++;
            Mesh.Add_N(P, N);
            return N;
        }
        #endregion

        #region 使用常用方法Mesh夹丝区域
        /// <summary>
        /// 生成夹丝区域的节点，单元，输出，绘制等
        /// </summary> 
        /// <param name="AxisX">横向轴线</param>
        /// <param name="WBs">夹丝线</param>
        /// <param name="Ws">夹丝区域的上下边界线</param>
        /// <remarks></remarks>
        private static WMesh2D_Mesh Mesh_SingleRim(ref WLine2D AxisX,
                                           ref WEntity2D[] WBs, ref WEntity2D[] Ws,
                                           ref GLCommon GLC)
        {
            WMesh2D_Mesh Glass_Mesh = new WMesh2D_Mesh("Wires");
            Glass_Mesh.Shapes.Add(new WShapeRim2D(Color.Black, 1f));
            /////
            int N = 0;          /////节点编号
            int E4 = 0;         /////Shell单元编号
            int E2 = 0;         /////Beam单元编号

            WEntity2D[] WBs_V = Sort_WBs(AxisX, ref WBs, ref GLC);  /////找到竖向边界排序输出，并在边界线数组中去除竖向边界线
            if (WBs_V.Length == 0) return null;
            WCurve2D Bd_L = (WCurve2D)WBs_V[0];                      /////左侧边界
            WCurve2D Bd_R = (WCurve2D)WBs_V[1];                      /////右侧边界
            WCurve2D[] Bds_O = new WCurve2D[WBs.Length];            /////上下边界线
            for (int i = 0; i < WBs.Length; i++)
                Bds_O[i] = (WCurve2D)WBs[i];

            ///////先求出上下边界
            List<WPoint2D> Ps_WiL = new List<WPoint2D>();            /////记录所有线与上边界的交点，从左至右
            List<WPoint2D> Ps_WiU = new List<WPoint2D>();            /////记录所有线与下边界的交点，从左至右
            List<WPoint2D> Pts;                                      /////辅助点集

            for (int i = 0; i <= Ws.Length - 1; i++)
            {
                Pts = Geos2D_Intersection.Intersection_L2Cvs((WLine2D)Ws[i], ref WBs); /////找与上下边线的交点作为划分的边界
                if (Pts.Count == 0) continue;        /////夹丝线与上下边界不一定会有交点
                Add_Ps2_Bnds(ref Pts, ref Bds_O);

                Geos2D_Other.Merge_Points(ref Pts);  /////去掉多余节点
                if (Pts[0].Y > Pts[1].Y)
                {
                    Ps_WiU.Add(Pts[0]);
                    Ps_WiL.Add(Pts[1]);
                }
                else
                {
                    Ps_WiU.Add(Pts[1]);
                    Ps_WiL.Add(Pts[0]);
                }
            }

            ///////对上下左右边界进行网格划分//////
            Bd_L.Nodes = Geos2D_Modify.DotCurve_Times(Bd_L, GLC.Mesh_WireTimes);  /////左边界划分
            Bd_R.Nodes = Geos2D_Modify.DotCurve_Times(Bd_R, GLC.Mesh_WireTimes);  /////右边界划分
            WMFuncs2D.Ps_Sort(Bd_L.Nodes, false, true);                       /////从下到上排列左边界节点
            WMFuncs2D.Ps_Sort(Bd_R.Nodes, false, true);                       /////从下到上排列右边界节点

            //////Shell//////
            List<int> Ns_WiL = new List<int>();             //////记录所有线与上边界的交点的节点编号(Shell)，从左至右
            List<int> Ns_WiU = new List<int>();             //////记录所有线与下边界的交点的节点编号(Shell)，从左至右
            N = WMFuncs2D.NsID_App(ref Bd_L.Nodes, ref  Bd_L.Nodes_num, N);      //////将左边界的节点进行划分并输入Bnd中(Shell)
            N = WMFuncs2D.NsID_App(ref Ps_WiU, ref  Ns_WiU, N);                  //////将上边界的节点进行划分(Shell)
            N = WMFuncs2D.NsID_App(ref Bd_R.Nodes, ref  Bd_R.Nodes_num, N, true);//////将右边界的节点进行划分并输入Bnd中(Shell)，注意需要反向
            N = WMFuncs2D.NsID_App(ref Ps_WiL, ref  Ns_WiL, N, true);            //////将下边界的节点进行划分，注意需要反向(Shell)

            //Add_HT2_Bnds(ref Bd_L.Nodes, ref Bds_O);                           //////将左边界头尾节点加入上下边界中（为了节点顺序对，右边界在最后输入）

            //////输出
            Glass_Mesh.Q_FreeNs = Bd_L.Nodes.Count * 2 + Ns_WiL.Count * 2;    /////自由节点数量输出(Shell)
            Glass_Mesh.Add_Ns(ref Bd_L.Nodes, ref Bd_L.Nodes_num, false, 0);  /////输出左边界的节点至Shell节点文件中
            Glass_Mesh.Add_Ns(ref Ps_WiU, ref  Ns_WiU, false, 0);             /////输出上边界的节点至Shell节点文件中
            Glass_Mesh.Add_Ns(ref Bd_R.Nodes, ref  Bd_R.Nodes_num, true, 0);  /////输出右边界的节点至Shell节点文件中，注意反向
            Glass_Mesh.Add_Ns(ref Ps_WiL, ref  Ns_WiL, true, 0);              /////输出下边界的节点至Shell节点文件中，注意反向
            /////将边界点放到Shape中
            Glass_Mesh.Shapes[0].Add_Ps(ref Bd_L.Nodes, ref Bd_L.Nodes_num, false, 0);  /////输出左边界的节点至Shell节点文件中
            Glass_Mesh.Shapes[0].Add_Ps(ref Ps_WiU, ref  Ns_WiU, false, 0);             /////输出上边界的节点至Shell节点文件中
            Glass_Mesh.Shapes[0].Add_Ps(ref Bd_R.Nodes, ref  Bd_R.Nodes_num, true, 0);  /////输出右边界的节点至Shell节点文件中，注意反向
            Glass_Mesh.Shapes[0].Add_Ps(ref Ps_WiL, ref  Ns_WiL, true, 0);              /////输出下边界的节点至Shell节点文件中，注意反向

            /////对夹丝线进行网格划分//////
            List<WPoint2D> Ps1 = Bd_L.Nodes;
            List<WPoint2D> Ps2 = new List<WPoint2D>();
            List<int> Ns1 = Bd_L.Nodes_num;
            List<int> Ns2 = new List<int>();
            WElement2D[] Es = new WElement2D[0];

            for (int i = 0; i <= Ws.Length - 1; i++)
            {
                Ps2 = Geos2D_Modify.DotCurve_Times(Ws[i], Ps_WiU[i], Ps_WiL[i], GLC.Mesh_WireTimes, ref GLC.WGC); /////划分夹丝线
                WMFuncs2D.Ps_Sort(Ps2, false, true);                          /////从下到上排列节点
                //Add_HT2_Bnds(ref Ps2, ref Bds_O);                               /////将头尾节点加入上下边界中
                //////Shell//////
                N = WMFuncs2D.NsID_App(ref Ps2, ref  Ns2, false, N);              /////生成Shell单元的节点编号（注意头尾不要）
                Ns2[0] = Ns_WiL[i];                                               /////为生成单元，用下边界节点替换最下点编号
                Ns2[Ns2.Count - 1] = Ns_WiU[i];                                   /////为生成单元，用上边界节点替换最上点编号
                E4 = WMFuncs2D.E4sID_App(ref Ns1, ref Ns2, ref  Es, E4);          /////生成Shell单元
                Glass_Mesh.AddCross_NodesPair(ref Ns1, ref Ns2);                  /////将节点之间架桥，用于显示
                /////输出
                Glass_Mesh.Add_Ns(ref Ps2, ref  Ns2, true, false);                /////将Shell的中间节点输出至文件（注意头尾不要）
                Glass_Mesh.Add_Es(ref Es);                                        /////输出Shell单元

                E2 = WMFuncs2D.E2sID_App(ref Ns2, ref  Es, E2, true);             /////Beam单元生成
                Glass_Mesh.Add_Es(ref Es);                                        /////Beam单元输出

                //////循环推进//////
                Ps1 = Ps2;
                Ns1 = Ns2;
            }

            ///////最右侧单元处理//////
            Ps2 = Bd_R.Nodes;
            Ns2 = Bd_R.Nodes_num;
            //Add_HT2_Bnds(ref Bd_R.Nodes, ref  Bds_O);                           /////将右侧边界头尾节点加入上下边界中
            E4 = WMFuncs2D.E4sID_App(ref Ns1, ref  Ns2, ref Es, E4);              /////输入单元编号
            Glass_Mesh.AddCross_NodesPair(ref Ns1, ref Ns2);                      /////将节点之间架桥，用于显示
            Glass_Mesh.Add_Es(ref Es);
            return Glass_Mesh;
        }

        #region "计算函数"
        /// 找到某点所在的Bnd的编号
        private static List<int> Bnd_Find(WPoint2D P, ref WCurve2D[] Bnds)
        {
            List<int> Out = new List<int>();
            double Dt;
            for (int i = 0; i < Bnds.Length; i++)
            {
                Dt = Geos2D_Other.ClosestDistance_P2C(Bnds[i], P, true);
                if (Dt < WGeos2D_Paras.E_Merge)
                    Out.Add(i);
            }
            return (Out);
        }

        /// 将计算出夹丝线与边界线的交点加入边界线的节点中
        private static void Add_Ps2_Bnds(ref List<WPoint2D> Pts, ref WCurve2D[] Bds_O)
        {
            for (int j = 0; j < Pts.Count; j++)
            {
                Bds_O[Pts[j].CheckNum].Nodes.Add(new WPoint2D(Pts[j].X, Pts[j].Y, 0));
                for (int k = 0; k < Bds_O.Length; k++)
                {
                    if (Bds_O[k].Meshed_Check == true) continue;
                    if (k == Pts[j].CheckNum) continue;
                    if (Geos2D_Other.Check_PsMerge(Pts[j], Bds_O[k].StartPoint) == true)
                        Bds_O[k].Nodes.Add(new WPoint2D(Pts[j].X, Pts[j].Y, 0));
                    if (Geos2D_Other.Check_PsMerge(Pts[j], Bds_O[k].EndPoint) == true)
                        Bds_O[k].Nodes.Add(new WPoint2D(Pts[j].X, Pts[j].Y, 0));
                }
                Pts[j] = new WPoint2D(Pts[j].X, Pts[j].Y, 0);
            }
        }

        /// 将已经找到的节点集的头尾节点位置输入到上下边界的节点集中
        private static void Add_HT2_Bnds(ref List<WPoint2D> Ps, ref WCurve2D[] Bnds)
        {
            List<int> Ns = Bnd_Find(Ps[0], ref  Bnds);
            for (int i = 0; i < Ns.Count; i++)
                Bnds[Ns[i]].Nodes.Add(Ps[0]);

            Ns = Bnd_Find(Ps[Ps.Count - 1], ref  Bnds);
            for (int i = 0; i < Ns.Count; i++)
                Bnds[Ns[i]].Nodes.Add(Ps[Ps.Count - 1]);
        }

        #region "线的排序"
        /// 对夹丝边界线进行排序，从左至右输出竖向线
        private static WEntity2D[] Sort_WBs(WLine2D AxisX, ref WEntity2D[] WBs, ref GLCommon GLC)
        {
            int Quan = WBs.Length;
            WPoint2D[] Ps = new WPoint2D[0];  /////夹丝边界线与横轴的交点点集
            List<WPoint2D> Pst;
            WPoint2D P;
            int[] Ns = new int[0];
            int N = 0;
            for (int i = 0; i < Quan; i++)
            {
                Pst = Geos2D_Intersection.Intersection_L2C(AxisX, WBs[i]);
                if (Pst.Count == 0) continue;
                P = Pst[0];
                Array.Resize<WPoint2D>(ref Ps, N + 1);
                Array.Resize<int>(ref Ns, N + 1);
                Ps[N] = P;
                Ns[N] = i;
                N++;
            }

            ///////得到竖向线并排序输出
            int[] Sort = WMFuncs2D.Ps_Sort(ref Ps, true, true);
            WEntity2D[] Out = new WEntity2D[N];
            for (int i = 0; i < N; i++)
                Out[i] = WBs[Ns[Sort[i]]];

            ///////去除竖向线
            for (int i = 0; i < N; i++)
                for (int j = 0; j < WBs.Length; j++)
                    if (Ns[i] == j)
                    {
                        for (int k = j; k < WBs.Length - 1; k++)
                            WBs[k] = WBs[k + 1];
                        Array.Resize<WEntity2D>(ref WBs, WBs.Length - 1);

                        for (int k = i + 1; k < N; k++)
                            Ns[k] = Ns[k] - 1;
                        break;
                    }
            return (Out);
        }

        /// 对夹丝线进行排序，从左至右输出
        private static WEntity2D[] Sort_Wires(WLine2D AxisX, ref WEntity2D[] Wires, ref GLCommon GLC)
        {
            List<WPoint2D> Ps_Wires = Geos2D_Intersection.Intersection_L2Cvs(AxisX, ref Wires);  ///////得到交点

            int Quan = Wires.Length;
            int[] PNums = new int[Quan];   /////用于记录排列顺序
            for (int i = 0; i < Quan; i++)
                PNums[i] = i;
            int Nt;
            WPoint2D Pt;     /////排列点

            for (int i = 0; i <= Quan - 2; i++)
                for (int j = i + 1; j <= Quan - 1; j++)
                    if (Ps_Wires[i].X > Ps_Wires[j].X)
                    {
                        Nt = PNums[i];
                        PNums[i] = PNums[j];
                        PNums[j] = Nt;

                        Pt = Ps_Wires[i];
                        Ps_Wires[i] = Ps_Wires[j];
                        Ps_Wires[j] = Pt;
                    }

            WEntity2D[] ID_Wires_Sorted = new WEntity2D[Quan];   /////按顺序记录排列后的线

            int N;
            for (int i = 0; i <= Quan - 1; i++)
            {
                N = PNums[i];
                ID_Wires_Sorted[i] = Wires[N];
            }

            Ps_Wires = null;
            return (ID_Wires_Sorted);
        }
        #endregion

        /// 根据夹丝线计算Mesh的参数
        private static void MeshPara_Compute(ref WEntity2D[] Wires_Sorted, ref GLCommon GLC)
        {
            List<WPoint2D> Ps_WL = new List<WPoint2D>();     //////记录所有线的起点，即下方的点
            List<WPoint2D> Ps_WU = new List<WPoint2D>();     //////记录所有线的终点，即上方的点

            int Quan = Wires_Sorted.Length;
            double Ls_Len = 0;
            //////////取得首尾点
            WLine2D L;
            for (int i = 0; i < Quan; i++)
            {
                L = (WLine2D)Wires_Sorted[i];
                Ps_WL.Add(L.StartPoint);
                Ps_WU.Add(L.EndPoint);
                Ls_Len += L.Length;
            }

            //////////用于计算网格尺寸
            Ls_Len = Ls_Len / Quan;
            double LPs = 0;
            double L2;

            for (int i = 0; i <= Quan - 2; i++)
            {
                L2 = WMFuncs2D.Length_P2P(Ps_WL[i], Ps_WL[i + 1]);
                LPs += L2;
            }

            LPs = LPs / (Quan - 1);
            GLC.Mesh_Length = LPs;
            GLC.Mesh_WireTimes = (int)(Ls_Len / LPs);

        }
        #endregion
        #endregion
    }
}
