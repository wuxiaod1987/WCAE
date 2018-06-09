using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Text;

namespace WCAE.WGUIs.Controls
{
    public class WColorBox : Control
    {
        #region Designer

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion

        #endregion

        Brush fillBlue1 = new SolidBrush(Color.FromArgb(60, 100, 140));
        Color Color_Back = WCAE.WGUIs.Controls.WColorS_Dark.Color_Back;
        Color Color_Border = WCAE.WGUIs.Controls.WColorS_Dark.Color_Rim;
        Color Color_Value;

        public WColorBox()
        {
            this.BackColor = Color_Back;
            this.Color_Value = Color.Black;
            InitializeComponent();
        }

        public Color Value
        {
            set { Color_Value = value; }
            get { return Color_Value; }
        }

        Pen Pen_Border;
        SolidBrush Br;
        Point[] Ps = new Point[4];
        protected override void OnPaint(PaintEventArgs e)
        {
            Ps[0] = new Point(2, 2);
            Ps[1] = new Point(this.Width - 2, 2);
            Ps[2] = new Point(this.Width - 2, this.Height - 2);
            Ps[3] = new Point(2, this.Height - 2);


            Br = new SolidBrush(Color_Value);
            e.Graphics.FillPolygon(Br, Ps);
            Pen_Border = new Pen(Color_Border, 1);
            e.Graphics.DrawPolygon(Pen_Border, Ps);
            base.OnPaint(e);
        }
    }
}
