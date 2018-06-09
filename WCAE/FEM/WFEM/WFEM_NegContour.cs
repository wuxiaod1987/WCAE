using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using WCAE.Entities;
using WCAE.WMesh2D;
using WCAE.WMesh2D.Entities;
using WCAE.WGeos2D.Entities;
using WCAE.WFEM;

namespace WCAE.WFEM
{
    /// 绘制2D有限元结果
    /// <summary>
    /// 绘制2D有限元结果
    /// </summary>
    public static class WFEM_NegContour
    {
        /// 绘制2D结果的云图
        /// <summary>
        /// 绘制2D结果的云图
        /// </summary>
        /// <param name="NP">云图绘制参数</param>
        /// <param name="Mesh">需要绘制的Mesh</param>
        /// <param name="RES">每个节点的结果数组，数组编号应与节点编号一致</param>
        /// <returns></returns>
        public static WFEM_NegOut Draw_to_PNG(ref WFEM_NegPara NP, WMesh2D_Mesh Mesh, ref double[] RES)
        {
            WFEM_NegOut ParaOut = new WFEM_NegOut();
            ParaOut.RES = RES;
            Bitmap bitmap = Draw_to_Bitmap(ref NP, Mesh, ref RES, ref ParaOut);
            /////保存并结束
            bitmap.Save(NP.PicPath + NP.PicName + ".png");
            bitmap.Dispose();
            return ParaOut;
        }

        /// <summary>
        /// 为视频绘制云图
        /// </summary>
        /// <param name="NP"></param>
        /// <param name="Mesh"></param>
        /// <param name="RES"></param>
        /// <returns></returns>
        public static WFEM_NegOut Draw_to_PNG(ref WFEM_NegPara NP, WMesh2D_Mesh Mesh, ref double[] RES, ref double[] RESM)
        {
            WFEM_NegOut ParaOut = new WFEM_NegOut();
            ParaOut.RES = RES;
            ParaOut.RESM = RESM;
            Bitmap bitmap = Draw_to_Bitmap(ref NP, Mesh, ref RES, ref ParaOut);
            /////保存并结束
            bitmap.Save(NP.PicPath + NP.PicName + ".png");
            bitmap.Dispose();
            return ParaOut;
        }

        private static Bitmap Draw_to_Bitmap(ref WFEM_NegPara NP, WMesh2D_Mesh Mesh, ref double[] RES, ref WFEM_NegOut ParaOut)
        {
            #region 结果的最小值，最大值
            double[] RESM_in = new double[0]; int[] NodeM_in = new int[0];
            Compute_ResPara(ref RES, ref RESM_in, ref NodeM_in);
            //Debugger.Break();
            if (ParaOut.RESM == null)
            {
                ParaOut.RESM = RESM_in;
                ParaOut.NodeM = NodeM_in;
            }
            #endregion
            
            #region 求取颜色变量
            ParaOut.Res_Each = Compute_ResEach(ref NP, ref RES, ref ParaOut.RESM);
            #endregion

            #region "初始化绘图工具"
            Bitmap bitmap = new Bitmap(NP.PicWidth, NP.PicHeight);
            Graphics graphic = Graphics.FromImage(bitmap);
            graphic.Clear(NP.Color_BackGround);
            #endregion

            #region 绘制Lable及Title
            int LableHeight = NP.PicRim;
            if (NP.ShowLable == true)
                LableHeight = Draw_Label(ref NP, ref graphic, ref ParaOut.Res_Each);  /////绘制Label
            int TitleHeight = NP.PicRim;
            if (NP.ShowTitle == true)
                TitleHeight = Draw_Title(ref NP, ref graphic, NP.PicTitle);   /////绘制Title
            #endregion

            #region 求取绘图比例
            double[] DrawRate = Compute_DrawRate((Mesh.Xmax - Mesh.Xmin),     /////求取绘图比例
                                           (Mesh.Ymax - Mesh.Ymin),
                                            NP.PicWidth - NP.PicRim * 2,
                                            NP.PicHeight - TitleHeight - LableHeight);
            #endregion

            /////绘制所有单元
            ParaOut.Areas = Draw_Elements(ref NP, ref Mesh,
                                          ref RES, ref ParaOut.Res_Each, 
                                          ref graphic, ref DrawRate, LableHeight);

            /////绘制最大值最小值
            Draw_MaxMin(ref NP, ref graphic, Mesh, 
                        RESM_in, NodeM_in, 
                        DrawRate, NP.PicHeight - LableHeight, TitleHeight);
            
            graphic.Dispose();
            return bitmap;
        }

