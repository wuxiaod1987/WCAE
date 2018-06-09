using System;
using WCAE.WGeos2D;

namespace WCAE.WRenderer
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IMeshRenderer
    {
        bool ShowVoronoi { get; set; }
        bool ShowRegions { get; set; }

        void Rerender();
        void Zoom(float x, float y, int delta);
        void DragMove(float x, float y);

        void HandleResize();

        void HandleUpdate();

        void Initialize();
        void Initialize(ref RenderParameter RParas);

        void SetData(RenderData data);
    }
}
