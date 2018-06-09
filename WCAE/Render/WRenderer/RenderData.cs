using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet;
using TriangleNet.Tools;
using WCAE.Entities;
using WCAE.WGeos2D.Entities;
using WCAE.WGeos2D;
using WCAE.WMesh2D;
using WCAE.WMesh2D.Entities;

namespace WCAE.WRenderer
{
    public class RenderData
    {
        public BoundingBox Bounds;
        public bool Imped_Triangle = false;
        public bool Imped_WGeo2D
        {
            get 
            {
                if (Entities == null) return false;
                if (Entities.Count == 0) return false;
                return true;
            }
        }
        public bool Imped_WMesh2D
        {
            get
            {
                if (Meshs == null) return false;
                if (Meshs.Count == 0) return false;
                return true;
            }
        }
        public bool Imped_WRim2D
        {
            get
            {
                if (Rims == null) return false;
                if (Rims.Count == 0) return false;
                return true;
            }
        }
        public ObjSnap objSnap = new ObjSnap();

        #region Triangle
        #region "Triangle Datas"
        public float[] Points;
        public uint[] Segments;
        public uint[] Triangles;
        public uint[] MeshEdges;
        public float[] VoronoiPoints;
        public uint[] VoronoiEdges;
        public int[] TrianglePartition;

        public int NumberOfRegions;
        public int NumberOfInputPoints;
        #endregion

        #region "Set Triangle Datas"
        /// <summary>
        /// Copy input geometry data.
        /// </summary>
        public void SetInputGeometry(InputGeometry data)
        {
            Imped_Triangle = true;
            // Clear unused buffers
            this.Segments = null;
            this.Triangles = null;
            this.MeshEdges = null;
            this.VoronoiPoints = null;
            this.VoronoiEdges = null;

            int n = data.Count;
            int i = 0;

            this.NumberOfRegions = data.Regions.Count;
            this.NumberOfInputPoints = n;

            // Copy points
            this.Points = new float[2 * n];
            foreach (var pt in data.Points)
            {
                this.Points[2 * i] = (float)pt.X;
                this.Points[2 * i + 1] = (float)pt.Y;
                i++;
            }

            // Copy segments
            n = data.Segments.Count;
            if (n > 0)
            {
                var segments = new List<uint>(2 * n);
                foreach (var seg in data.Segments)
                {
                    segments.Add((uint)seg.P0);
                    segments.Add((uint)seg.P1);
                }
                this.Segments = segments.ToArray();
            }

            this.Bounds = new BoundingBox(
                (float)data.Bounds.Xmin,
                (float)data.Bounds.Xmax,
                (float)data.Bounds.Ymin,
                (float)data.Bounds.Ymax);
        }

        /// <summary>
        /// Copy mesh data.
        /// </summary>
        public void SetMesh(Mesh mesh)
        {
            Imped_Triangle = true;
            // Clear unused buffers
            this.Segments = null;
            this.VoronoiPoints = null;
            this.VoronoiEdges = null;

            int n = mesh.Vertices.Count;
            int i = 0;

            this.NumberOfInputPoints = mesh.NumberOfInputPoints;

            // Linear numbering of mesh
            mesh.Renumber();

            // Copy points
            this.Points = new float[2 * n];
            foreach (var pt in mesh.Vertices)
            {
                this.Points[2 * i] = (float)pt.X;
                this.Points[2 * i + 1] = (float)pt.Y;
                i++;
            }

            // Copy segments
            n = mesh.Segments.Count;
            if (n > 0 && mesh.IsPolygon)
            {
                var segments = new List<uint>(2 * n);
                foreach (var seg in mesh.Segments)
                {
                    segments.Add((uint)seg.P0);
                    segments.Add((uint)seg.P1);
                }
                this.Segments = segments.ToArray();
            }

            // Copy edges
            var edges = new List<uint>(2 * mesh.NumberOfEdges);

            EdgeEnumerator e = new EdgeEnumerator(mesh);
            while (e.MoveNext())
            {
                edges.Add((uint)e.Current.P0);
                edges.Add((uint)e.Current.P1);
            }
            this.MeshEdges = edges.ToArray();


            if (this.NumberOfRegions > 0)
            {
                this.TrianglePartition = new int[mesh.Triangles.Count];
            }

            i = 0;

            // Copy Triangles
            var triangles = new List<uint>(3 * mesh.Triangles.Count);
            foreach (var tri in mesh.Triangles)
            {
                triangles.Add((uint)tri.P0);
                triangles.Add((uint)tri.P1);
                triangles.Add((uint)tri.P2);

                if (this.NumberOfRegions > 0)
                {
                    this.TrianglePartition[i++] = tri.Region;
                }
            }
            this.Triangles = triangles.ToArray();

            this.Bounds = new BoundingBox(
                (float)mesh.Bounds.Xmin,
                (float)mesh.Bounds.Xmax,
                (float)mesh.Bounds.Ymin,
                (float)mesh.Bounds.Ymax);
        }