        #region 图形总体
        /// <summary>
        /// 求出结果的最大值，最小值
        /// </summary>
        private static void Compute_ResPara(ref double[] RES, ref double[] RESM, ref int[] NodeM)
        {
            #region 结果的最小值，最大值
            RESM = new double[2] { double.MaxValue, double.MinValue };
            NodeM = new int[2];
            for (int i = 1; i < RES.Length; i++)
            {
                if (RES[i] < RESM[0])
                {
                    RESM[0] = RES[i];
                    NodeM[0] = i;
                }
                if (RES[i] > RESM[1])
                {
                    RESM[1] = RES[i];
                    NodeM[1] = i;
                }
            }
            #endregion
        }

        /// 求出Rese_Each
        /// <summary>
        /// 求出Rese_Each
        /// </summary>
        private static double[] Compute_ResEach(ref WFEM_NegPara NP, ref double[] RES, ref double[] RESM)
        {
            int Q = NP.Color_Contours.Length;   /////颜色总数，14，0~13，其中0,13为超过Limit的颜色
            double[] REach = new double[Q + 1]; /////Res_Each，其数值数量比颜色数量多1，共15,0~14
            double Res_Per, Max, Min;
            REach[0] = RESM[0];                 /////0数值为最小值
            REach[Q] = RESM[1];                 /////最后的数值为最大值
            //==============================//
            if (NP.ShowFixLimit == false)
            {
                Max = RESM[1]; 
                Min = RESM[0];
            }
            else
            {
                Max = NP.Neg_Max; if (Max > RESM[1]) Max = RESM[1]; /////若ULimit大于最大值，忽略ULimit
                Min = NP.Neg_Min; if (Min < RESM[0]) Min = RESM[0]; /////若LLimit小于最小值，忽略LLimit
            }
            //==============================//
            Res_Per = (Max - Min) / (Q - 2);        /////求每档的数值，不设置Limit
            for (int i = 1; i < Q - 1; i++)         /////直接赋值，注意此时1与1相同，最后和倒二相同
                REach[i] = Min + (i - 1) * Res_Per;
            REach[Q - 1] = Max;                     /////防止计算舍入误差，直接赋值
            return REach;
        }

        /// 求绘图比例
        /// <summary>
        /// 求绘图比例，0_绘图比例，为 实际长度/绘制长度，1_图上X坐标的偏移量，2_图上Y坐标的偏移量
        /// </summary>
        private static double[] Compute_DrawRate(double WidthA, double HeightA, int WidthP, int HeightP)
        {
            double[] Out = new double[3];
            float R1 = (float)(WidthA / WidthP);          /////云图的绘制比例，为 实际长度/绘制长度
            float R2 = (float)(HeightA / HeightP);        /////云图的绘制比例，为 实际长度/绘制长度
            if (R1 < R2)
            {
                Out[0] = R2;
                Out[1] = (WidthP - (WidthA / Out[0])) / 2;
                Out[2] = 0;
            }
            else
            {
                Out[0] = R1;
                Out[1] = 0;
                Out[2] = (HeightP - (HeightA / Out[0])) / 2;
            }
            return Out;
        }

