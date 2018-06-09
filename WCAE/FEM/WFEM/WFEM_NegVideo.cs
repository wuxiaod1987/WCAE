using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using WCAE.WMesh2D;
using WCAE.WMesh2D.Entities;
using WCAE.WFEM;

namespace WCAE.WFEM
{
    public static class WFEM_NegVideo
    {
        public static WFEM_NegOut Create(ref WFEM_NegPara NP, WMesh2D_Mesh Mesh, string ResultFile, String ProgramPath, String[] Titles)
        {
            double[] RESM = new double[2];
            RESM = FindLimit(ResultFile);   /////如果不限制最大/最小值则需要找到最大/最小值 
            StreamReader sr = new StreamReader(ResultFile);
            sr.ReadLine();
            int Quan_Frames = Convert.ToInt16(sr.ReadLine());
            sr.ReadLine();
            int Quan_Nodes = Convert.ToInt16(sr.ReadLine());
            sr.ReadLine();
            /////
            double[] RES = new double[Quan_Nodes + 1];
            String Path = NP.PicPath;
            String Name = NP.PicName;
            NP.PicPath = Path + "VT" + DateTime.Now.TimeOfDay.ToString().Replace(":","");
            Directory.CreateDirectory(NP.PicPath);
            NP.PicPath += "\\";
            WFEM_NegOut ParaOut = null;
            for (int i = 0; i < Quan_Frames; i++)
            {
                if (NP.VidTitleOut == true && Titles.Length == Quan_Frames)
                    NP.PicTitle = Titles[i];
                //Debugger.Break();
                for (int j = 1; j <= Quan_Nodes; j++)
                    RES[j] = Convert.ToDouble(sr.ReadLine());
                sr.ReadLine();
                NP.PicName = (i + 1).ToString();
                ParaOut = WFEM_NegContour.Draw_to_PNG(ref NP, Mesh, ref RES, ref RESM);
            }
            //=============================//
            Process Proc = new Process();
            String Arg = NP.PicPath + ",";
            Arg += (Path + Name + ".avi" + ",");
            Arg += (NP.PicWidth.ToString() + ",");
            Arg += (NP.PicHeight.ToString() + ",");
            Arg += (NP.VidFrameRate.ToString() + ",");
            Arg += (Quan_Frames.ToString() + ",");
            /////
            Proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Proc.StartInfo.FileName = ProgramPath + "\\VideoCreator\\VideoCreator.exe";
            //Console.WriteLine(Proc.StartInfo.FileName); 
            Proc.StartInfo.Arguments = Arg;
            Proc.Start();
            ///
            NP.PicPath = Path;
            NP.PicName = Name;
            return ParaOut; /////返回最后一帧的结果
        }

        /// <summary>
        /// 找到结果的最大值，最小值，用于每帧云图
        /// </summary>
        private static double[] FindLimit(string ResultFile)
        {
            StreamReader  sr = new StreamReader(ResultFile);
            sr.ReadLine();
            int Quan_Frames = Convert.ToInt16(sr.ReadLine());
            sr.ReadLine();
            int Quan_Nodes = Convert.ToInt16(sr.ReadLine());
            sr.ReadLine();
            //////
            //////得到所有的结果数值
            double Max= double.MinValue;
            double Min=  double.MaxValue;
            double res = 0;
            for (int i = 0; i < Quan_Frames; i++)
            {
                {
                    for (int j = 1; j <= Quan_Nodes; j++)
                    {
                        res = Convert.ToDouble(sr.ReadLine());
                        if (res > Max) Max = res;
                        if (res < Min) Min = res;
                    }
                    sr.ReadLine();
                }
            }
            sr.Close();
            return new double[] { Min, Max };
        }
    }
}
