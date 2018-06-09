//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Drawing;
//using System.IO;
//using System.Diagnostics;
//using WCAE.Entities;
//using WCAE.WMesh2D;
//using WCAE.WMesh2D.Entities;
//using WCAE.WGeos2D.Entities;
//using WCAE.WFEM;

//namespace WCAE.WFEM
//{
//    /// 绘制2D有限元结果
//    /// <summary>
//    /// 绘制2D有限元结果
//    /// </summary>
//    public static class Contour_Nephogram2D
//    {
//        static double[] RESM;  /////结果的最大值，最小值
//        static int[] NodeM;    /////最小值，最大值的对应节点编号

//        /// 绘制2D结果的云图
//        /// <summary>
//        /// 绘制2D结果的云图
//        /// </summary>
//        /// <param name="NP">云图绘制参数</param>
//        /// <param name="Mesh">需要绘制的Mesh</param>
//        /// <param name="RES">每个节点的结果数组，数组编号应与节点编号一致</param>
//        /// <returns></returns>
//        public static double[] Draw_to_PNG(ref WFEM_NegPara NP, WMesh2D_Mesh Mesh, ref double[] RES)
//        {
//            Bitmap bitmap = Draw_to_Bitmap(ref NP, Mesh, ref RES);
//            /////保存并结束
//            bitmap.Save(NP.PicPath + NP.PicName + ".png");
//            bitmap.Dispose();
//            return RESM;
//        }

//        public static Bitmap Draw_to_Bitmap(ref WFEM_NegPara NP, WMesh2D_Mesh Mesh, ref double[] RES)
//        {
//            #region 结果的最小值，最大值
//            RESM = new double[2] { double.MaxValue, double.MinValue };
//            NodeM = new int[2];
//            for (int i = 1; i < RES.Length; i++)
//            {
//                if (RES[i] < RESM[0])
//                {
//                    RESM[0] = RES[i];
//                    NodeM[0] = i;
//                }
//                if (RES[i] > RESM[1])
//                {
//                    RESM[1] = RES[i];
//                    NodeM[1] = i;
//                }
//            }
//            #endregion

//            #region 求取颜色变量
//            int Quan = NP.Color_Contours.Length;
//            double Res_Per = (RESM[1] - RESM[0]) / Quan;
//            if (NP.ShowFixLimit == true)
//                Res_Per = (NP.Neg_Max - NP.Neg_Min) / Quan;
//            double[] Res_Each = new double[Quan + 1];
//            if (NP.ShowFixLimit == false)
//                for (int i = 0; i <= Quan; i++) Res_Each[i] = RESM[0] + i * Res_Per;
//            else
//                for (int i = 0; i <= Quan; i++) Res_Each[i] = NP.Neg_Min + i * Res_Per;
//            #endregion

//            //System.Diagnostics.Debugger.Break();

//            #region "初始化绘图工具"
//            Bitmap bitmap = new Bitmap(NP.PicWidth, NP.PicHeight);
//            Graphics graphic = Graphics.FromImage(bitmap);
//            graphic.Clear(NP.Color_BackGround);
//            #endregion

//            #region 绘制Lable及Title
//            int LableHeight = NP.PicRim;
//            if (NP.ShowLable == true)
//                LableHeight = Draw_Label(ref NP, ref graphic, ref Res_Each);  /////绘制Label
//            int TitleHeight = NP.PicRim;
//            if (NP.ShowTitle == true)
//                TitleHeight = Draw_Title(ref NP, ref graphic, NP.PicTitle);   /////绘制Title
//            #endregion

//            #region 求取绘图比例
//            double[] DrawRate = DrawRate_Compute((Mesh.Xmax - Mesh.Xmin),     /////求取绘图比例
//                                           (Mesh.Ymax - Mesh.Ymin),
//                                            NP.PicWidth - NP.PicRim * 2,
//                                            NP.PicHeight - TitleHeight - LableHeight);
//            #endregion

//            /////绘制所有单元
//            Draw_Elements(ref NP, ref Mesh, ref RES, ref RESM, Res_Per, ref Res_Each, ref graphic, ref DrawRate, LableHeight);

//            /////绘制最大值最小值
//            Draw_MaxMin(ref NP, ref graphic, Mesh, RESM, NodeM, DrawRate, NP.PicHeight - LableHeight, TitleHeight);
            