        #region 绘制云图部件
        /// 绘制云图的Title
        /// <summary>
        /// 绘制云图的Title，返回值为Title高度（包含留白）
        /// </summary>
        /// <param name="NP"></param>
        /// <param name="Gf"></param>
        /// <param name="Title"></param>
        /// <returns></returns>
        private static int Draw_Title(ref WFEM_NegPara NP, ref Graphics Gf, String Title)
        {
            StringFormat format = new StringFormat();
            Font font = new Font(NP.TitleFont, NP.TitleHeight);
            SolidBrush brush = new SolidBrush(Color.Black);
            /////
            Rectangle Rec = new Rectangle(NP.PicRim, NP.PicRim, NP.PicWidth, (int)(NP.TitleHeight * 2));
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            Gf.DrawString(NP.PicTitle, font, brush, Rec, format);

            Rec = new Rectangle(NP.PicRim, (int)(NP.PicRim + NP.TitleHeight * 2.3), NP.PicWidth, (int)(NP.TitleHeight * 1.6));
            Gf.DrawString(NP.PicUnit, font, brush, Rec, format);
            /////
            int L = (int)(Gf.MeasureString(Title, font).Width);
            int Y = NP.PicRim + (int)(NP.TitleHeight * 2);
            Gf.DrawLine(new Pen(brush, NP.TitleHeight * 0.1f), new Point(NP.PicWidth / 2 - (int)(L * 0.53), Y), new Point(NP.PicWidth / 2 + (int)(L * 0.6), Y));
            /////
            return NP.PicRim * 2 + (int)(NP.TitleHeight * 4.3);
        }

        /// 绘制Label条
        /// <summary>
        /// 云图绘制Label条，返回值为Label条占用的高度，注意已经留白，可以直接使用
        /// </summary>
        /// <param name="NP"></param>
        /// <param name="graphic"></param>
        /// <param name="RES_Each"></param>
        /// <returns></returns>
        private static int Draw_Label(ref WFEM_NegPara NP, ref Graphics graphic, ref double[] RES_Each)
        {
            Point[] Psp = new Point[4];
            Font font = new Font(NP.LableFont, NP.LableTextHeight);
            SolidBrush brushR = new SolidBrush(Color.Black);
            SolidBrush brushF;
            /////
            int Q = NP.Color_Contours.Length;
            int Start = 0; if (NP.ShowFixLimit == false) Start = 1; /////开始的编号，对于无FixLimit从1开始
            int End = Q; if (NP.ShowFixLimit == false) End = Q - 1; /////结束的编号，对于无FixLimit在Q-1处结束
            //---------------------------//
            int Len_Total = (int)(NP.LableWidthRate * NP.PicWidth); /////Lab的横向长度
            int Len_Per = (int)(Len_Total / End);                     /////每种颜色的Label的长度
            int X_Left = (int)(NP.PicWidth - Len_Total) / 2;        /////Label最左侧的点的X坐标
            int Y_Bott = NP.PicHeight - NP.PicRim;                  /////Label最低处的Y坐标
            int Lab_DisT2L = (int)(NP.LableTextHeight * (0.8));     /////文字到Label条之间的距离

            int Y_Text = (int)(Y_Bott - Lab_DisT2L - 2.5 * NP.LableTextHeight);  /// -90;
            for (int i = Start; i < End; i++)
            {
                Psp[0] = new Point(X_Left + i * Len_Per, Y_Bott);
                Psp[1] = new Point(X_Left + (i + 1) * Len_Per, Y_Bott);
                Psp[2] = new Point(X_Left + (i + 1) * Len_Per, Y_Bott - NP.LableHight);
                Psp[3] = new Point(X_Left + i * Len_Per, Y_Bott - NP.LableHight);

                brushF = new SolidBrush(NP.Color_Contours[i]);
                graphic.FillPolygon(brushF, Psp);
                graphic.DrawString(Convert.ToString(Math.Round(RES_Each[i], 2)), font, brushR, X_Left - 2 * NP.LableTextHeight + i * Len_Per, Y_Text);
            }
            graphic.DrawString(Convert.ToString(Math.Round(RES_Each[End], 2)), font, brushR, X_Left - 2 * NP.LableTextHeight + Len_Total, Y_Text);
            return 2 * NP.PicRim + NP.LableHight + Lab_DisT2L + NP.LableTextHeight;
        }