        /// <summary>
        /// Copy voronoi data.
        /// </summary>
        public void SetVoronoi(IVoronoi voro)
        {
            Imped_Triangle = true;
            SetVoronoi(voro, 0);
        }

        /// <summary>
        /// Copy voronoi data.
        /// </summary>
        public void SetVoronoi(IVoronoi voro, int infCount)
        {
            Imped_Triangle = true;
            int i, n = voro.Points.Length;

            // Copy points
            this.VoronoiPoints = new float[2 * n + infCount];
            foreach (var v in voro.Points)
            {
                if (v == null)
                {
                    continue;
                }

                i = v.ID;
                this.VoronoiPoints[2 * i] = (float)v.X;
                this.VoronoiPoints[2 * i + 1] = (float)v.Y;
            }

            // Copy edges
            Point first, last;
            var edges = new List<uint>(voro.Regions.Count * 4);
            foreach (var region in voro.Regions)
            {
                first = null;
                last = null;

                foreach (var pt in region.Vertices)
                {
                    if (first == null)
                    {
                        first = pt;
                        last = pt;
                    }
                    else
                    {
                        edges.Add((uint)last.ID);
                        edges.Add((uint)pt.ID);

                        last = pt;
                    }
                }

                if (region.Bounded && first != null)
                {
                    edges.Add((uint)last.ID);
                    edges.Add((uint)first.ID);
                }
            }
            this.VoronoiEdges = edges.ToArray();
        }
        #endregion
        #endregion

        #region "Set WGeo2D Datas"
        public List<WEntity2D> Entities;
        public WGeos2D_PsList PsList;
        /////
        public void SetWGeo2DDatas(WGeometry2D WGC)
        {
            SetWGeo2DDatas(WGC, true);
        }
        public void SetWGeo2DDatas(WGeometry2D WGC, bool Update_Bound)
        {
            Entities = WGC.Entities;
            PsList = WGC.PsList;
            if (Update_Bound == false) return;
            this.Bounds = new BoundingBox(
                (float)WGC.Bounds.Xmin,
                (float)WGC.Bounds.Xmax,
                (float)WGC.Bounds.Ymin,
                (float)WGC.Bounds.Ymax);
        }
        #endregion

        #region Set Rim
        public List<WRim2D> Rims;
        /////
        public void SetWRim2DDatas(ref List<WRim2D> Rims)
        {
            SetWRim2DDatas(ref Rims, true);
        }
        public void SetWRim2DDatas(ref List<WRim2D> Rims, bool Update_Bound)
        {
            this.Rims = Rims;
            if (Rims == null || Rims.Count == 0) return;
            if (Update_Bound == false) return;
            for (int i = 0; i < Rims.Count; i++)
            {
                if (i == 0)
                {
                    this.Bounds = new BoundingBox(
                              (float)Rims[i].Xmin,
                              (float)Rims[i].Xmax,
                              (float)Rims[i].Ymin,
                              (float)Rims[i].Ymax);
                    continue;
                }

                this.Bounds.Update((float)Rims[i].Xmin, (float)Rims[i].Ymin);
                this.Bounds.Update((float)Rims[i].Xmax, (float)Rims[i].Ymax);
            }
        }
        #endregion

        #region Set WMesh2D Datas
        public List<WMesh2D_Mesh> Meshs;
        /////
        public void SetWMesh2DDatas(ref List<WMesh2D_Mesh> Meshs)
        {
            SetWMesh2DDatas(ref Meshs, true);
        }
        public void SetWMesh2DDatas(ref List<WMesh2D_Mesh> Meshs, bool Update_Bound)
        {
            this.Meshs = Meshs;
            if (Meshs == null || Meshs.Count == 0) return;
            if (Update_Bound == false) return;
            for (int i = 0; i < Meshs.Count; i++)
            {
                if (i == 0)
                {
                    this.Bounds = new BoundingBox(
                              (float)Meshs[i].Xmin,
                              (float)Meshs[i].Xmax,
                              (float)Meshs[i].Ymin,
                              (float)Meshs[i].Ymax);
                    continue;
                }

                this.Bounds.Update((float)Meshs[i].Xmin, (float)Meshs[i].Ymin);
                this.Bounds.Update((float)Meshs[i].Xmax, (float)Meshs[i].Ymax);
            }

        }
        #endregion
    }
}
