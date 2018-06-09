using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using WCAE.WGeos2D.Entities;

namespace WCAE.WGeos2D.IO 
{
    /// 读写wgo文件
    /// <summary>
    /// 读写wgo文件，并定义文件格式
    /// </summary>
    public static class WGeo2DFile
    {
        /// 从wgo文件输入实体，添加至WGO中
        /// <summary>
        /// 从wgo文件输入实体，添加至WGO中
        /// </summary>
        /// <returns>true表示文件包含参数，false表示文件不包含参数</returns>
        public static bool Read_WGeoFile(string FileName, WGeometry2D WGO)
        {
            StreamReader sr = new StreamReader(FileName);
            sr.ReadLine();
            sr.ReadLine();
            string t = sr.ReadLine();
            while (t != "*****************")
            {
                if (t == "")
                    continue;
                if (t[0] == '*')
                    continue;
                if (t == "Start")
                    Read_SingleEntitie(ref sr, ref WGO);
                t = sr.ReadLine();
            }
            if (sr.Peek() == -1)
            {
                sr.Close();
                return false;
            }
            else
            {
                sr.Close();
                return true;
            }
        }

        /// 从wgo文件输入实体，添加至WGO中
        /// <summary>
        /// 从wgo文件输入实体，添加至WGO中
        /// </summary>
        /// <returns>true表示文件包含参数，false表示文件不包含参数</returns>
        public static bool Read_WGeoFile(WGeometry2D WGO, ref StreamReader sr)
        {
            sr.ReadLine();
            sr.ReadLine();
            string t = sr.ReadLine();
            while (t != "*****************")
            {
                if (t == "")
                    continue;
                if (t[0] == '*')
                    continue;
                if (t == "Start")
                    Read_SingleEntitie(ref sr, ref WGO);
                t = sr.ReadLine();
            }
            if (sr.Peek() == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #region 临时变量
        static string tLayer;
        static Color tColor;
        static float tWidth;
        static double x1, y1, x2, y2;
        static string[] t_1;
        static List<WPoint2D> tPs;
        static WPoint2D[] tPst;
        static WPoint2D tP;
        #endregion

        private static void Read_SingleEntitie(ref StreamReader sr, ref WGeometry2D WGO)
        {
            string Kind = sr.ReadLine();
            switch (Kind)
            {
                case "Line":
                    Read_Line(ref sr, ref WGO);
                    break;
                case "PolyLine":
                    Read_PolyLine(ref sr, ref WGO);
                    break;
                case "Circle":
                    Read_Circle(ref sr, ref WGO);
                    break;
                case "Text":
                    //Read_Text(ref sr, ref WGO);
                    break;
                default:
                    while (true)
                        if (sr.ReadLine() == "End")
                            break;
                    break;
            }


        }

        private static void Read_Line(ref StreamReader sr, ref WGeometry2D WGC)
        {
            tLayer = sr.ReadLine();
            t_1 = sr.ReadLine().Split(',');
            tColor = Color.FromArgb(Convert.ToInt16(t_1[0]), Convert.ToInt16(t_1[1]), Convert.ToInt16(t_1[2]));
            tWidth = Convert.ToSingle(sr.ReadLine());
            t_1 = sr.ReadLine().Split(',');
            x1 = Convert.ToDouble(t_1[0]);
            y1 = Convert.ToDouble(t_1[1]);

            t_1 = sr.ReadLine().Split(',');
            x2 = Convert.ToDouble(t_1[0]);
            y2 = Convert.ToDouble(t_1[1]);
            /////
            if (Math.Abs(x1 - x2) <= WGeos2D_Paras.E_Merge &&
                Math.Abs(y1 - y2) <= WGeos2D_Paras.E_Merge)
            {
                sr.ReadLine();
                return;
            }

            WLine2D L = new WLine2D(WGC.PsList.Add(x1, y1, true), WGC.PsList.Add(x2, y2, true));
            L.Kind = GeoKind.Line;
            L.Layer = tLayer;
            L.Color = tColor;
            L.LineWidth = tWidth;
            L.Sort = ShowSort._5;
            WGC.Add_Geo(L);
            sr.ReadLine();
        }

        private static void Read_PolyLine(ref StreamReader sr, ref WGeometry2D WGC)
        {
            tLayer = sr.ReadLine();
            t_1 = sr.ReadLine().Split(',');
            tColor = Color.FromArgb(Convert.ToInt16(t_1[0]), Convert.ToInt16(t_1[1]), Convert.ToInt16(t_1[2]));
            tWidth = Convert.ToSingle(sr.ReadLine());
            int Q = Convert.ToInt32(sr.ReadLine());
            tPs = new List<WPoint2D>();
            tPst = new WPoint2D[Q];

            for (int i = 0; i < Q; i++)
            {
                t_1 = sr.ReadLine().Split(',');
                tPst[i] = new WPoint2D(Convert.ToDouble(t_1[0]), Convert.ToDouble(t_1[1]));
            }

            tPs.Add(WGC.PsList.Add(tPst[0].X, tPst[0].Y, true));

            for (int i = 1; i < Q - 1; i++)
            {
                if (Near_Check(tPst[i], tPst[i - 1]) == false && Near_Check(tPst[i], tPst[i + 1]) == false)
                    tPs.Add(WGC.PsList.Add(tPst[i].X, tPst[i].Y, false));
            }
            tPs.Add(WGC.PsList.Add(tPst[Q - 1].X, tPst[Q - 1].Y, true));
            tPst = null;
            /////
            if (tPs.Count < 2)
            {
                sr.ReadLine();
                return;
            }
            WPolyLine2D PL = new WPolyLine2D(tPs, ref WGC);
            PL.Kind = GeoKind.PolyLine;
            PL.Layer = tLayer;
            PL.Color = tColor;
            PL.LineWidth = tWidth;
            PL.Sort = ShowSort._5;
            WGC.Add_Geo(PL);
            sr.ReadLine();
        }

        private static bool Near_Check(WPoint2D P1, WPoint2D P2)
        {
            if (Math.Abs(P1.X - P2.X) < WGeos2D_Paras.E_Merge && Math.Abs(P1.Y - P2.Y) < WGeos2D_Paras.E_Merge)
                return true;
            else
                return false;
        }

        private static void Read_Circle(ref StreamReader sr, ref WGeometry2D WGC)
        {
            tLayer = sr.ReadLine();
            t_1 = sr.ReadLine().Split(',');
            tColor = Color.FromArgb(Convert.ToInt16(t_1[0]), Convert.ToInt16(t_1[1]), Convert.ToInt16(t_1[2]));
            tWidth = Convert.ToSingle(sr.ReadLine());
            t_1 = sr.ReadLine().Split(',');
            x1 = Convert.ToDouble(t_1[0]);
            y1 = Convert.ToDouble(t_1[1]);
            x2 = Convert.ToDouble(sr.ReadLine());

            WCircle2D C = new WCircle2D(WGC.PsList.Add(x1, y1, true), x2, ref WGC);
            C.Kind = GeoKind.Circle;
            C.Layer = tLayer;
            C.Color = tColor;
            C.LineWidth = tWidth;
            C.Sort = ShowSort._5;
            WGC.Add_Geo(C);
            sr.ReadLine();
        }

        private static void Read_Text(ref StreamReader sr, ref WGeometry2D WGO)
        {
            tLayer = sr.ReadLine();
            t_1 = sr.ReadLine().Split(',');
            tColor = Color.FromArgb(Convert.ToInt16(t_1[0]), Convert.ToInt16(t_1[1]), Convert.ToInt16(t_1[2]));
        }
    }
}
