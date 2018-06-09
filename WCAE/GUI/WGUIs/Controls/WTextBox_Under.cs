using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WGUIs.Controls
{
    public partial class WTextBox_Under : Control
    {
        public WTextBox_Under()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox textBox;
        Color Color_Back = WCAE.WGUIs.Controls.WColorS_Dark.Color_Back;
        Color Color_Border = WCAE.WGUIs.Controls.WColorS_Dark.Color_Rim;
        Color Color_Text = WCAE.WGUIs.Controls.WColorS_Dark.Color_Text;
        Color Color_TextAc = WCAE.WGUIs.Controls.WColorS_Dark.Color_TextAc;

        private void InitializeComponent()
        {
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // UserControl1
            this.BackColor = Color_Back;
            this.Controls.Add(this.textBox);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(90, 25);
            this.Font = new Font("Segoe UI", 9f);
            this.ResumeLayout(false);
            this.PerformLayout();
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.ForeColor = Color_Text;
            this.textBox.Font = this.Font;
            this.textBox.BackColor = Color_Back;
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Location = new System.Drawing.Point(4, 2);
            this.textBox.Name = "textBox1";
            this.textBox.TextAlign = HorizontalAlignment.Right;
            this.textBox.TabIndex = 0;
            this.textBox.MouseClick += textBox_MouseClick;
            this.textBox.TextChanged += textBox_TextChanged;
        }

        #region Event
        public delegate void textchange(object sender, EventArgs e);
        public event textchange Text_Change;
        void textBox_TextChanged(object sender, EventArgs e)
        {
            if (Text_Change != null)
                Text_Change(sender, new EventArgs());
        }

        #endregion

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
                textBox.Height = this.Font.Height + 8;
                this.Height = textBox.Height + 3;
            }
        }

        public override String Text
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
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

        protected override void OnPaint(PaintEventArgs e)
        {
            this.textBox.Width = this.Width - 8;
            this.textBox.Height = this.Height - 3;

            Pen Pen_Border = new Pen(Color_Border, 1);
            e.Graphics.DrawLine(Pen_Border, new Point(1, 1 + this.textBox.Height + 1), new Point(this.Width - 1, 1 + this.textBox.Height + 1));
            base.OnPaint(e);
        }

        void textBox_MouseClick(object sender, MouseEventArgs e)
        {
            this.textBox.SelectAll();
        }
    }
}