//            graphic.Dispose();
//            return bitmap;
//        }
        
//        /// 求绘图比例，0_绘图比例，为 实际长度/绘制长度，1_图上X坐标的偏移量，2_图上Y坐标的偏移量
//        private static double[] DrawRate_Compute(double WidthA, double HeightA, int WidthP, int HeightP)
//        {
//            double[] Out = new double[3];
//            float R1 = (float)(WidthA / WidthP);          /////云图的绘制比例，为 实际长度/绘制长度
//            float R2 = (float)(HeightA / HeightP);        /////云图的绘制比例，为 实际长度/绘制长度
//            if (R1 < R2)
//            {
//                Out[0] = R2;
//                Out[1] = (WidthP - (WidthA / Out[0])) / 2;
//                Out[2] = 0;
//            }
//            else
//            {
//                Out[0] = R1;
//                Out[1] = 0;
//                Out[2] = (HeightP - (HeightA / Out[0])) / 2;
//            }
//            return Out;
//        }

//        /// 绘制所有单元
//        /// <summary>
//        /// 绘制所有单元
//        /// </summary>
//        private static void Draw_Elements(ref WFEM_NegPara NP, ref WMesh2D_Mesh Mesh, 
//                                          ref double[] RES, ref double[] RESM,
//                                          double Res_Per, ref double[] Res_Each,
//                                          ref Graphics graphic, ref double[] DrawRate, int LableHeight)
//        {
//            Pen RimPen = new Pen(NP.ColorEleRim, NP.PicEleRimWidth);
//            Brush FillBrush;
//            #region 临时中间变量
//            ///单元节点参数
//            int K;                /////单元类型
//            double[] E_Rs;        /////每个单元每个节点的结果值
//            WNode2D[] E_Ns;       /////每个单元的节点
//            int[] E_CIs;          /////每个单元每个节点的颜色编号，从0开始
//            int CI_max, CI_min;   /////每个单元每个节点的CI最值

//            WNode2D[] NsT;
//            Point[] PsT = new Point[0];        /////用于画图的临时图像点
//            WNode2D[] All_Ns = new WNode2D[0]; /////将每个单元的所有节点,中间节点混合放置在此
//            int[] All_CIs = new int[0];        /////计算中间节点时使用的临时变量
//            int Q;
//            #endregion

//            double Color_Max = RESM[1];
//            double Color_Min = RESM[0];
//            if (NP.ShowFixLimit == true)
//            {
//                Color_Max = NP.Neg_Max;
//                Color_Min = NP.Neg_Min;
//            }
//            #region 绘制单元
//            for (int i = 0; i < Mesh.Elements.Count; i++)   //////
//            {
//                K = Mesh.Elements[i].Kind;
//                if (K < 3)
//                    continue;
//                /////初始化四个点的位置，结果，Color_Index
//                E_Rs = new double[K + 1]; E_Ns = new WNode2D[K + 1]; E_CIs = new int[K + 1];  /////为了循环方便，将最后0#节点数值复制到最后
//                E_Rs[0] = RES[Mesh.Elements[i].N1]; E_Ns[0] = Mesh.Nodes[Mesh.Elements[i].N1]; E_CIs[0] = Color_Index(E_Rs[0], Color_Max, Color_Min, Res_Per);
//                E_Rs[1] = RES[Mesh.Elements[i].N2]; E_Ns[1] = Mesh.Nodes[Mesh.Elements[i].N2]; E_CIs[1] = Color_Index(E_Rs[1], Color_Max, Color_Min, Res_Per);
//                E_Rs[2] = RES[Mesh.Elements[i].N3]; E_Ns[2] = Mesh.Nodes[Mesh.Elements[i].N3]; E_CIs[2] = Color_Index(E_Rs[2], Color_Max, Color_Min, Res_Per);
//                CI_max = E_CIs[0]; CI_min = E_CIs[0];
//                if (E_CIs[1] > CI_max) CI_max = E_CIs[1]; if (E_CIs[1] < CI_min) CI_min = E_CIs[1];
//                if (E_CIs[2] > CI_max) CI_max = E_CIs[2]; if (E_CIs[2] < CI_min) CI_min = E_CIs[2];
//                if (K == 4)
//                {
//                    E_Rs[3] = RES[Mesh.Elements[i].N4]; E_Ns[3] = Mesh.Nodes[Mesh.Elements[i].N4]; E_CIs[3] = Color_Index(E_Rs[3], Color_Max, Color_Min, Res_Per);
//                    if (E_CIs[3] > CI_max) CI_max = E_CIs[3]; if (E_CIs[3] < CI_min) CI_min = E_CIs[3];
//                }
//                E_Rs[K] = E_Rs[0]; E_Ns[K] = E_Ns[0]; E_CIs[K] = E_CIs[0];           /////为了循环方便，将最后0#节点数值复制到最后

