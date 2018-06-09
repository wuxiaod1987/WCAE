using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace WCAE.WFEM
{
    /// 云图渲染输出
    /// <summary>
    /// 云图渲染输出
    /// </summary>
    public class WFEM_NegOut
    {
        /// <summary>
        /// 每个节点结果数组
        /// </summary>
        public double[] RES;   /////每个节点结果数组
        /// <summary>
        /// 结果的，0：最小值，1：最大值
        /// </summary>
        public double[] RESM;  /////结果的最大值，最小值
        /// <summary>
        /// 最小值，最大值的对应节点编号，0：最小值，1：最大值
        /// </summary>
        public int[] NodeM;    /////最小值，最大值的对应节点编号
        /// <summary>
        /// 每种颜色单元的总面积
        /// </summary>
        public double[] Areas;
        /// <summary>
        /// 云图中每个分割点的数值
        /// </summary>
        public double[] Res_Each;  /////云图中每个分割点的数值
    }

    /// 云图的输出模式
    /// <summary>
    /// 云图的输出模式
    /// </summary>
    public enum NephogramMode
    {
        Picture,
        Video
    }

    /// 2D云图绘制参数（图片及视频参数）
    /// <summary>
    /// 2D云图绘制参数（图片及视频参数）
    /// </summary>
    public class WFEM_NegPara
    {
        #region 基本信息相关
        /// <summary>
        /// 图片文件名
        /// </summary>
        public String PicName;   /////图片文件名
        /// <summary>
        /// 图片保存地址，注意路径最后要带"/"
        /// </summary>
        public String PicPath;   /////图片保存地址
        /// <summary>
        /// 云图的Title
        /// </summary>
        public String PicTitle;  /////云图的Title
        /// <summary>
        /// 云图的单位
        /// </summary>
        public String PicUnit;   /////云图单位
        #endregion

        #region Title相关
        /// <summary>
        /// 是否显示Title，默认为True
        /// </summary>
        public bool ShowTitle;        /////是否显示Title
        /// <summary>
        /// Title的字体高度比例（相对于图片高度），默认0.0175倍，图片高4000时为70
        /// </summary>
        public float TitleHeightRate; /////Title的字体高度比例
        /// <summary>
        /// Title的字体绝对高度
        /// </summary>
        public int TitleHeight
        {
            get { return (int)(this.PicHeight * this.TitleHeightRate); }
        }
        /// <summary>
        /// Title的字体，默认为"Times Nem Roman"
        /// </summary>
        public String TitleFont;      /////Title的字体
        #endregion

        #region Lable相关
        /// <summary>
        /// Label长度占总宽度的比例（相对于图片宽度），默认为0.75
        /// </summary>
        public float LableWidthRate;      /////Label长度占总宽度的比例
        /// <summary>
        /// Label条的高度比例（相对于图片高度），默认为0.02，图片高为4000时为80
        /// </summary>
        public float LableHightRate;      /////Label条的高度比例
        /// <summary>
        /// Label条的绝对高度
        /// </summary>
        public int LableHight 
        { 
            get { return (int)(this.PicHeight * this.LableHightRate); } 
        }
        /// <summary>
        /// Label的字体高度比例（相对于图片高度），默认为0.0125，图片高4000时为50
        /// </summary>
        public float LableTextHeightRate; /////Label的字体高度比例
        /// <summary>
        /// Label的字体绝对高度
        /// </summary>
        public int LableTextHeight
        {
            get { return (int)(this.PicHeight * this.LableTextHeightRate); }
        }
        /// <summary>
        /// Label的字体，默认为"Times Nem Roman"
        /// </summary>
        public String LableFont;          /////Label的字体
        /// <summary>
        /// 是否显示Lable
        /// </summary>
        public bool ShowLable;            /////是否显示Lable
        #endregion

        #region 显示最大\小值相关
        /// <summary>
        /// 显示最大值的文字颜色，默认为红色
        /// </summary>
        public Color ColorMax;        /////显示最大值的文字颜色
        /// <summary>
        /// 显示最小值的文字颜色，默认为蓝色
        /// </summary>
        public Color ColorMin;        /////显示最小值的文字颜色
        /// <summary>
        /// 是否在最大/小值位置显示数值，默认为true
        /// </summary>
        public bool ShowMaxMin;       /////是否在最大/小值位置显示
        /// <summary>
        /// 是否在最大/小值位置显示节点编号，默认为true
        /// </summary>
        public bool ShowMaxMinNode;   /////是否在最大/小值位置显示节点编号
        #endregion

        #region 单元边界相关
        /// <summary>
        /// 是否显示单元边界，默认为false
        /// </summary>
        public bool ShowEleRim;        /////是否显示单元边界
        /// <summary>
        /// 单元边缘线颜色，默认为黑色
        /// </summary>
        public Color ColorEleRim;      /////用于绘制云图所用的单元边缘线颜色
        /// <summary>
        /// 单元边界线宽，默认为1
        /// </summary>
        public float PicEleRimWidth;   /////单元边线线宽
        #endregion

        #region 云图整体相关
        /// <summary>
        /// 云图的背景颜色,默认为透明
        /// </summary>
        public Color Color_BackGround;/////云图的背景颜色
        /// <summary>
        /// 2D云图绘制颜色，默认为Abaqus相同的从蓝到红的12种颜色
        /// </summary>
        public Color[] Color_Contours;/////用于绘制云图所用的渲染颜色数组
        /// <summary>
        /// 云图数值的圆整小数位数，默认为2位小数
        /// </summary>
        public byte PicRound;         /////文字显示时的保留小数位数量
        /// <summary>
        /// 云图图片宽度，按照高度的1.5自动计算得到
        /// </summary>
        public int PicWidth
        {
            get { return (int)(PicHeight * 1.5); }
        }
        /// <summary>
        /// 云图图片高度，Pic默认为4000，Video默认为2000
        /// </summary>
        public int PicHeight;         /////细致云图的图片高
        /// <summary>
        /// 云图边缘留白宽度比例（相对于图片高度），默认为0.0125，图片高4000时为50
        /// </summary>
        public float PicRimRate;      /////云图单侧边缘留白比例
        /// <summary>
        /// 云图边缘留白宽度
        /// </summary>
        public int PicRim
        {
            get { return (int)(this.PicHeight * this.PicRimRate); }
        }
        #endregion

        #region 视频相关
        /// <summary>
        /// 是否从外部输入Title
        /// </summary>
        public bool VidTitleOut;
        /// <summary>
        /// 固定视频颜色对应的最大数值
        /// </summary>
        public bool VidFixLimit;      /////固定视频的最大数值
        /// <summary>
        /// 视频每秒帧数
        /// </summary>
        public int VidFrameRate;      /////视频每秒帧数
        #endregion

        #region 云图最大值最小值控制
        /// <summary>
        /// 固定云图颜色对应的最大数值
        /// </summary>
        public bool ShowFixLimit;     /////固定云图颜色对应的最大数值
        /// <summary>
        /// 云图最大颜色对应的固定数值
        /// </summary>
        public double Neg_Max;  /////云图最大颜色对应的固定数值
        /// <summary>
        /// 云图最小颜色对应的固定数值
        /// </summary>
        public double Neg_Min;  /////云图最小颜色对应的固定数值
        #endregion 

        /// <summary>
        /// 初始化2D云图绘制参数
        /// </summary>
        /// <param name="PicPath">图片存放路径</param>
        /// <param name="PicName">图片名称</param>
        /// <param name="PicTitle">云图的Title</param>
        public WFEM_NegPara(String PicPath, String PicName, String PicTitle, String PicUnit, NephogramMode OutMode)
        {
            
            #region Title相关
            TitleHeightRate = 0.0175f;
            TitleFont = "Times Nem Roman";
            ShowTitle = true;
            #endregion
            
            #region Lable相关
            LableWidthRate = 0.75f;
            LableHightRate = 0.02f;
            LableTextHeightRate = 0.0125f;
            LableFont = "Times Nem Roman";
            ShowLable = true;
            #endregion
            
            #region 显示最大/最小值相关
            ColorMax = Color.Red;
            ColorMin = Color.Blue;
            if (OutMode == NephogramMode.Picture)
            {
                ShowMaxMin = true;
                ShowMaxMinNode = true;
            }
            else
            {
                ShowMaxMin = false;
                ShowMaxMinNode = false;
            }
            #endregion
            
            #region Rim相关
            ShowEleRim = false;
            ColorEleRim = Color.Black;
            PicEleRimWidth = 1f;
            #endregion

            #region 视频相关
            VidTitleOut = false;
            VidFixLimit = false;
            VidFrameRate = 1;
            #endregion

            #region 显示最值控制
            ShowFixLimit = false;
                
            Neg_Max = 15;
            Neg_Min = 0;
            #endregion

            #region 云图整体相关
            this.PicPath = PicPath;
            this.PicName = PicName;
            this.PicTitle = PicTitle;
            this.PicUnit = PicUnit;

            PicRound = 2;
            PicHeight = 2000;
            PicRimRate = 0.0125f;
            #endregion

            #region 颜色相关
            if (OutMode == NephogramMode.Picture) Color_BackGround = Color.Transparent;
            else Color_BackGround = Color.White;

            Color_Contours = new Color[14];
            Color_Contours[13] = Color.FromArgb(204, 204, 204);

            Color_Contours[12] = Color.FromArgb(255, 0, 0);
            Color_Contours[11] = Color.FromArgb(255, 93, 0);
            Color_Contours[10] = Color.FromArgb(255, 185, 0);
            Color_Contours[9] = Color.FromArgb(232, 255, 0);
            Color_Contours[8] = Color.FromArgb(139, 255, 0);
            Color_Contours[7] = Color.FromArgb(46, 255, 0);
            Color_Contours[6] = Color.FromArgb(0, 255, 46);
            Color_Contours[5] = Color.FromArgb(0, 255, 139);
            Color_Contours[4] = Color.FromArgb(0, 255, 232);
            Color_Contours[3] = Color.FromArgb(0, 185, 255);
            Color_Contours[2] = Color.FromArgb(0, 93, 255);
            Color_Contours[1] = Color.FromArgb(0, 0, 255);

            Color_Contours[0] = Color.FromArgb(51, 51, 51);
            #endregion
        }
    }
}
