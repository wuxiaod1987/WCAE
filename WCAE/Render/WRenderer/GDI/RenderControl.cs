using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Geometry;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;
using System.Diagnostics;

namespace WCAE.WRenderer
{
    public class RenderControl : Control, IMeshRenderer
    {
        #region Triangle
        MeshRenderer meshRenderer;
        VoronoiRenderer voronoiRenderer;
        public bool ShowVoronoi
        {
            get { return showVoronoi; }
            set
            {
                showVoronoi = value;
                this.Render();
            }
        }

        public bool ShowRegions
        {
            get { return showRegions; }
            set
            {
                showRegions = value;
                this.Render();
            }
        }

        bool showVoronoi = false;
        bool showRegions = true;
        #endregion

        IWRender_GUI Father;

        #region Render 变量
        /// <summary>
        /// Gets the currently displayed <see cref="RenderData"/>.
        /// </summary>
        public RenderData Data
        {
            get { return data; }
        }
        RenderData data;

        private BufferedGraphics buffer;
        private BufferedGraphicsContext context;

        ScreenTrans trans;
        /////
        WGeoRenderer wgeoRenderer;
        WGeoRimRenderer wgeoRRenderer;
        WMeshRenderer wmeshRenderer;
        WResRenderer wresRenderer;

        RenderParameter RParas;
        ObjSnap objSnap { get { return data.objSnap; } }
        bool initialized = false;

        string coordinate = String.Empty;
        #endregion 

        #region 临时变量
        Graphics G;                  /////用于绘制图形的图形工具
        PointF Pos_Mouse;            /////鼠标指针的位置，按照比例显示
        PointF Pos_Orin, Pos_Move;   /////用于拖动图形的临时鼠标位置信息
        bool Move_Mouse = false;     /////判断鼠标是否移动
        float Rate2ViewPort;         /////图形到ViewPort的比例
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderControl" /> class.
        /// </summary>
        public RenderControl(IWRender_GUI Father)
        {
            this.Father = Father;
            SetStyle(ControlStyles.ResizeRedraw, true);

            trans = new ScreenTrans(true);
            context = new BufferedGraphicsContext();
            data = new RenderData();
        }

        /// <summary>
        /// Initialize the graphics buffer (should be called in the forms load event).
        /// </summary>
        public void Initialize(ref RenderParameter RParas)
        {
            this.RParas = RParas;
            Initialize();
        }
        public void Initialize()
        {
            if (RParas == null)
                RParas = new RenderParameter(Theme.Default);

            wresRenderer = new WResRenderer();

            this.BackColor = RParas.Background;
            this.Dock = DockStyle.Fill;
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "renderControl1";
            this.TabIndex = 0;
            this.Text = "meshRenderer1";

            trans.Initialize(this.ClientRectangle);
            InitializeBuffer();
            G = buffer.Graphics;

            initialized = true;

            this.Invalidate();
        }