        /// 绘制最大值、最小值的Lable
        /// <summary>
        /// 绘制最大值、最小值的Lable
        /// </summary>
        /// <param name="NP"></param>
        /// <param name="Gf"></param>
        /// <param name="Mesh"></param>
        /// <param name="RESM"></param>
        /// <param name="NodeM"></param>
        /// <param name="DrawRate"></param>
        /// <param name="Pic_YL">渲染区域下边界Pic坐标</param>
        /// <param name="Pic_YU">渲染区域上边界Pic坐标</param>
        private static void Draw_MaxMin(ref WFEM_NegPara NP, ref Graphics Gf, WMesh2D_Mesh Mesh, double[] RESM, int[] NodeM, double[] DrawRate, int Pic_YL, int Pic_YU)
        {
            if (NP.ShowMaxMin == false) return;
            Font font = new Font(NP.LableFont, NP.LableTextHeight);
            Brush brush = new SolidBrush(Color.Black);
            StringFormat format = new StringFormat();
            ///
            int X, Y, xt, yt;
            string t1, t2, t3, t4;
            int[] lts;
            int lmax;
            //==========Draw Max========//
            X = (int)((Mesh.Nodes[NodeM[1]].X - Mesh.Xmin) / DrawRate[0] + DrawRate[1]);
            Y = (int)(Pic_YL - (Mesh.Nodes[NodeM[1]].Y - Mesh.Ymin) / DrawRate[0] - DrawRate[2]);
            /////
            t1 = "Max.= " + Math.Round(RESM[1], 2).ToString();
            t2 = "Node: " + NodeM[1].ToString();
            t3 = "x = " + Math.Round(Mesh.Nodes[NodeM[1]].X, 2).ToString();
            t4 = "y = " + Math.Round(Mesh.Nodes[NodeM[1]].Y, 2).ToString();
            lts = new int[4];
            lts[0] = (int)(Gf.MeasureString(t1, font).Width);
            lts[1] = (int)(Gf.MeasureString(t2, font).Width);
            lts[2] = (int)(Gf.MeasureString(t3, font).Width);
            lts[3] = (int)(Gf.MeasureString(t4, font).Width);
            lmax = lts.Max();
            if (Y < (Pic_YL - Pic_YU) / 2)
            {
                yt = Y - NP.LableTextHeight / 2;
                if (X < NP.PicWidth / 2) xt = X + NP.LableTextHeight / 2; ///第二象限
                else xt = X - lmax - NP.LableTextHeight / 2;                 ///第一象限
            }
            else
            {
                yt = Y - (int)(NP.LableTextHeight * 5.5);
                if (X < NP.PicWidth / 2) xt = X + NP.LableTextHeight / 2; ///第三象限
                else xt = X - lmax - NP.LableTextHeight / 2;                 ///第四象限
            }
            ///
            Gf.DrawString(t1, font, brush, new PointF(xt, yt));
            if (NP.ShowMaxMinNode == true)
            {
                yt = yt + (int)(1.5 * NP.LableTextHeight);
                Gf.DrawString(t2, font, brush, new PointF(xt, yt));
                yt = yt + (int)(1.5 * NP.LableTextHeight);
                Gf.DrawString(t3, font, brush, new PointF(xt, yt));
                yt = yt + (int)(1.5 * NP.LableTextHeight);
                Gf.DrawString(t4, font, brush, new PointF(xt, yt));
            }

            xt = X - NP.LableTextHeight / 4;
            yt = Y - NP.LableTextHeight / 4;
            Gf.FillEllipse(brush, new Rectangle(xt, yt, NP.LableTextHeight / 2, NP.LableTextHeight / 2));

            //==========Draw Min========//
            t1 = "Min.= " + Math.Round(RESM[0], 2).ToString();
            t2 = "Node: " + NodeM[0].ToString();
            t3 = "x = " + Math.Round(Mesh.Nodes[NodeM[0]].X, 2).ToString();
            t4 = "y = " + Math.Round(Mesh.Nodes[NodeM[0]].Y, 2).ToString();
            lts[0] = (int)(Gf.MeasureString(t1, font).Width);
            lts[1] = (int)(Gf.MeasureString(t2, font).Width);
            lts[2] = (int)(Gf.MeasureString(t3, font).Width);
            lts[3] = (int)(Gf.MeasureString(t4, font).Width);
            lmax = lts.Max();
            ///
            X = (int)((Mesh.Nodes[NodeM[0]].X - Mesh.Xmin) / DrawRate[0] + DrawRate[1]);
            Y = (int)(Pic_YL - (Mesh.Nodes[NodeM[0]].Y - Mesh.Ymin) / DrawRate[0] - DrawRate[2]);
            if (Y < (Pic_YL - Pic_YU) / 2)
            {
                yt = Y - NP.LableTextHeight / 2;                          ///第二象限
                if (X < NP.PicWidth / 2) xt = X + NP.LableTextHeight / 2; ///第一象限                
                else xt = X - lmax - NP.LableTextHeight / 2;
            }
            else
            {
                yt = Y - (int)(NP.LableTextHeight * 5.5);                 ///第三象限                
                if (X < NP.PicWidth / 2) xt = X + NP.LableTextHeight / 2; ///第四象限                
                else xt = X - lmax - NP.LableTextHeight / 2;
            }
            ///
            Gf.DrawString(t1, font, brush, new PointF(xt, yt));
            if (NP.ShowMaxMinNode == true)
            {
                yt = yt + (int)(1.5 * NP.LableTextHeight);
                Gf.DrawString(t2, font, brush, new PointF(xt, yt));
                yt = yt + (int)(1.5 * NP.LableTextHeight);
                Gf.DrawString(t3, font, brush, new PointF(xt, yt));
                yt = yt + (int)(1.5 * NP.LableTextHeight);
                Gf.DrawString(t4, font, brush, new PointF(xt, yt));
            }

            xt = X - NP.LableTextHeight / 4;
            yt = Y - NP.LableTextHeight / 4;
            Gf.FillEllipse(brush, new Rectangle(xt, yt, NP.LableTextHeight / 2, NP.LableTextHeight / 2));

        }
        #endregion
        #endregion