//                /////如果各个节点的Color_Index完全相同则直接画图
//                if (CI_max == CI_min)
//                {
//                    PsT = Nodes_To_Points(ref E_Ns, Mesh.Xmin, Mesh.Ymin, ref DrawRate, true, NP.PicHeight - LableHeight);
//                    FillBrush = new SolidBrush(NP.Color_Contours[E_CIs[0]]);
//                    graphic.FillPolygon(FillBrush, PsT);
//                    if (NP.ShowEleRim == true) graphic.DrawPolygon(RimPen, PsT);  /////绘制单元边界
//                    continue;
//                }

//                /////四个节点Color_Index不完全相等的情况
//                ///求取中间节点，并和单元节点放在一起
//                All_Ns = new WNode2D[0]; All_CIs = new int[0];
//                for (int j = 0; j < K; j++)
//                    NodesInter_Compute(ref E_Ns[j], ref E_Ns[j + 1],       /////节点
//                                           E_Rs[j], E_Rs[j + 1],           /////节点上的结果数值
//                                           E_CIs[j], E_CIs[j + 1], CI_max, ref Res_Each,
//                                           ref All_Ns, ref All_CIs, E_CIs, E_Rs);

//                for (int j = CI_min; j <= CI_max; j++)
//                {
//                    NsT = new WNode2D[0];
//                    Q = 0;
//                    for (int k = 0; k < All_Ns.Length; k++)
//                    {
//                        if (All_CIs[k] == j)
//                        { Q++; Array.Resize<WNode2D>(ref NsT, Q); NsT[Q - 1] = All_Ns[k]; continue; }
//                        if (All_CIs[k] == j + 1)
//                        { Q++; Array.Resize<WNode2D>(ref NsT, Q); NsT[Q - 1] = All_Ns[k]; continue; }
//                    }

//                    PsT = Nodes_To_Points(ref NsT, Mesh.Xmin, Mesh.Ymin, ref DrawRate, false, NP.PicHeight - LableHeight);
//                    FillBrush = new SolidBrush(NP.Color_Contours[j]);
//                    graphic.FillPolygon(FillBrush, PsT);
//                    if (NP.ShowEleRim == true) graphic.DrawPolygon(RimPen, PsT);  /////绘制单元边界
//                }
//            }
//            #endregion
//        }

//        #region 绘制云图部件
//        /// 绘制云图的Title，返回值为Title高度（包含留白）
//        /// <summary>
//        /// 绘制云图的Title，返回值为Title高度（包含留白）
//        /// </summary>
//        /// <param name="NP"></param>
//        /// <param name="Gf"></param>
//        /// <param name="Title"></param>
//        /// <returns></returns>
//        private static int Draw_Title(ref WFEM_NegPara NP, ref Graphics Gf, String Title)
//        {
//            StringFormat format = new StringFormat();
//            Font font = new Font(NP.TitleFont, NP.TitleHeight);
//            SolidBrush brush = new SolidBrush(Color.Black);
//            /////
//            Rectangle Rec = new Rectangle(NP.PicRim, NP.PicRim, NP.PicWidth, (int)(NP.TitleHeight * 2));
//            format.Alignment = StringAlignment.Center;
//            format.LineAlignment = StringAlignment.Center;
//            Gf.DrawString(NP.PicTitle, font, brush, Rec, format);

