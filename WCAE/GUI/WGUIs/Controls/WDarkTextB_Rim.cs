using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WCAE.WGUIs.Controls
{
    /// <summary>
    /// Dark textbox control.
    /// </summary>
    public class WDarkTextB_Rim : Control
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

            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Location = new System.Drawing.Point(4, 2);
            this.textBox.Name = "textBox";
            this.textBox.TabIndex = 0;
            // 
            // DarkTextBox
            // 
            this.BackColor = Color.White;
            this.Controls.Add(this.textBox);
            this.Cursor = Cursors.IBeam;
            this.Size = new System.Drawing.Size(90, 22);
            this.ResumeLayout(false);
            this.PerformLayout();
            this.ForeColor = Color_Text;
        }

        #endregion

        #endregion

        TextBox textBox;
        Color Color_Back = Color.FromArgb(96, 96, 96);
        Color Color_Border = Color.White;
        Color Color_Text = Color.White;
        Color Color_TextAc = WColorS_Dark.ColorGray68;

        /// <summary>
        /// Initializes a new instance of the <see cref="WDarkTextB_Rim" /> control.
        /// </summary>
        public WDarkTextB_Rim()
        {
            InitializeComponent();

            this.MouseClick += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left) textBox.Focus();
            };

            textBox.Font = this.Font;
            textBox.Location = new Point(4, (this.Height - textBox.Height) / 2);
            textBox.Width = this.Width - 8;
            textBox.TextAlign = HorizontalAlignment.Right;
            textBox.ForeColor = this.ForeColor;
            this.BackColor = Color_Back;
            
            //textBox.MaxLength = 6;

            textBox.GotFocus += delegate(object sender, EventArgs e)
            {
                textBox.ForeColor = Color_TextAc;
            };

            textBox.LostFocus += delegate(object sender, EventArgs e)
            {
                textBox.ForeColor = Color_Text;
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Rectangle rect = this.ClientRectangle;

            #region 画成立体边框
            //Brush brushOuter = new LinearGradientBrush(rect, Color.FromArgb(82, 82, 82), 
            //Color.FromArgb(96, 96, 96), LinearGradientMode.Vertical);

            //Pen borderTop = new Pen(Color.FromArgb(76, 76, 76), 1f);
            //Pen borderBottom = new Pen(Color.FromArgb(128, 128, 128), 1f);
            //e.Graphics.FillRectangle(brushOuter, rect);

            //rect = new Rectangle(1, 1, this.Width - 3, this.Height - 3);
            //g.FillRectangle(new SolidBrush(this.BackColor), rect);

            //g.DrawLine(borderTop, 0, 0, this.Width - 1, 0);
            //g.DrawLine(borderTop, 0, 0, 0, this.Height - 1);
            //g.DrawLine(borderBottom, 1, this.Height - 1, this.Width - 1, this.Height - 1);
            //g.DrawLine(borderBottom, this.Width - 1, this.Height - 1, this.Width - 1, this.Height - 1);

            //brushOuter.Dispose();
            //borderTop.Dispose();
            //borderBottom.Dispose();
            #endregion

            Pen Pen_Birder = new Pen(Color_Border , 1f);
            rect = new Rectangle(1, 1, this.Width - 3, this.Height - 3);
            g.DrawRectangle(Pen_Birder, rect);

            base.OnPaint(e);
        }

        #region Property overrides

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                textBox.Font = value;
                base.Font = value;
            }
        }

        public override String Text
        {
            get
            {
                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
            }
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                textBox.ForeColor = value;
                base.ForeColor = value;
            }
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                textBox.BackColor = value;
                base.BackColor = value;
            }
        }

        #endregion

        #region Textbox properties

        public HorizontalAlignment TextAlign
        {
            get
            {
                return textBox.TextAlign;
            }
            set
            {
                textBox.TextAlign = value;
            }
        }

        #endregion
    }
}
