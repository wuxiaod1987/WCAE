using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WCAE.WRenderer
{
    public class RenderParameter
    {
        #region Triangle Renderer
        internal Color background;
        internal SolidBrush point;
        internal SolidBrush steinerPoint;
        internal SolidBrush triangle;
        internal Pen line;
        internal Pen segment;
        internal Pen voronoiLine;

        internal SolidBrush[] regions;
        internal Dictionary<int, int> regionMap;

        // Change or add as many colors as you like...
        private static Color[] regionColors = {
            Color.FromArgb(20,   0, 255,   0),
            Color.FromArgb(20, 255, 255,   0),
            Color.FromArgb(20, 255,   0,   0),
            Color.FromArgb(20,   0,   0, 255)
        };

        #region Properties

        public Color Background
        {
            get { return background; }
            set { background = value; }
        }

        public SolidBrush Point
        {
            get { return point; }
            set
            {
                if (point != null) point.Dispose();
                point = value;
            }
        }

        public SolidBrush SteinerPoint
        {
            get { return steinerPoint; }
            set
            {
                if (steinerPoint != null) steinerPoint.Dispose();
                steinerPoint = value;
            }
        }

        public SolidBrush Triangle
        {
            get { return triangle; }
            set
            {
                if (triangle != null) triangle.Dispose();
                triangle = value;
            }
        }

        public Pen Line
        {
            get { return line; }
            set
            {
                if (line != null) line.Dispose();
                line = value;
            }
        }

        public Pen Segment
        {
            get { return segment; }
            set
            {
                if (segment != null) segment.Dispose();
                segment = value;
            }
        }

        public Pen VoronoiLine
        {
            get { return voronoiLine; }
            set
            {
                if (voronoiLine != null) voronoiLine.Dispose();
                voronoiLine = value;
            }
        }

        #endregion

        /// <summary>
        /// Setup a region map.
        /// </summary>
        /// <param name="partition">Mesh partition (size = number of triangles in mesh)</param>
        /// <param name="size">Number of partitions / regions.</param>
        public void MakeRegionMap(int[] partition, int size)
        {
            if (regions == null || regions.Length != size)
            {
                int n = regionColors.Length;

                regions = new SolidBrush[size];

                for (int j = 0; j < size; j++)
                {
                    regions[j] = new SolidBrush(regionColors[j % n]);
                }
            }

            if (regionMap == null)
            {
                regionMap = new Dictionary<int, int>(size);
            }
            else
            {
                regionMap.Clear();
            }

            int k = 0;
            for (int i = 0; i < partition.Length; i++)
            {
                if (!regionMap.ContainsKey(partition[i]))
                {
                    regionMap.Add(partition[i], k++);
                }
            }
        }

        /// <summary>
        /// Get the color defined for given region.
        /// </summary>
        public Brush GetRegionBrush(int region)
        {
            if (regionMap == null)
            {
                return triangle;
            }

            int k;
            if (regionMap.TryGetValue(region, out k))
            {
                return regions[k];
            }

            return triangle;
        }
        #endregion

        #region Initial 
        public RenderParameter(Theme theme)
        {
            if (theme == Theme.Default)
            {
                this.Background = Color.FromArgb(0, 0, 0);
                this.BrushHP = new SolidBrush(Color.Green);
                this.BrushSP = new SolidBrush(Color.Aqua);
                this.BrushN = new SolidBrush(Color.Green);
                this.BrushE = Brushes.Aqua;
                this.PenES = new Pen(Color.White, 0.1f);
                this.PenEB = new Pen(Color.Red, 1f);
                this.SnapGeo2DPen = new Pen(SnapGeo2DColor, SnapGeo2DWidth);
                this.BrushSnapP = new SolidBrush(SnapGeo2DColor);

                this.Point = new SolidBrush(Color.Green);
                this.SteinerPoint = new SolidBrush(Color.Peru);
                this.Triangle = new SolidBrush(Color.Black);
                this.Line = new Pen(Color.FromArgb(30, 30, 30));
                this.Segment = new Pen(Color.DarkBlue);
                this.VoronoiLine = new Pen(Color.FromArgb(40, 50, 60));
                this.Cood_Text = Brushes.White;
                this.Cood_Font = new Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            }
            if (theme == Theme.Light)
            {
                this.Background = Color.White;
                this.Point = new SolidBrush(Color.FromArgb(60, 80, 120));
                this.BrushHP = new SolidBrush(Color.FromArgb(60, 80, 120));
                this.BrushSP = new SolidBrush(Color.FromArgb(60, 80, 120));
                this.BrushN = new SolidBrush(Color.FromArgb(60, 80, 120));
                this.BrushE = Brushes.Aqua;

                this.PenES = new Pen(Color.Black, 0.1f);
                this.PenEB = new Pen(Color.Gray, 0.3f);
                this.SnapGeo2DPen = new Pen(SnapGeo2DColor, SnapGeo2DWidth);
                this.BrushSnapP = new SolidBrush(SnapGeo2DColor);

                this.SteinerPoint = new SolidBrush(Color.DarkGreen);
                this.Triangle = new SolidBrush(Color.FromArgb(230, 240, 250));
                this.Line = new Pen(Color.FromArgb(150, 150, 150));
                this.Segment = new Pen(Color.SteelBlue);
                this.VoronoiLine = new Pen(Color.FromArgb(160, 170, 180));
                this.Cood_Text = Brushes.Black;
                this.Cood_Font = new Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            }
        }
        #endregion
        /// <summary>
        /// 渲染的抗锯齿模式
        /// </summary>
        public SmoothingMode Smooth = SmoothingMode.Default; /////SmoothingMode.AntiAlias;
        /// <summary>
        /// 显示当前已经开启的对象捕捉模式组合，可以同时打开几种对象捕捉模式
        /// </summary>
        public List<SnapMode> SModes = new List<SnapMode>();

        #region 显示开关
        public RenderMode Mode = RenderMode.Geo2D;
        /// <summary>
        /// 修改渲染对象模式
        /// </summary>
        /// <param name="Mode"></param>
        public void Mode_Swtich(RenderMode Mode)
        {
            if (Mode == RenderMode.Triangle)
            {
                SModes.Clear();
                Show_Triangle = true;
                Show_WGeo2D = false;
                Show_WRim2D = false;
                Show_WMesh2D = false;
                Show_Res = false;
                this.Mode = Mode;
                return;
            }
            if (Mode == RenderMode.Geo2D)
            {
                SModes.Clear();
                SModes.Add(SnapMode.Geo2DEntity);
                //SModes.Add(SnapMode.Geo2DPoint);
                Show_Triangle = false;
                Show_WGeo2D = true;
                Show_WRim2D = false;
                Show_WMesh2D = false;
                Show_Res = false;
                this.Mode = Mode;
                return;
            }
            if (Mode == RenderMode.Geo2DRim)
            {
                SModes.Clear();
                SModes.Add(SnapMode.Geo2DRim);
                Show_Triangle = false;
                Show_WGeo2D = false;
                Show_WRim2D = true;
                Show_WMesh2D = false;
                Show_Res = false;
                this.Mode = Mode;
                return;
            }
            if (Mode == RenderMode.Mesh2D)
            {
                Show_Triangle = false;
                Show_WGeo2D = false;
                Show_WRim2D = false;
                Show_WMesh2D = true;
                Show_Res = false;
                this.Mode = Mode;
                return;
            }
            if (Mode == RenderMode.Result)
            {
                Show_Triangle = false;
                Show_WGeo2D = false;
                Show_WRim2D = false;
                Show_WMesh2D = false;
                Show_Res = true;
                this.Mode = Mode;
                return;
            }
        }

        /// <summary>
        /// 几何硬点显示开关
        /// </summary>
        public bool SwitchHP = true;      /////几何硬点显示开关
        /// <summary>
        /// 几何中间点显示开关
        /// </summary>
        public bool SwitchSP = false;     /////几何中间点显示开关
        /// <summary>
        /// 节点显示开关
        /// </summary>
        public bool SwitchN = false;      /////节点显示开关
        /// <summary>
        /// 是否打开对象捕捉
        /// </summary>
        public bool SwitchSnap = true;    /////是否打开对象捕捉
        /// <summary>
        /// 是否打开Triangle显示
        /// </summary>
        internal bool Show_Triangle = true;
        /////
        internal bool Show_WGeo2D = true;     /////显示几何
        internal bool Show_WRim2D = false;    /////显示Rim的开关
        internal bool Show_WMesh2D = true;
        internal bool Show_Res = false;
        #endregion

        #region 显示属性
        /// <summary>
        /// 点坐标的Brush
        /// </summary>
        public Brush Cood_Text;     /////点坐标的Brush
        /// <summary>
        /// 点坐标的Font
        /// </summary>
        public Font Cood_Font;      /////点坐标的Font
        /// <summary>
        /// 几何硬点的Brush
        /// </summary>
        public SolidBrush BrushHP;  /////几何硬点的Brush
        /// <summary>
        /// 几何软点的Brush
        /// </summary>
        public SolidBrush BrushSP;  /////几何软点的Brush
        /// <summary>
        /// 节点的Brush
        /// </summary>
        public SolidBrush BrushN;   /////节点的Brush
        /// <summary>
        /// 单元的Brush
        /// </summary>
        public Brush BrushE;        /////单元的Brush
        /// <summary>
        /// Shell单元边界
        /// </summary>
        public Pen PenES;           /////Shell单元边界
        /// <summary>
        /// Beam单元
        /// </summary>
        public Pen PenEB;           /////Beam单元

        /// <summary>
        /// 几何硬点的显示半径
        /// </summary>
        public float Rhp = 5;       /////几何硬点的显示半径
        /// <summary>
        /// 几何软点的显示半径
        /// </summary>
        public float Rsp = 3;       /////几何软点的显示半径
        /// <summary>
        /// 节点的显示半径
        /// </summary>
        public float Rn = 2;        /////节点的显示半径
        #endregion

        #region 对象捕捉相关
        /// <summary>
        /// 捕捉几何Entity的显示颜色
        /// </summary>
        public Color SnapGeo2DColor = Color.Azure;    /////捕捉几何Entity的显示颜色
        /// <summary>
        /// 捕捉Point的显示半径
        /// </summary>
        public float RSnapP = 10;                     /////捕捉Point的显示半径
        /// <summary>
        /// 捕捉Point的Brush
        /// </summary>
        public SolidBrush BrushSnapP;                 /////捕捉Point的Brush
        /// <summary>
        /// 捕捉几何Entity的显示线宽
        /// </summary>
        public int SnapGeo2DWidth = 5;                /////捕捉几何Entity的显示线宽
        /// <summary>
        /// 已选择对象的边界Pen
        /// </summary>
        public Pen SnapGeo2DPen;                      /////已选择对象的边界Pen
        #endregion

        public string Pic_FileNeme;
    }
}