        /// 绘制所有单元
        /// <summary>
        /// 绘制所有单元
        /// </summary>
        private static double[] Draw_Elements(ref WFEM_NegPara NP, ref WMesh2D_Mesh Mesh, 
                                          ref double[] RES, ref double[] Res_Each,
                                          ref Graphics graphic, ref double[] DrawRate, int LableHeight)
        {
            #region 绘图工具
            Pen RimPen = new Pen(NP.ColorEleRim, NP.PicEleRimWidth);
            Brush[] FBrushs = new Brush[NP.Color_Contours.Length];
            for (int i = 0; i < NP.Color_Contours.Length; i++)
                FBrushs[i] = new SolidBrush(NP.Color_Contours[i]);
            #endregion
            
            #region 找出所有节点的CI
            int[] Ns_CIs = Find_Ns_CIs(ref RES, ref Res_Each);
            #endregion

            #region 临时中间变量
            ///单元节点参数
            int K;                             /////单元类型
            double[] E_Rs = new double[0];     /////每个单元每个节点的结果值
            WNode2D[] E_Ns = new WNode2D[0];   /////每个单元的节点
            int[] E_CIs = new int[0];          /////每个单元每个节点的颜色编号，从0开始
            int CI_max, CI_min;                /////每个单元每个节点的CI最值
            Point[] PsT = new Point[0];        /////用于画图的临时图像点
            WNode2D[] NsT;
            WNode2D[] All_Ns = new WNode2D[0]; /////将每个单元的所有节点,中间节点混合放置在此
            int[] All_CIs = new int[0];        /////计算中间节点时使用的临时变量
            int Q;
            double[] Areas = new double[NP.Color_Contours.Length]; /////用于输出的每个CI的单元面积总和
            #endregion
            
            #region 绘制单元
            for (int i = 0; i < Mesh.Elements.Count; i++)
            {
                K = Mesh.Elements[i].Kind;
                if (K < 3)
                    continue;
                /////初始化四个点的位置，结果，Color_Index
                Compute_ElementParas(ref Mesh, i, ref RES,ref Ns_CIs, ref E_Rs, ref E_CIs, ref E_Ns);
                CI_max = E_CIs.Max(); CI_min = E_CIs.Min();
                /////如果各个节点;的Color_Index完全相同则直接画图
                if (CI_max == CI_min)
                {
                    PsT = Nodes_To_Points(ref E_Ns, Mesh.Xmin, Mesh.Ymin, ref DrawRate, true, NP.PicHeight - LableHeight);
                    graphic.FillPolygon(FBrushs[CI_max], PsT);
                    if (NP.ShowEleRim == true) graphic.DrawPolygon(RimPen, PsT);  /////绘制单元边界
                    Element_Area(ref E_Ns, ref Areas, CI_max, true);
                    continue;
                }

                /////四个节点Color_Index不完全相等的情况
                ///求取中间节点，并和单元节点放在一起
                All_Ns = new WNode2D[0]; All_CIs = new int[0];
                for (int j = 0; j < K; j++)
                    NodesInter_Compute(ref E_Ns[j], ref E_Ns[j + 1],       /////节点
                                           E_Rs[j], E_Rs[j + 1],           /////节点上的结果数值
                                           E_CIs[j], E_CIs[j + 1], CI_max, ref Res_Each,
                                           ref All_Ns, ref All_CIs, E_CIs, E_Rs);

                for (int j = CI_min; j <= CI_max; j++)
                {
                    NsT = new WNode2D[0];
                    Q = 0;
                    for (int k = 0; k < All_Ns.Length; k++)
                    {
                        if (All_CIs[k] == j)
                        { Q++; Array.Resize<WNode2D>(ref NsT, Q); NsT[Q - 1] = All_Ns[k]; continue; }
                        if (All_CIs[k] == j + 1)
                        { Q++; Array.Resize<WNode2D>(ref NsT, Q); NsT[Q - 1] = All_Ns[k]; continue; }
                    }
                    PsT = Nodes_To_Points(ref NsT, Mesh.Xmin, Mesh.Ymin, ref DrawRate, false, NP.PicHeight - LableHeight);
                    graphic.FillPolygon(FBrushs[j], PsT);
                    if (NP.ShowEleRim == true) graphic.DrawPolygon(RimPen, PsT);  /////绘制单元边界
                    Element_Area(ref NsT, ref Areas, CI_max, false);
                }
            }
            #endregion                

            return Areas;
        }