        private void InitializeBuffer()
        {
            if (this.Width > 0 && this.Height > 0)
            {
                if (buffer != null)
                {
                    if (this.ClientRectangle == buffer.Graphics.VisibleClipBounds)
                    {
                        this.Invalidate();
                        return;
                    }

                    buffer.Dispose();
                }

                buffer = context.Allocate(Graphics.FromHwnd(this.Handle), this.ClientRectangle);

                if (initialized)
                {
                    this.Render();
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
        #endregion

        #region Datas
        public void SetData(RenderData data)
        {
            this.data = data;

            if (data.Imped_WGeo2D == true)
            {
                wgeoRenderer = new WGeoRenderer(data, RParas);
                wgeoRRenderer = new WGeoRimRenderer(data);
            }

            if (data.Imped_WMesh2D == true)
                wmeshRenderer = new WMeshRenderer(ref data);

            if (data.Imped_Triangle == true)
            {
                meshRenderer = new MeshRenderer(data, RParas);

                this.showVoronoi = data.VoronoiPoints != null;

                if (showVoronoi)
                    voronoiRenderer = new VoronoiRenderer(data);
            }

            // Reset the zoom on new data
            trans.Initialize(this.ClientRectangle, data.Bounds);

            initialized = true;

            this.Render();
        }
        #endregion

        #region Render
        private void Render()
        {
            #region Initial
            coordinate = String.Empty;

            if (buffer == null)
                return;

            G = buffer.Graphics;
            G.Clear(this.BackColor);

            if (!initialized || data == null)
                return;

            G.SmoothingMode = RParas.Smooth;
            #endregion

            #region Triangle
            if (data.Imped_Triangle == true && RParas.Show_Triangle == true)
            {
                if (voronoiRenderer != null && this.showVoronoi)
                {
                    meshRenderer.RenderMesh(G, trans);
                    voronoiRenderer.Render(G, trans, RParas);
                    meshRenderer.RenderGeometry(G, trans);
                    this.Invalidate();
                    return;
                }

                if (meshRenderer != null)
                {
                    meshRenderer.Render(G, trans, showRegions);
                    this.Invalidate();
                    return;
                }
            }
            #endregion
            if (data.Imped_WGeo2D == true && RParas.Show_WGeo2D == true)
                wgeoRenderer.Render(G, trans);

            if (data.Imped_WRim2D == true && RParas.Show_WRim2D == true)
                wgeoRRenderer.Render(G, trans, RParas);
            
            if (data.Imped_WMesh2D == true && RParas.Show_WMesh2D == true)
                wmeshRenderer.Render(G, trans, RParas);

            if (RParas.Show_Res == true)
                wresRenderer.Render(G, RParas, this.Width, this.Height);

            this.Invalidate();
        }
        #endregion

        #region "GUI Reaction"
        /// <summary>
        /// 刷新图面
        /// </summary>
        public void Rerender()
        {
            if (!initialized) return;
            this.Render();
        }

        /// <summary>
        /// Zoom to the given location.
        /// </summary>
        /// <param name="location">The zoom focus.</param>
        /// <param name="delta">Indicates whether to zoom in or out.</param>
        public void Zoom(float x, float y, int delta)
        {
            if (!initialized) return;

            if (trans.ZoomUpdate(delta, x, y))
            {
                // Redraw
                this.Render();
            }
        }

        /// <summary>
        /// Move to the given location.
        /// </summary>
        public void DragMove(float x, float y)
        {
            if (!initialized) return;

            if (trans.MoveUpdate(x, y))
                this.Render();
        }

        /// <summary>
        /// Update graphics buffer and zoom after a resize.
        /// </summary>
        public void HandleResize()
        {
            trans.Initialize(this.ClientRectangle, data.Bounds);
            InitializeBuffer();
        }

        public void HandleUpdate()
        {
            this.Render();
        }
        #endregion

        #region Control overrides
        #region "Paint"
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (!initialized)
            {
                base.OnPaint(pe);
                return;
            }

            buffer.Render();

            if (!String.IsNullOrEmpty(coordinate) && (data.Points != null || RParas.Show_WGeo2D == true))
            {
                G = this.CreateGraphics();
                G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                G.DrawString(coordinate, RParas.Cood_Font, RParas.Cood_Text, 10, 10);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Do nothing
            if (!initialized)
            {
                base.OnPaintBackground(pevent);
            }
        }
        #endregion

        #region "Mouse"
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!initialized) return;

            if (e.Button == MouseButtons.Left)
            {
                Pos_Orin = new PointF(e.X, e.Y);
                Move_Mouse = true;
            }
            base.OnMouseClick(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Move_Mouse = false;
            base.OnMouseUp(e);
        }

        /// 鼠标移动
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Pos_Mouse = trans.ScreenToWorld((float)e.X / this.Width, (float)e.Y / this.Height);

            coordinate = String.Format("X:{0} Y:{1}",
                Convert.ToString(Math.Round(Pos_Mouse.X, 3)),
                Convert.ToString(Math.Round(Pos_Mouse.Y, 3)));

            //objSnap.Snap(data, new WPoint2D(Pos_Mouse.X, Pos_Mouse.Y), RParas.Mode);

            /////用于鼠标拖拽
            if (Move_Mouse == true)
            {
                Rate2ViewPort = this.Width / trans.Viewport.Width;
                Pos_Move = new PointF(e.X, e.Y);

                this.DragMove((Pos_Move.X - Pos_Orin.X) / Rate2ViewPort, (Pos_Orin.Y - Pos_Move.Y) / Rate2ViewPort);
            }
            this.Invalidate();
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var container = this.ClientRectangle;

            System.Drawing.Point pt = e.Location;
            pt.Offset(0, 0);

            if (container.Contains(pt))
            {
                Zoom(((float)pt.X) / container.Width,
                    ((float)pt.Y) / container.Height, e.Delta);
            }
            base.OnMouseWheel(e);
        }

        /// 双击响应
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                trans.ZoomReset();
                this.Render();
            }
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            /////如果对象捕捉没开就直接返回
            if (RParas.SwitchSnap == false)
                objSnap.PointFree = new WPoint2D(Pos_Mouse.X, Pos_Mouse.Y);
            else
            {
                Pos_Mouse = trans.ScreenToWorld((float)e.X / this.Width, (float)e.Y / this.Height);
                objSnap.Snap(data, new WPoint2D(Pos_Mouse.X, Pos_Mouse.Y), RParas.SModes);
            }
            Render();

            Father.Change_SelectItem();
            base.OnMouseClick(e);
        }
        #endregion
        #endregion
    }
}