//            Rec = new Rectangle(NP.PicRim, (int)(NP.PicRim + NP.TitleHeight * 2.3), NP.PicWidth, (int)(NP.TitleHeight * 1.6));
//            Gf.DrawString(NP.PicUnit, font, brush, Rec, format);
//            /////
//            int L = (int)(Gf.MeasureString(Title, font).Width);
//            int Y = NP.PicRim + (int)(NP.TitleHeight * 2);
//            Gf.DrawLine(new Pen(brush, NP.TitleHeight*0.1f), new Point(NP.PicWidth / 2 - (int)(L * 0.53), Y), new Point(NP.PicWidth / 2 + (int)(L * 0.6), Y));
//            /////
//            return NP.PicRim * 2 + (int)(NP.TitleHeight * 4.3);
//        }

//        /// 云图绘制Label条，返回值为Label条占用的高度，注意已经留白，可以直接使用
//        private static int Draw_Label(ref WFEM_NegPara NP, ref Graphics graphic, ref double[] RES_Each)
//        {
//            Point[] Psp = new Point[4];
//            Font font = new Font(NP.LableFont, NP.LableTextHeight);
//            SolidBrush brushR = new SolidBrush(Color.Black);
//            SolidBrush brushF;

//            int Quan = NP.Color_Contours.Length;
//            int Len_Total = (int)(NP.LableWidthRate * NP.PicWidth); /////Lab的横向长度
//            int Len_Per = (int)(Len_Total / Quan);                  /////每种颜色的Label的长度
//            int X_Left = (int)(NP.PicWidth - Len_Total) / 2;        /////Label最左侧的点的X坐标
//            int Y_Bott = NP.PicHeight - NP.PicRim;                  /////Label最低处的Y坐标
//            int Lab_DisT2L = (int)(NP.LableTextHeight * (0.8));     /////文字到Label条之间的距离

//            //double Res_Per = (ResM[1] - ResM[0]) / Quan;

//            int Y_Text = (int) (Y_Bott - Lab_DisT2L - 2.5* NP.LableTextHeight);  /// -90;

//            for (int i = 0; i < Quan; i++)
//            {
//                Psp[0] = new Point(X_Left + i * Len_Per, Y_Bott);
//                Psp[1] = new Point(X_Left + (i + 1) * Len_Per, Y_Bott);
//                Psp[2] = new Point(X_Left + (i + 1) * Len_Per, Y_Bott - NP.LableHight);
//                Psp[3] = new Point(X_Left + i * Len_Per, Y_Bott - NP.LableHight);

//                brushF = new SolidBrush(NP.Color_Contours[i]);
//                graphic.FillPolygon(brushF, Psp);
//                graphic.DrawString(Convert.ToString(Math.Round(RES_Each[i], 2)), font, brushR, X_Left - 2 * NP.LableTextHeight + i * Len_Per, Y_Text);
//                //graphic.DrawString(Convert.ToString(Math.Round(ResM[0] + i * Res_Per, 2)), F_T, B_T, X_Left - 2 * NP.LableTextHeight + i * Len_Per, Y_Text);
//            }
//            //graphic.DrawString(Convert.ToString(Math.Round(ResM[1], 2)), F_T, B_T, X_Left - 2 * NP.LableTextHeight + Len_Total, Y_Text);
//            graphic.DrawString(Convert.ToString(Math.Round(RES_Each[Quan], 2)), font, brushR, X_Left - 2 * NP.LableTextHeight + Len_Total, Y_Text); 
//            return 2 * NP.PicRim + NP.LableHight + Lab_DisT2L + NP.LableTextHeight;
//        }

