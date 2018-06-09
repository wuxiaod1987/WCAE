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
    public partial class WComboArrow : Control
    {
        public WComboArrow()
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

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button Button_up, Button_Down;
        private WCAE.WGUIs.Controls.WDarkListBox listBox;
        private System.ComponentModel.IContainer components = null;

        Color Color_Back = WCAE.WGUIs.Controls.WColorS_Dark.Color_Back;
        Color Color_Border = WCAE.WGUIs.Controls.WColorS_Dark.Color_Rim;
        Color Color_Text = WCAE.WGUIs.Controls.WColorS_Dark.Color_Text;
        Color Color_TextAc = WCAE.WGUIs.Controls.WColorS_Dark.Color_TextAc;
        Color Color_BackDeep = WCAE.WGUIs.Controls.WColorS_Dark.Color_BackDeep;

        private int Value_Ini;
        public bool Value_Change;
        private void InitializeComponent()
        {
            this.textBox = new System.Windows.Forms.TextBox();
            this.listBox = new WCAE.WGUIs.Controls.WDarkListBox();
            this.Button_up = new System.Windows.Forms.Button();
            this.Button_Down = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.DimGray;
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox.ForeColor = System.Drawing.Color.White;
            this.textBox.Location = new System.Drawing.Point(3, 2);
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.Width = 96;
            this.textBox.MinimumSize = new Size(50, 14);
            this.textBox.TabIndex = 0;
            this.textBox.Text = "12";
            this.textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox.Click += new System.EventHandler(this.textBox_Click);
            // 
            // listBox1
            // 
            for (int i = 0; i < 6; i++)
                this.listBox.Items.Add(i);
            this.listBox.BackColor = Color_BackDeep;
            this.listBox.ForeColor = Color_Text;
            this.listBox.Font = this.Font;
            this.listBox.DrawMode = DrawMode.OwnerDrawVariable;
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox.FormattingEnabled = true;
            this.listBox.ItemHeight = 22;
            this.listBox.Location = new System.Drawing.Point(3, 2 + this.textBox.Height + 9);
            this.listBox.Margin = new System.Windows.Forms.Padding(1);
            this.listBox.MaximumSize = new System.Drawing.Size(this.Width-6, 5 * this.listBox.ItemHeight);
            this.listBox.Name = "listBox1";
            this.listBox.TabIndex = 1;
            this.listBox.Visible = false;
            this.listBox.MouseClick += listBox_MouseClick;
            this.listBox.SelectedIndexChanged += listBox_SelectedIndexChanged;
            // 
            // Button_Up
            // 
            this.Button_up.BackColor = Color.Black;
            //this.Button_up.FlatStyle = FlatStyle.Flat;
            //this.Button_up.FlatAppearance.BorderColor = Color.White ;
            //this.Button_up.Height =  this.Height / 2;
            //this.Button_up.Width = this.Width - 3 - this.textBox.Width - 2;
            this.Button_up.Name = "button1";
            this.Button_up.Size = new System.Drawing.Size(20, 60);
            this.Button_up.TabIndex = 0;
            this.Button_up.Text = "button1";
            this.Button_up.UseVisualStyleBackColor = true;
            this.Button_up.Location = new Point(0,0);
            // 
            // WCombo
            // 
            this.BackColor = Color_Back;
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.textBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "WCombo";
            this.Size = new System.Drawing.Size(96, 2 + this.textBox.Height + 2);
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Fold = true;
        }

        #region Property overrides
        public int SelectedIndex
        {
            set
            {
                listBox.SelectedIndex = value;
                this.textBox.Text = listBox.SelectedItem.ToString();
                Value_Ini = value;
            }
            get
            {
                return listBox.SelectedIndex;
            }
        }
        public object SelectedItem
        {
            get
            {
                return listBox.SelectedItem;
            }
        }
        public ListBox.ObjectCollection Items
        {
            get
            {
                return this.listBox.Items;
            }
            set
            {
                for (int i = 0; i < value.Count; i++)
                    this.listBox.Items.Add(value[i]);
            }
        }
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

        #region Event
        public delegate void indexchange(object sender, EventArgs e);
        public event indexchange SelectIndex_Change;
        void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectIndex_Change != null)
                SelectIndex_Change(sender, new EventArgs());
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            this.textBox.Size = new System.Drawing.Size(this.Width - 26, 14);
            this.listBox.Size = new System.Drawing.Size(this.Width - 6, this.listBox.Items.Count * this.listBox.ItemHeight);
            
            Pen Pen_Border = new Pen(Color_Border, 1);
            e.Graphics.DrawLine(Pen_Border, new Point(1, 2 + this.textBox.Height + 1), new Point(1 + this.textBox.Width + 2, 2 + this.textBox.Height + 1));
            if (Fold == false)
                e.Graphics.DrawRectangle(Pen_Border, new Rectangle(new Point(listBox.Location.X - 2, listBox.Location.Y - 2), new Size(listBox.Size.Width + 4, listBox.Size.Height + 4)));
            SolidBrush Br = new SolidBrush(Color.Black);
            Point[] Ps = new Point[3];
            Ps[0] = new Point(textBox.Location.X + textBox.Width + 8, this.Height - 8);
            Ps[1] = new Point(textBox.Location.X + textBox.Width + 16, this.Height - 8);
            Ps[2] = new Point(textBox.Location.X + textBox.Width + 12, this.Height - 1);
            e.Graphics.FillPolygon(Br, Ps);
            /////
            Ps[0] = new Point(textBox.Location.X + textBox.Width + 8, 10);
            Ps[1] = new Point(textBox.Location.X + textBox.Width + 16, 10);
            Ps[2] = new Point(textBox.Location.X + textBox.Width + 12, 1);
            e.Graphics.FillPolygon(Br, Ps);
            /////
            Pen_Border = null;
            Br = null;
            Ps = null;
            base.OnPaint(e);
        }

        bool Fold;   ///判断控件是否折叠的参数
        private void textBox_Click(object sender, EventArgs e)
        {
            if (Fold == true)
            {
                this.Height = 2 + this.textBox.Height + 2 + this.listBox.Height + 10;
                this.listBox.Visible = true;
                this.BringToFront();
                Fold = false;
            }
            else
            {
                this.Height = 2 + this.textBox.Height + 2;
                this.listBox.Visible = false;
                Fold = true;
            }
        }

        void listBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (Value_Ini == this.listBox.SelectedIndex)
                Value_Change = false;
            else
                Value_Change = true;
            Value_Ini = this.listBox.SelectedIndex;
            this.textBox.Text = this.listBox.SelectedItem.ToString();
            this.Height = 2 + this.textBox.Height + 2;
            this.listBox.Visible = false;
            Fold = true;
            textBox.Focus();
        }
    }
}
