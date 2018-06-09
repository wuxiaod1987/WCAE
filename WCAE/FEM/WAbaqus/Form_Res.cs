using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WCAE.WFEM.WAbaqus
{
    public partial class Form_Res : Form
    {
        private string Path;
        private string Work_Name;
        private double[] Res_Data;
        int Res_No;
        public Form_Res( string Path_in, string Work_Name_in,ref double[] Res_Data_in)
        {
            Path = Path_in;
            Work_Name = Work_Name_in;
            Res_Data = Res_Data_in;
            InitializeComponent();
        }

        private void Form_Res_Load(object sender, EventArgs e)
        {
            Res_No = 1;
            Show_Res(1);
        }

        private void Show_Res(int No)
        {
            string t = "";
            if (No == 1)
                t = "第1层玻璃";
            if (No == 2)
                t = "第2层玻璃";
            statusLabel1.Text = "显示状态：" + t + 
                              "，最低温度：" + Convert.ToString(Math.Round(Res_Data[(No - 1) * 2], 2)) + 
                              "，最高温度：" + Convert.ToString(Math.Round(Res_Data[(No - 1) * 2 + 1], 2));
            pictureBox1.Image = Image.FromFile(Path + Work_Name + "_Coarse_Layer_" + Convert.ToString(No) + ".png");
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (Res_No == 1)
                Res_No = 2;
            else
                Res_No = 1;
            Show_Res(Res_No);
        }
    }
}