//        /// 绘制最大值、最小值的Lable
//        /// <summary>
//        /// 绘制最大值、最小值的Lable
//        /// </summary>
//        /// <param name="NP"></param>
//        /// <param name="Gf"></param>
//        /// <param name="Mesh"></param>
//        /// <param name="RESM"></param>
//        /// <param name="NodeM"></param>
//        /// <param name="DrawRate"></param>
//        /// <param name="Pic_YL">渲染区域下边界Pic坐标</param>
//        /// <param name="Pic_YU">渲染区域上边界Pic坐标</param>
//        private static void Draw_MaxMin(ref WFEM_NegPara NP, ref Graphics Gf, WMesh2D_Mesh Mesh, double[] RESM, int[] NodeM, double[] DrawRate, int Pic_YL, int Pic_YU)
//        {
//            if (NP.ShowMaxMin == false) return;
//            Font font = new Font(NP.LableFont, NP.LableTextHeight);
//            Brush brush = new SolidBrush(Color.Black);
//            StringFormat format = new StringFormat();
//            ///
//            int X, Y, xt, yt;
//            string t1, t2, t3, t4;
//            int[] lts;
//            int lmax;
//            //==========Draw Max========//
//            X = (int)((Mesh.Nodes[NodeM[1]].X - Mesh.Xmin) / DrawRate[0] + DrawRate[1]);
//            Y = (int)(Pic_YL - (Mesh.Nodes[NodeM[1]].Y - Mesh.Ymin) / DrawRate[0] - DrawRate[2]);
//            /////
//            t1 = "Max.= " + Math.Round(RESM[1], 2).ToString();
//            t2 = "Node: " + NodeM[1].ToString();
//            t3 = "x = " + Math.Round(Mesh.Nodes[NodeM[1]].X, 2).ToString();
//            t4 = "y = " + Math.Round(Mesh.Nodes[NodeM[1]].Y, 2).ToString();
//            lts = new int[4];
//            lts[0] = (int)(Gf.MeasureString(t1, font).Width);
//            lts[1] = (int)(Gf.MeasureString(t2, font).Width);
//            lts[2] = (int)(Gf.MeasureString(t3, font).Width);
//            lts[3] = (int)(Gf.MeasureString(t4, font).Width);
//            lmax = lts.Max();
//            if (Y < (Pic_YL - Pic_YU) / 2)
//            {
//                yt = Y - NP.LableTextHeight / 2;
//                if (X < NP.PicWidth / 2) xt = X + NP.LableTextHeight / 2; ///第二象限
//                else xt = X - lmax - NP.LableTextHeight / 2;                 ///第一象限
//            }
//            else
//            {
//                yt = Y - (int)(NP.LableTextHeight * 5.5);
//                if (X < NP.PicWidth / 2) xt = X + NP.LableTextHeight / 2; ///第三象限
//                else xt = X - lmax - NP.LableTextHeight / 2;                 ///第四象限
//            }
//            ///
//            Gf.DrawString(t1, font, brush, new PointF(xt, yt));
//            if (NP.ShowMaxMinNode == true)
//            {
//                yt = yt + (int)(1.5 * NP.LableTextHeight);
//                Gf.DrawString(t2, font, brush, new PointF(xt, yt));
//                yt = yt + (int)(1.5 * NP.LableTextHeight);
//                Gf.DrawString(t3, font, brush, new PointF(xt, yt));
//                yt = yt + (int)(1.5 * NP.LableTextHeight);
//                Gf.DrawString(t4, font, brush, new PointF(xt, yt));
//            }

//            xt = X - NP.LableTextHeight / 4;
//            yt = Y - NP.LableTextHeight / 4;
//            Gf.FillEllipse(brush, new Rectangle(xt, yt, NP.LableTextHeight / 2, NP.LableTextHeight / 2));

//            //==========Draw Min========//
//            t1 = "Min.= " + Math.Round(RESM[0], 2).ToString();
//            t2 = "Node: " + NodeM[0].ToString();
//            t3 = "x = " + Math.Round(Mesh.Nodes[NodeM[0]].X, 2).ToString();
//            t4 = "y = " + Math.Round(Mesh.Nodes[NodeM[0]].Y, 2).ToString();
//            lts[0] = (int)(Gf.MeasureString(t1, font).Width);
//            lts[1] = (int)(Gf.MeasureString(t2, font).Width);
//            lts[2] = (int)(Gf.MeasureString(t3, font).Width);
//            lts[3] = (int)(Gf.MeasureString(t4, font).Width);
//            lmax = lts.Max();
//            ///
//            X = (int)((Mesh.Nodes[NodeM[0]].X - Mesh.Xmin) / DrawRate[0] + DrawRate[1]);
//            Y = (int)(Pic_YL - (Mesh.Nodes[NodeM[0]].Y - Mesh.Ymin) / DrawRate[0] - DrawRate[2]);
//            if (Y < (Pic_YL - Pic_YU) / 2)
//            {
//                yt = Y - NP.LableTextHeight / 2;                          ///第二象限
//                if (X < NP.PicWidth / 2) xt = X + NP.LableTextHeight / 2; ///第一象限                
//                else xt = X - lmax - NP.LableTextHeight / 2;
//            }
//            else
//            {
//                yt = Y - (int)(NP.LableTextHeight * 5.5);                 ///第三象限                
//                if (X < NP.PicWidth / 2) xt = X + NP.LableTextHeight / 2; ///第四象限                
//                else xt = X - lmax - NP.LableTextHeight / 2;
//            }
//            ///
//            Gf.DrawString(t1, font, brush, new PointF(xt, yt));
//            if (NP.ShowMaxMinNode == true)
//            {
//                yt = yt + (int)(1.5 * NP.LableTextHeight);
//                Gf.DrawString(t2, font, brush, new PointF(xt, yt));
//                yt = yt + (int)(1.5 * NP.LableTextHeight);
//                Gf.DrawString(t3, font, brush, new PointF(xt, yt));
//                yt = yt + (int)(1.5 * NP.LableTextHeight);
//                Gf.DrawString(t4, font, brush, new PointF(xt, yt));
//            }

