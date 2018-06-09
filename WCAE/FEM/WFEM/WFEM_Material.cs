using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using WCAE.WMesh2D;
using WCAE.WMesh2D.IO;
using WCAE.WGeos2D.Entities;

namespace WCAE.WFEM
{
    /// <summary>
    /// 表示材料的CAE计算类型
    /// </summary>
    public enum Mart_Kind
    {
        /// <summary>
        /// 热传导材料
        /// </summary>
        _0_Termal,      /////热传导材料
        /// <summary>
        /// 热电耦合单元
        /// </summary>
        _1_TermElec     /////热电耦合单元
    };


    public class Mart_Prop
    {
        /// <summary>
        /// 材料计算类型
        /// </summary>
        public Mart_Kind Kind;        /////材料计算类型
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 密度
        /// </summary>
        public double Density;        /////密度
        /// <summary>
        /// 导热系数
        /// </summary>
        public double Heat_Conduct;   /////导热系数
        /// <summary>
        /// 比热容
        /// </summary>
        public double Heat_Specif;    /////比热容
        /// <summary>
        /// 电阻率
        /// </summary>
        public double Elec_Resist;    /////电阻率
        /// <summary>
        /// 导电率
        /// </summary>
        public double Elec_Conduct;   /////导电率，等于电阻率的倒数

        public Mart_Prop(string Name_Ini, Mart_Kind Kind_Ini)
        {
            Kind = Kind_Ini;
            Name = Name_Ini;
            Density = 0;
            Heat_Conduct = 0;
            Heat_Specif = 0;
            Elec_Resist = 0;
            Elec_Conduct = 0;
        }

        /// 将单位从ISO按照相应比例进行转换
        /// <summary>
        /// 将单位从ISO按照相应比例进行转换
        /// </summary>
        /// <param name="Lconvert">长度的转换比例，如从m转到mm取1000</param>
        /// <returns></returns>
        public Mart_Prop Unit_LConvert(double Lconvert)
        {
            if (Lconvert == 1)
                return this;          /////如果不转换就不再继续了
            //double L_c = 1000;   /////长度单位的转换比例，如转为mm则为1000
            Density = Density / (Lconvert * Lconvert * Lconvert);
            Heat_Conduct = Heat_Conduct * Lconvert;
            Heat_Specif = Heat_Specif * Lconvert * Lconvert;
            Elec_Resist = Elec_Resist * Lconvert * Lconvert * Lconvert;
            Elec_Conduct = 1 / Elec_Resist;
            return this;
        }

        /// <summary>
        /// 材料属性单位转换
        /// </summary>
        /// <param name="Lconvert">从m转换为mm，Lconvert=1000</param>
        public void Unit_LConvert(int Lconvert)
        {
            if (Lconvert == 1)
                return;          /////如果不转换就不再继续了
            //double L_c = 1000;   /////长度单位的转换比例，如转为mm则为1000
            Density = Density / (Lconvert * Lconvert * Lconvert);
            Heat_Conduct = Heat_Conduct * Lconvert;
            Heat_Specif = Heat_Specif * Lconvert * Lconvert;
            Elec_Resist = Elec_Resist * Lconvert * Lconvert * Lconvert;
            Elec_Conduct = 1 / Elec_Resist;
        }
    }   /////材料属性
}
