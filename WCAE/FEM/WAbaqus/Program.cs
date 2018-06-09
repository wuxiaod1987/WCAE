using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace WCAE.WFEM.WAbaqus
{
    class Program
    {
        static void Main(string[] args)
        {
            string Path;
            string Work_Name;
            
            if (args.Length == 0)
            {
                Console.WriteLine("请输入路径：");
                Path = Console.ReadLine() + "\\";
                Console.WriteLine("请输入工程名：");
                Work_Name = Console.ReadLine();
            }
            else
            {
                string[] t_1 = args[0].Split(',');
                Path = t_1[0];
                Work_Name = t_1[1];
            }

            FEM_Props FEM = new FEM_Props(0);
            Comput_FEM(Path, Work_Name, ref  FEM);
        }

        #region "计算"
        static void Comput_FEM(string Path, string Work_Name, ref FEM_Props FEM)
        {
            System.Diagnostics.Process Proc = new System.Diagnostics.Process();
            //////将几个网格文件组合在一起
            Mesh_Merge_Mod2.MeshFile_Combine(Path, "Free", "Pole", "S1", 0.005);
            Console.WriteLine("已经进行完第一阶段网格合并，正进行第二阶段网格合并");
            Mesh_Merge_Mod2.MeshFile_Combine(Path, "S1", "Wires_Shell", Work_Name, 0.005);
            File.Delete(Path + "S1.mesh");

            //////书写INP文件
            Console.WriteLine("已经完成网格合并，正调用Abaqus计算");
            Abaqus_Inp.Write_Serial(Path, Work_Name, ref FEM);

            //////调用Abaqus Standard计算
            Write_AbaqusBat(Path, Work_Name);
            Proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Proc.StartInfo.FileName = Path + Work_Name + ".bat";
            Proc.Start();
            ///等待计算完成
            while (true)
            {
                if (File.Exists(Path + Work_Name + ".lck") == true)
                    break;
            }
            while (true)
            {
                if (File.Exists(Path + Work_Name + ".lck") == false)
                    break;
            }

            ///调用Abaqus Python提取结果
            Console.WriteLine("计算已经完成，正读取ODB文件");
            Write_Py(Path, Work_Name);
            Write_AbaqusODBBat(Path, Work_Name);

            Proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Proc.StartInfo.FileName = Path + Work_Name + "_ODB.bat";
            Proc.Start();
            ///等待结果提取完成
            while (true)
            {
                try
                {
                    using (File.Open(Path + Work_Name + "_ODB.wxd", FileMode.Open, FileAccess.Read, FileShare.None))
                        break;
                }
                catch (System.Exception Ex) { }
            }
            ///绘制PNG图片
            Console.WriteLine("读取ODB文件完成，准备输出");
            double[] Res_Data = Draw_PNGs.Draw(Path, Work_Name, ref FEM);
            ///结果输出
            StreamWriter sw = new StreamWriter(Path + Work_Name + ".res");
            for (int i = 0; i < 4; i++)
                sw.WriteLine(Res_Data[i]);
            sw.Close();

            /////文件删除，精简
            Delete_Files(Path, Work_Name);

            ///图片显示
            //double[] Res_Data = new double[6];
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form_Res(Path, Work_Name, ref Res_Data));
        }

        #region "调用及文件处理"
        static void Delete_Files(string Path, string Work_Name)
        {
            DirectoryInfo Folder = new DirectoryInfo(Path);
            FileInfo[] Fs = Folder.GetFiles(Work_Name + "*");
            string t;
            for (int i = 0; i < Fs.Length; i++)
            {
                t = Fs[i].ToString();
                t = t.Substring(t.Length - 3, 3);
                if (t != "png" && t != "res" && t != "inp" && t != "odb")
                    try
                    {
                        Fs[i].Delete();
                    }
                    catch (Exception Ex) { }
            }
            Fs = Folder.GetFiles("*.poly");
            for (int i = 0; i < Fs.Length; i++)
                try
                {
                    Fs[i].Delete();
                }
                catch (Exception Ex) { }
        }

        static void Write_AbaqusBat(string Path, string Work_Name)
        {
            StreamWriter sw = new StreamWriter(Path + Work_Name + ".bat");
            sw.WriteLine("cd " + Path);
            sw.WriteLine("@echo off");
            string Abq_Path = "\"C:\\SIMULIA\\CAE\\2016\\win_b64\\code\\bin\\ABQLauncher.exe\"";
            sw.WriteLine(Abq_Path + " job=" + Work_Name);
            sw.Close();
            sw.Dispose();
        }

        static void Write_Py(string Path, string Work_Name)
        {
            Path = Path.Replace("\\", "\\\\");
            StreamWriter sw = new StreamWriter(Path + Work_Name + ".py");
            sw.WriteLine("# -*- coding: mbcs -*-");
            sw.WriteLine("import sys");
            sw.WriteLine("import io");
            sw.WriteLine("from odbAccess import *");
            sw.WriteLine("from abaqusConstants import *");
            sw.WriteLine("#==============Material read and write=======================");
            sw.WriteLine("WorkPath=" + "\"" + Path + "\"");
            sw.WriteLine("WorkName=" + "\"" + Work_Name + "\"");
            sw.WriteLine("OutName=" + "\"" + Work_Name + "_ODB.wxd\"");
            sw.WriteLine("o = openOdb(path=WorkPath+WorkName+'.odb')");
            sw.WriteLine("sts=o.steps");
            sw.WriteLine("f1=sts['Step-1'].frames[1]");
            sw.WriteLine("fops=f1.fieldOutputs");
            sw.WriteLine("fop=fops['NT11']");
            sw.WriteLine("sw=open(WorkPath+OutName,'w')");
            sw.WriteLine("for v in fop.values:");
            sw.WriteLine("	t=str(round(v.data,2))");
            sw.WriteLine("	sw.writelines(t)");
            sw.WriteLine("	sw.write('\\" + "n')");
            sw.WriteLine("sw.close()");
            sw.Close();
            sw.Dispose();
        }

        static void Write_AbaqusODBBat(string Path, string Work_Name)
        {
            StreamWriter sw = new StreamWriter(Path + Work_Name + "_ODB.bat");
            sw.WriteLine("cd " + Path);
            sw.WriteLine("@echo off");
            string Abq_Path = "abaqus Python ";
            sw.WriteLine(Abq_Path + Work_Name + ".py");
            sw.Close();
            sw.Dispose();
        }
        #endregion
        #endregion
    }
}
