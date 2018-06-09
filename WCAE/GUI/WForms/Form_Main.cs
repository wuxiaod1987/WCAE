using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using WCAE.WRenderer;
using WCAE.WForms.IO;
using WCAE.WGeos2D;
using WCAE.WGeos2D.Entities;
using WCAE.WGeos2D.IO;
using WCAE.WMesh2D;
using WCAE.WMesh2D.IO;
using WCAE.WMesh2D.Entities;

namespace WCAE.WForms
{
    public partial class Form_Main :Form 
    {
        #region "声明"
        InputGeometry input;
        Mesh mesh;
        WGeometry2D WGC;
        List<WMesh2D_Mesh> WMC;

        RenderManager renderManager;
        RenderData renderData;
        #endregion

        public Form_Main()
        {
            WMC = new List<WMesh2D_Mesh>();
            WGC = new WGeometry2D();
            InitializeComponent();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            renderManager = new RenderManager();
            renderManager.CreateDefaultControl();

            var control = renderManager.RenderControl;
            this.splitContainer1.Panel2.Controls.Add(control);

            renderManager.Initialize();
            renderData = new RenderData();
        }

        #region "打开Triangle"
        private void 打开TriangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenTriangleWithDialog();
        }

        private void OpenTriangleWithDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Triangle file (*.node;*.poly)|*.node;*.poly";
            if (ofd.ShowDialog() == DialogResult.OK)
                OpenTriangle(ofd.FileName);
        }

        private bool OpenTriangle(string filename)
        {
            if (FileProcessor.ContainsMeshData(filename))
            {
                input = null;
                mesh = FileProcessor.Import(filename);

                if (mesh != null)
                    HandleMeshImport();
                return true;
            }

            input = FileProcessor.Read(filename);

            if (input != null)
                HandleNewInput();

            return true;
        }
        #endregion

        #region "打开WGC"
        private void 打开WGOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "WGeometry file (*.wgo)|*.wgo";
            if (ofd.ShowDialog() == DialogResult.OK)
                WGeo2DFile.Read_WGeoFile(ofd.FileName, ref WGC);
            HandleWgeo2dInput();
        }
        #endregion

        #region "打开WMC"
        private void 打开MeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "WMesh file (*.mesh)|*.mesh";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                WMC.Add(new WMesh2D_Mesh("Mesh"));
                WMC[0].Input_FromFile(ofd.FileName);
            }
            HandleWmesh2dInput();
        }
        #endregion

        #region 输入及更新
        private void HandleWmesh2dInput()
        {
            renderData.SetWMesh2DDatas(ref WMC);
            renderManager.SetData(renderData);
        }

        private void HandleWgeo2dInput()
        {
            renderData.SetWGeo2DDatas(ref WGC);
            renderManager.SetData(renderData);
        }

        private void HandleNewInput()
        {
            renderData.SetInputGeometry(input);
            renderManager.SetData(renderData);
        }

        private void HandleMeshImport()
        {
            // Render mesh
            renderData.SetMesh(mesh);
            renderManager.SetData(renderData);
            //renderManager.Initialize();
        }

        private void HandleMeshUpdate()
        {
            // Render mesh
            renderData.SetMesh(mesh);
            renderManager.SetData(renderData);
        }
        #endregion

        #region "Resize"
        bool isResizing = false;
        Size oldClientSize;

        private void Form_Main_Resize(object sender, EventArgs e)
        {
            if (!isResizing)
            {
                renderManager.HandleResize();
            }

        }

        private void Form_Main_ResizeBegin(object sender, EventArgs e)
        {
            isResizing = true;
        }

        private void Form_Main_ResizeEnd(object sender, EventArgs e)
        {
            isResizing = false;

            if (this.ClientSize != this.oldClientSize)
            {
                this.oldClientSize = this.ClientSize;
                renderManager.HandleResize();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var container = this.splitContainer1.Panel2.ClientRectangle;

            System.Drawing.Point pt = e.Location;
            pt.Offset(-splitContainer1.SplitterDistance, 0);

            if (container.Contains(pt))
            {
                renderManager.Zoom(((float)pt.X) / container.Width,
                    ((float)pt.Y) / container.Height, e.Delta);
            }
            base.OnMouseWheel(e);
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            WGC.Del_Geo(WGC.Entities[0]);
            renderData.SetWGeo2DDatas(ref WGC);
            renderManager.HandleUpdate();
            button1.Text = "OK";
        }
    }
}
