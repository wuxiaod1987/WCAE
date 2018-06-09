using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TriangleNet;
using TriangleNet.IO;
using TriangleNet.Geometry;
using TriangleNet.Data;
using FEM_Create;

namespace Triangle_Trial
{
    public partial class Form1 : Form
    {
        static Mesh mesh;
        static InputGeometry input;

        public Form1()
        {
            InitializeComponent();

            //Mesh_Para MP = new Mesh_Para();
            //MP.Path = "G:\\";
            //MP.Name = "Free";
            //MP.MaxAngle_Tri = 120;
            //MP.MinAngle_Tri = 25;
            //MP.MaxAngle_Qua = 135;
            //MP.MaxArea_Tri = 150;
            //MP.Round = 2;

            ////Node[] Ns = new Node[0];          /////注意该数组是从1开始的，目的是为了Element使用方便
            ////Element[] Es = new Element[0];

            //Generate_Mesh.Do_Mesh(ref MP);     /////, ref Ns, ref Es);
            ////Triangulate_Refine("G:\\Free.poly", MP);

            this.Text = "OK";
        }

        private void Shell_Try()
        {
            string Para = "";
            Para += "G:\\,";
            Para += "Free,";
            Para += "120,";
            Para += "25,";
            Para += "135,";
            Para += "150,";
            Para += "2,";
            Para += "0,";
            Para += "0";

            string Program_Path = "G:\\Glass_Lines\\ACAD_WXD\\FEM_Create\\FEM_Create.exe";

            System.Diagnostics.Process.Start(Program_Path);
 
        }

        #region "Triangle"
        private static bool Triangulate_Refine(string File_Name, Mesh_Para MP)
        {
            bool Out = false;
            mesh = new Mesh();
            input = FileReader.ReadPolyFile(File_Name);
            MP.QN_Ini = input.Count;

            if (MP.MinAngle_Tri > 0 && MP.MinAngle_Tri < 180)
                mesh.Behavior.MinAngle = MP.MinAngle_Tri;
            try
            {
                mesh.Triangulate(input);
                Out = true;
            }
            catch (System.Exception ex) { }

            if (Out == false)
                return Out;
            else
                Out = Refine(MP.MaxArea_Tri, MP.MaxAngle_Tri, MP.MinAngle_Tri);

            return Out;
        }

        private static bool Refine(double MaxArea, double MaxAngle, double MinAngle)
        {
            bool Out = false;
            if (mesh == null) return false;

            if (MaxArea > 0)
                mesh.Behavior.MaxArea = MaxArea;

            if (MinAngle > 0 && MinAngle < 180)
                mesh.Behavior.MinAngle = MinAngle;

            if (MinAngle < 180 && MinAngle > 0)
                mesh.Behavior.MaxAngle = MaxAngle;

            try
            {
                mesh.Refine();
                Out = true;
            }
            catch (System.Exception ex) { }
            return Out;
        }
        #endregion
    }
}
