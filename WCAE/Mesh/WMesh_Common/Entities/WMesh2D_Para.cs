using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WCAE.WGeos2D.Entities;
using WCAE.WMesh2D.IO;

namespace WCAE.WMesh2D.Entities
{
    /// 2D网格划分参数
    /// <summary>
    /// 2D网格划分参数
    /// </summary>
    public class WMesh2D_Para
    {
        /// <summary>
        /// 用于Triangle的最小三角形内角，默认为25
        /// </summary>
        public double MinAngle_Tri;      /////用于Triangle的最小三角形内角
        /// <summary>
        /// 用于Triangle的最大三角形内角，默认为120
        /// </summary>
        public double MaxAngle_Tri;      /////用于Triangle的最大三角形内角
        /// <summary>
        /// 用于单元合并的最大允许的四边形内角，默认为135
        /// </summary>
        public double MaxAngle_Qua;      /////用于单元合并的最大允许的四边形内角
        /// <summary>
        /// 用于Triangle的最大三角形面积，只读属性，根据Mesh_Length及TriArea_Times计算
        /// </summary>
        public double MaxArea_Tri
        {
            get { return Mesh_Length * Mesh_Length * 0.5 * TriArea_Times; }
        }     /////用于Triangle的最大三角形面积
        /// <summary>
        /// 用于计算Triangle的最大三角形面积相对于大范围面积的比例，默认为1.8
        /// </summary>
        public double TriArea_Times;     /////用于计算Triangle的最大三角形面积相对于大范围面积的比例
        /// <summary>
        /// 网格划分的长度
        /// </summary>
        public double Mesh_Length;       /////网格划分的长度

        /// <summary>
        /// 网格默认输出路径
        /// </summary>
        public string Path;              /////文件夹路径
        /// <summary>
        /// 网格名称
        /// </summary>
        public string Name;              /////文件名
        /// <summary>
        /// 初输入的Node数量
        /// </summary>
        public int QN_Ini;               /////最初输入的Node数量
        /// <summary>
        /// 已经生成的最新的Node编号
        /// </summary>
        public int Num_NodeLatest;       /////已经生成的最新的Node编号
        /// <summary>
        /// 已经生成的最新的Element编号
        /// </summary>
        public int Num_EleLatest;        /////已经生成的最新的Element编号

        /// <summary>
        /// 从输入流中输入
        /// </summary>
        /// <param name="sr"></param>
        public bool Input_From_Sr(StreamReader sr)
        {
            if (sr.ReadLine() != "The following datas are mesh parameters.") return false;
            string[] t;
            t = sr.ReadLine().Split('\t');
            MinAngle_Tri = Convert.ToDouble(t[1]);
            /////
            t = sr.ReadLine().Split('\t');
            MaxAngle_Tri = Convert.ToDouble(t[1]);
            /////
            t = sr.ReadLine().Split('\t');
            MaxAngle_Qua = Convert.ToDouble(t[1]);
            /////
            t = sr.ReadLine().Split('\t');
            TriArea_Times = Convert.ToDouble(t[1]);
            return true;
        }

        /// <summary>
        /// 向输出流中输出
        /// </summary>
        /// <param name="sw"></param>
        public void Output_To_Sw(StreamWriter sw)
        {
            sw.WriteLine("The following datas are mesh parameters.");
            sw.WriteLine("MinAngle_Tri" + "\t" + MinAngle_Tri.ToString());
            sw.WriteLine("MaxAngle_Tri" + "\t" + MaxAngle_Tri.ToString());
            sw.WriteLine("MaxAngle_Qua" + "\t" + MaxAngle_Qua.ToString());
            sw.WriteLine("TriArea_Times" + "\t" + TriArea_Times.ToString());
            //sw.WriteLine("Mesh_Length" + "\t" + Mesh_Length.ToString());
            //sw.WriteLine("Glass_Times" + "\t" + Glass_Times.ToString());
            //sw.WriteLine("********");
        }

        public WMesh2D_Para(string Path)
        {
            this.Path = Path;
            Name = "Free";
            MaxAngle_Tri = 120;
            MinAngle_Tri = 25;
            MaxAngle_Qua = 135;
            TriArea_Times = 1.0;
            Num_NodeLatest = 0;
            Num_EleLatest = 0;
        }
    }
}
