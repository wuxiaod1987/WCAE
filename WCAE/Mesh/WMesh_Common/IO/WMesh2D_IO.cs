using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WCAE.Entities;
using WCAE.WMesh2D;
using WCAE.WMesh2D.Entities;
using WCAE.WGeos2D.Entities;

namespace WCAE.WMesh2D.IO
{
    /// <summary>
    /// 2D网格的文件IO操作
    /// </summary>
    public static class WMesh2D_IO
    {
        public static void Read_WMeshFile(string FileName, ref List<WNode2D> Nodes, ref List<WElement2D> Elements, ref WBoundingBox Bounds)
        {
            StreamReader sr = new StreamReader(FileName);
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            string t = sr.ReadLine();
            string[] t_1;
            int No = 0;
            ///获取节点信息
            t = t.Split('	')[1];
            t_1 = t.Split(',');
            No++;
            Nodes.Add(new WNode2D(Convert.ToDouble(t_1[0]),
                            Convert.ToDouble(t_1[1])));
            Bounds.Update(Nodes[1].X, Nodes[1].Y);
            t = sr.ReadLine();
            while (t != "")
            {
                t = t.Split('	')[1];
                t_1 = t.Split(',');
                No++;
                Nodes.Add(new WNode2D(Convert.ToDouble(t_1[0]),
                                Convert.ToDouble(t_1[1])));
                ///求取节点坐标范围
                Bounds.Update(Nodes[No].X, Nodes[No].Y);
                t = sr.ReadLine();
            }
            ///获取单元信息
            sr.ReadLine();
            sr.ReadLine();
            int K;
            while (sr.Peek() != -1)
            {
                t_1 = sr.ReadLine().Split('	');
                if (t_1[0] == "0")
                    continue;
                K = Convert.ToInt16(t_1[0]);
                t_1 = t_1[1].Split(',');
                Elements.Add(new WElement2D(K,
                       Convert.ToInt32(t_1[0]),
                       Convert.ToInt32(t_1[1]),
                       Convert.ToInt32(t_1[2]),
                       Convert.ToInt32(t_1[3])));
            }
            sr.Close();
            sr.Dispose();
        }

        public static void Read_WMeshFile(string FileName, ref WMesh2D_Mesh Mesh)
        {
            StreamReader sr = new StreamReader(FileName);
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            string t = sr.ReadLine();
            string[] t_1;
            int No = 0;
            int QNo = Mesh.Nodes.Count - 1;    /////已经读入的节点的数量
            ///获取节点信息
            t = t.Split('	')[1];
            t_1 = t.Split(',');
            No++;
            Mesh.Nodes.Add(new WNode2D(Convert.ToDouble(t_1[0]),
                            Convert.ToDouble(t_1[1])));
            t = sr.ReadLine();
            while (t != "")
            {
                t = t.Split('	')[1];
                t_1 = t.Split(',');
                No++;
                Mesh.Nodes.Add(new WNode2D(Convert.ToDouble(t_1[0]),
                                Convert.ToDouble(t_1[1])));
                t = sr.ReadLine();
            }
            ///获取单元信息
            sr.ReadLine();
            sr.ReadLine();
            int K;
            while (sr.Peek() != -1)
            {
                t_1 = sr.ReadLine().Split('	');
                if (t_1[0] == "0")
                    continue;
                K = Convert.ToInt16(t_1[0]);
                t_1 = t_1[1].Split(',');
                Mesh.Elements.Add(new WElement2D(K,
                       Convert.ToInt32(t_1[0]) + QNo,
                       Convert.ToInt32(t_1[1]) + QNo,
                       Convert.ToInt32(t_1[2]) + QNo,
                       Convert.ToInt32(t_1[3]) + QNo));
            }
            sr.Close();
            sr.Dispose();
        }

        public static void Write_WMeshFile(string Path, ref WMesh2D_Mesh Mesh)
        {
            StreamWriter Sw = new StreamWriter(Path + Mesh.Name + ".mesh");
            Sw.WriteLine("Parameters of the Mesh:");
            Sw.WriteLine(Mesh.Q_FreeNs);
            Sw.WriteLine("  Following Lines are Nodes");
            Sw.WriteLine("______________________________");
            for (int i = 1; i < Mesh.Nodes.Count; i++)
                Sw.WriteLine(Convert.ToString(i) + "	" + 
                             Convert.ToString(Mesh.Nodes[i].X) + "," + 
                             Convert.ToString(Mesh.Nodes[i].Y) + "," + 
                             Convert.ToString(Mesh.Nodes[i].Trace));
            Sw.WriteLine("");
            Sw.WriteLine("  Following Lines are Elements");
            Sw.WriteLine("______________________________");
            for (int i = 0; i < Mesh.Elements.Count; i++)
                Sw.WriteLine(Convert.ToString(Mesh.Elements[i].Kind) + "	" +
                             Convert.ToString(Mesh.Elements[i].N1) + "," +
                             Convert.ToString(Mesh.Elements[i].N2) + "," +
                             Convert.ToString(Mesh.Elements[i].N3) + "," +
                             Convert.ToString(Mesh.Elements[i].N4));
            Sw.Close();
        }
    }
}
