using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCAE.WRenderer
{
    /// <summary>
    /// 渲染的对象模式
    /// </summary>
    public enum RenderMode
    {
        Geo2D,
        Geo2DRim,
        Mesh2D,
        Result,
        Triangle
    }
    /// <summary>
    /// 对象捕捉的模式
    /// </summary>
    public enum SnapMode
    {
        None,
        Geo2DEntity,
        Geo2DPoint,
        Geo2DRim,
        Mesh2DNode,
        Mesh2DElement

    }
    /// <summary>
    /// 渲染主题
    /// </summary>
    public enum Theme
    {
        Default,
        Light
    }
}