//            xt = X - NP.LableTextHeight / 4;
//            yt = Y - NP.LableTextHeight / 4;
//            Gf.FillEllipse(brush, new Rectangle(xt, yt, NP.LableTextHeight / 2, NP.LableTextHeight / 2));

//        }
//        #endregion

//        #region 节点转换
//        /// 将实际的节点值转换为画图用的Point值
//        /// <summary>
//        /// 将实际的节点值转换为画图用的Point值
//        /// </summary>
//        /// <param name="Ns"></param>
//        /// <param name="Xmin">坐标原点</param>
//        /// <param name="Ymin">坐标原点</param>
//        /// <param name="Ratio">转换比例</param>
//        /// <param name="Tail_Check">是否是首尾重复点集</param>
//        /// <param name="Pic_YLim">因为Y方向是相反的，所以需要输入图片上Y方向的最大值</param>
//        private static Point[] Nodes_To_Points(ref WNode2D[] Ns, double Xmin, double Ymin, ref double[] Ratio, bool Tail_Check, double Pic_YLim)
//        {
//            Point[] Ps;
//            if (Tail_Check == true)
//            {
//                Ps = new Point[Ns.Length - 1];
//                for (int i = 0; i < Ns.Length - 1; i++)
//                    Ps[i] = new Point((int)((Ns[i].X - Xmin) / Ratio[0] + Ratio[1]), (int)(Pic_YLim - (Ns[i].Y - Ymin) / Ratio[0] - Ratio[2]));
//                //Ps[i] = new Point((int)((Ns[i].X - Xmin) / Ratio[0] + Ratio[1]), (int)((Ns[i].Y - Ymin) / Ratio[0] + Ratio[2] + Pic_YOff));
//            }
//            else
//            {
//                Ps = new Point[Ns.Length];
//                for (int i = 0; i < Ns.Length; i++)
//                    Ps[i] = new Point((int)((Ns[i].X - Xmin) / Ratio[0] + Ratio[1]), (int)(Pic_YLim - (Ns[i].Y - Ymin) / Ratio[0] - Ratio[2]));
//                //Ps[i] = new Point((int)((Ns[i].X - Xmin) / Ratio[0] + Ratio[1]), (int)((Ns[i].Y - Ymin) / Ratio[0] + Ratio[2] + Pic_YOff));
//            }
//            return Ps;
//        }

//        /// 将单个节点转换为画图用的Point
//        /// <summary>
//        /// 将单个节点转换为画图用的Point
//        /// </summary>
//        /// <param name="Node"></param>
//        /// <param name="Xmin"></param>
//        /// <param name="Ymin"></param>
//        /// <param name="Ratio"></param>
//        /// <param name="Pic_YLim"></param>
//        /// <returns></returns>
//        private static Point Node_To_Point(ref WNode2D Node, double Xmin, double Ymin, ref double[] Ratio, double Pic_YLim)
//        {
//            return new Point((int)((Node.X - Xmin) / Ratio[0] + Ratio[1]), (int)(Pic_YLim - (Node.Y - Ymin) / Ratio[0] - Ratio[2]));
//        }
//        #endregion

