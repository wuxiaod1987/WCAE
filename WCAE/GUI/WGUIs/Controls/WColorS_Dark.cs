using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WCAE.WGUIs.Controls
{
    /// <summary>
    /// Dark user interface color scheme.
    /// </summary>
    public static class WColorS_Dark
    {
        
        public static Color ColorGray13 = Color.FromArgb(13, 13, 13);
        public static Color ColorGray46 = Color.FromArgb(46, 46, 46);
        public static Color ColorGray50 = Color.FromArgb(50, 50, 50);
        public static Color ColorGray64 = Color.FromArgb(64, 64, 64);
        public static Color ColorGray68 = Color.FromArgb(68, 68, 68);
        public static Color ColorGray78 = Color.FromArgb(78, 78, 78);
        public static Color ColorGray89 = Color.FromArgb(89, 89, 89);
        public static Color ColorGray98 = Color.FromArgb(98, 98, 98);
        public static Color ColorGray107 = Color.FromArgb(107, 107, 107);
        public static Color ColorGray110 = Color.FromArgb(110, 110, 110);
        public static Color ColorGray122 = Color.FromArgb(122, 122, 122);

        public static Brush BrushGray50 = new SolidBrush(ColorGray50);
        public static Brush BrushGray68 = new SolidBrush(ColorGray68);
        public static Brush BrushGray78 = new SolidBrush(ColorGray78);

        // Linear gradient horizontal
        public static Brush SliderBorderBrush = new SolidBrush(ColorGray46);
        public static Brush SliderFillBrush = new SolidBrush(ColorGray89);

        public static Color Color_Back = Color.DimGray;
        public static Color Color_Text = Color.White;
        public static Color Color_Rim = Color.White;
        public static Color Color_TextAc = ColorGray68;
        public static Color Color_BackDeep = ColorGray89;
    }
}