        #region Draw Elements
        #region 绘制单元
        /// 求单元的各种参数
        /// <summary>
        /// 求单元的各种参数
        /// </summary>
        private static void Compute_ElementParas(ref WMesh2D_Mesh Mesh, int EN, ref double[] RES,
                                                 ref int[] Ns_CIs, ref double[] E_Rs, ref int[] E_CIs, ref WNode2D[] E_Ns)
        {
            int K = Mesh.Elements[EN].Kind;
            E_Rs = new double[K + 1]; E_Ns = new WNode2D[K + 1]; E_CIs = new int[K + 1]; /////为了循环方便，将最后0#节点数值复制到最后
            E_Rs[0] = RES[Mesh.Elements[EN].N1]; E_Ns[0] = Mesh.Nodes[Mesh.Elements[EN].N1]; E_CIs[0] = Ns_CIs[Mesh.Elements[EN].N1];
            E_Rs[1] = RES[Mesh.Elements[EN].N2]; E_Ns[1] = Mesh.Nodes[Mesh.Elements[EN].N2]; E_CIs[1] = Ns_CIs[Mesh.Elements[EN].N2];
            E_Rs[2] = RES[Mesh.Elements[EN].N3]; E_Ns[2] = Mesh.Nodes[Mesh.Elements[EN].N3]; E_CIs[2] = Ns_CIs[Mesh.Elements[EN].N3];
            if (K == 4) { E_Rs[3] = RES[Mesh.Elements[EN].N4]; E_Ns[3] = Mesh.Nodes[Mesh.Elements[EN].N4]; E_CIs[3] = Ns_CIs[Mesh.Elements[EN].N4]; }
            E_Rs[K] = E_Rs[0]; E_Ns[K] = E_Ns[0]; E_CIs[K] = E_CIs[0];                   /////为了循环方便，将最后0#节点数值复制到最后
        }
        