//        #region 线性插值方法绘制
//        /// 求出每两个节点之间的中间节点
//        private static void NodesInter_Compute(ref WNode2D N1, ref WNode2D N2,
//                                               double Res1, double Res2,
//                                               int CI1, int CI2, int CI_max,
//                                               ref double[] Res_Each,
//                                               ref WNode2D[] Ns_O, ref int[] CIs_O, int[] CIs, double[] ERes)
//        {
//            int Q = Ns_O.Length;
//            Q++;
//            Array.Resize<WNode2D>(ref Ns_O, Q);
//            Array.Resize<int>(ref CIs_O, Q);
//            Ns_O[Q - 1] = N1;
//            if (CI1 == CI_max)
//                CIs_O[Q - 1] = CI1 + 1;
//            else
//                CIs_O[Q - 1] = CI1;
//            if (CI1 == CI2)     /////如果两头一样就不要继续了
//                return;

//            double R1, R2, Ratio;
//            if (CI2 > CI1)
//                for (int i = 1; i <= CI2 - CI1; i++)
//                {
//                    R1 = Math.Abs(Res1 - Res_Each[CI1 + i]);
//                    R2 = Math.Abs(Res2 - Res_Each[CI1 + i]);
//                    Ratio = R1 / (R1 + R2);

//                    Q++;
//                    Array.Resize<WNode2D>(ref Ns_O, Q);
//                    Array.Resize<int>(ref CIs_O, Q);
//                    Ns_O[Q - 1] = new WNode2D(N1.X + (N2.X - N1.X) * Ratio, N1.Y + (N2.Y - N1.Y) * Ratio);
//                    CIs_O[Q - 1] = CI1 + i;
//                }
//            if (CI2 < CI1)
//                for (int i = CI1 - CI2; i >= 1; i--)
//                {
//                    R1 = Math.Abs(Res1 - Res_Each[CI2 + i]);
//                    R2 = Math.Abs(Res2 - Res_Each[CI2 + i]);
//                    Ratio = R1 / (R1 + R2);

//                    Q++;
//                    Array.Resize<WNode2D>(ref Ns_O, Q);
//                    Array.Resize<int>(ref CIs_O, Q);
//                    Ns_O[Q - 1] = new WNode2D(N1.X + (N2.X - N1.X) * Ratio, N1.Y + (N2.Y - N1.Y) * Ratio);
//                    CIs_O[Q - 1] = CI2 + i;
//                }
//        }

//        /// 通过结果数值求出Color Index
//        /// <summary>
//        /// 通过结果数值求出Color Index
//        /// </summary>
//        /// <param name="Re">输入的结果</param>
//        /// <param name="Color_Max">最大颜色对应的数值</param>
//        /// <param name="Color_Min">最小颜色对应的数值</param>
//        /// <param name="Res_Per">每个颜色的数值跨度</param>
//        /// <returns></returns>
//        private static int Color_Index(double Re, double Color_Max, double Color_Min, double Res_Per)
//        {
//            if (Re <= Color_Min)
//                return 0;
//            if (Re >= Color_Max)
//                return (int)((Color_Max - Color_Min) / Res_Per) - 1;
//            return (int)(Math.Ceiling((Re - Color_Min) / Res_Per)) - 1;
//        }

//        /// <summary>
//        /// 求节点集围成的单元的面积
//        /// </summary>
//        /// <param name="Ns">输入节点，注意从1开始</param>
//        /// <returns></returns>
//        private static double Element_Area(ref WNode2D[] Nodes)
//        {
//            return 0;
//            //if (Nodes.Length == 4)
//            //    return Math.Abs((Nodes[1].X - Nodes[2].X) * (Nodes[3].Y - Nodes[2].Y) - (Nodes[1].Y - Nodes[2].Y) * (Nodes[3].X - Nodes[2].X)) / 2;
//            //return Math.Abs((Nodes[1].X - Nodes[2].X) * (Nodes[3].Y - Nodes[2].Y) - (Nodes[1].Y - Nodes[2].Y) * (Nodes[3].X - Nodes[2].X)) / 2 +
//            //       Math.Abs((Nodes[1].X - Nodes[4].X) * (Nodes[3].Y - Nodes[4].Y) - (Nodes[1].Y - Nodes[4].Y) * (Nodes[3].X - Nodes[4].X)) / 2;
//        }
//        #endregion
//    }
//}