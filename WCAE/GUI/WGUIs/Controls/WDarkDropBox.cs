using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace WCAE.WGUIs.Controls
{
    /// <summary>
    /// Dark listbox control.
    /// </summary>
    public class WDarkDropBox : ComboBox
    {
        Font _boldFont;

        Color Color_Back = Color.FromArgb(96, 96, 96);
        Color Color_Border = Color.White;
        Color Color_Text = Color.White;
        Color Color_TextAc = WColorS_Dark.ColorGray68;

        /// <summary>
        /// Initializes a new instance of the <see cref="WDarkListBox" /> control.
        /// </summary>
        public WDarkDropBox()
        {
            _boldFont = new Font(base.Font.FontFamily, base.Font.Size, FontStyle.Bold);

            this.DrawMode = DrawMode.OwnerDrawVariable;

            this.FontChanged += new EventHandler(ListBoxFontChanged);
            this.BackColor = Color_Back;

            this.DropDownStyle = ComboBoxStyle.DropDownList;
            
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        }

        void ListBoxFontChanged(object sender, EventArgs e)
        {
            _boldFont = new Font(base.Font.FontFamily, base.Font.Size, FontStyle.Bold);
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            e.ItemHeight = 22;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (this.Items.Count == 0)
            {
                return;
            }

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int index = e.Index;

            string content = this.Items[0].ToString();

            if (index < this.Items.Count && index >= 0)
            {
                content = this.Items[index].ToString();
            }

            Color color = (e.Index % 2) == 0 ? Color.FromArgb(85, 85, 85) : Color.FromArgb(90, 90, 90);

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                color = Color.FromArgb(100, 105, 110);
            }

            using (SolidBrush background = new SolidBrush(color))
            {
                e.Graphics.FillRectangle(background, e.Bounds);
            }

            using (SolidBrush pen = new SolidBrush(Color.White))
            {
                e.Graphics.DrawString(content, this.Font, pen,
                    new PointF(10, e.Bounds.Y + 3), StringFormat.GenericDefault);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            //base.OnMouseWheel(e);
        }
    }
}