        /// 求节点集围成的单元的面积
        /// <summary>
        /// 求节点集围成的单元的面积
        /// </summary>
        /// <param name="Ns">输入节点，注意从1开始</param>
        /// <returns></returns>
        private static void Element_Area(ref WNode2D[] Nodes, ref double[] Areas, int CI, bool Tail)
        {
            int TQuan = Nodes.Length - 2;
            int Start = 0;
            if (Tail == true)
            {
                TQuan = Nodes.Length - 3;
                Start = 1;
            }
            for (int i = 1; i <= TQuan; i++)
            {
                Areas[CI] += Math.Abs((Nodes[Start + i].X - Nodes[Start].X) * (Nodes[Start + i + 1].Y - Nodes[Start].Y) - 
                                      (Nodes[Start + i].Y - Nodes[Start].Y) * (Nodes[Start + i + 1].X - Nodes[Start].X)) / 2;
            }         
        }

        /// 将实际的节点值转换为画图用的Point值
        /// <summary>
        /// 将实际的节点值转换为画图用的Point值
        /// </summary>
        /// <param name="Ns"></param>
        /// <param name="Xmin">坐标原点</param>
        /// <param name="Ymin">坐标原点</param>
        /// <param name="Ratio">转换比例</param>
        /// <param name="Tail_Check">是否是首尾重复点集</param>
        /// <param name="Pic_YLim">因为Y方向是相反的，所以需要输入图片上Y方向的最大值</param>
        private static Point[] Nodes_To_Points(ref WNode2D[] Ns, double Xmin, double Ymin, ref double[] Ratio, bool Tail_Check, double Pic_YLim)
        {
            Point[] Ps;
            if (Tail_Check == true)
            {
                Ps = new Point[Ns.Length - 1];
                for (int i = 0; i < Ns.Length - 1; i++)
                    Ps[i] = new Point((int)((Ns[i].X - Xmin) / Ratio[0] + Ratio[1]), (int)(Pic_YLim - (Ns[i].Y - Ymin) / Ratio[0] - Ratio[2]));
                //Ps[i] = new Point((int)((Ns[i].X - Xmin) / Ratio[0] + Ratio[1]), (int)((Ns[i].Y - Ymin) / Ratio[0] + Ratio[2] + Pic_YOff));
            }
            else
            {
                Ps = new Point[Ns.Length];
                for (int i = 0; i < Ns.Length; i++)
                    Ps[i] = new Point((int)((Ns[i].X - Xmin) / Ratio[0] + Ratio[1]), (int)(Pic_YLim - (Ns[i].Y - Ymin) / Ratio[0] - Ratio[2]));
                //Ps[i] = new Point((int)((Ns[i].X - Xmin) / Ratio[0] + Ratio[1]), (int)((Ns[i].Y - Ymin) / Ratio[0] + Ratio[2] + Pic_YOff));
            }
            return Ps;
        }

        /// 将单个节点转换为画图用的Point
        /// <summary>
        /// 将单个节点转换为画图用的Point
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="Xmin"></param>
        /// <param name="Ymin"></param>
        /// <param name="Ratio"></param>
        /// <param name="Pic_YLim"></param>
        /// <returns></returns>
        private static Point Node_To_Point(ref WNode2D Node, double Xmin, double Ymin, ref double[] Ratio, double Pic_YLim)
        {
            return new Point((int)((Node.X - Xmin) / Ratio[0] + Ratio[1]), (int)(Pic_YLim - (Node.Y - Ymin) / Ratio[0] - Ratio[2]));
        }
        #endregion

        #region 两个节点之间的中间节点
        /// 求出每两个节点之间的中间节点
        private static void NodesInter_Compute(ref WNode2D N1, ref WNode2D N2,
                                               double Res1, double Res2,
                                               int CI1, int CI2, int CI_max,
                                               ref double[] Res_Each,
                                               ref WNode2D[] Ns_O, ref int[] CIs_O, int[] CIs, double[] ERes)
        {
            int Q = Ns_O.Length;
            Q++;
            Array.Resize<WNode2D>(ref Ns_O, Q);
            Array.Resize<int>(ref CIs_O, Q);
            Ns_O[Q - 1] = N1;
            if (CI1 == CI_max)
                CIs_O[Q - 1] = CI1 + 1;
            else
                CIs_O[Q - 1] = CI1;
            if (CI1 == CI2)     /////如果两头一样就不要继续了
                return;

            double R1, R2, Ratio;
            if (CI2 > CI1)
                for (int i = 1; i <= CI2 - CI1; i++)
                {
                    R1 = Math.Abs(Res1 - Res_Each[CI1 + i]);
                    R2 = Math.Abs(Res2 - Res_Each[CI1 + i]);
                    Ratio = R1 / (R1 + R2);

                    Q++;
                    Array.Resize<WNode2D>(ref Ns_O, Q);
                    Array.Resize<int>(ref CIs_O, Q);
                    Ns_O[Q - 1] = new WNode2D(N1.X + (N2.X - N1.X) * Ratio, N1.Y + (N2.Y - N1.Y) * Ratio);
                    CIs_O[Q - 1] = CI1 + i;
                }
            if (CI2 < CI1)
                for (int i = CI1 - CI2; i >= 1; i--)
                {
                    R1 = Math.Abs(Res1 - Res_Each[CI2 + i]);
                    R2 = Math.Abs(Res2 - Res_Each[CI2 + i]);
                    Ratio = R1 / (R1 + R2);

                    Q++;
                    Array.Resize<WNode2D>(ref Ns_O, Q);
                    Array.Resize<int>(ref CIs_O, Q);
                    Ns_O[Q - 1] = new WNode2D(N1.X + (N2.X - N1.X) * Ratio, N1.Y + (N2.Y - N1.Y) * Ratio);
                    CIs_O[Q - 1] = CI2 + i;
                }
        }
        #endregion

        #region FindCIs
        /// 找到单个节点的CI
        /// <summary>
        /// 找到单个节点的CI
        /// </summary>
        private static int Find_CI(double Re, ref double[] REach)
        {
            int Q = REach.Length;                /////15
            if (Re < REach[1]) return 0;         /////超出LLimit情况，0
            if (Re > REach[Q - 2]) return Q - 2; /////超出ULimit情况，13
            //==============================//
            for (int j = 1; j < Q - 2; j++)      /////1~12
                if (Re >= REach[j] && Re < REach[j + 1])
                    return j;
            return Q - 3;      /////Re = REach[Q-2]情况，12
        }

        /// 找到所有节点的CI
        /// <summary>
        /// 找到所有节点的CI
        /// </summary>
        private static int[] Find_Ns_CIs(ref double[] Res, ref double[] Res_Each)
        {
            int[] Ns_CIs = new int[Res.Length];
            for (int i = 1; i < Res.Length; i++)
                Ns_CIs[i] = Find_CI(Res[i], ref Res_Each);
            return Ns_CIs;
        }
        #endregion        
        #endregion
    }
}