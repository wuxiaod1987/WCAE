using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TriangleNet.Data
{
    public struct Node
    {
        public int ID;       /////在实际模型中的单元编号
        public double x, y;
        public int[] EsB;
        public Node(double x, double y)
        {
            this.x = x;
            this.y = y;
            EsB = new int[0];
            ID = 0;
        }
        public Node(double x, double y, int ID)
        {
            this.x = x;
            this.y = y;
            EsB = new int[0];
            this.ID = ID;
        }
    }

    public struct Element
    {
        public byte Kind;
        public int ID;                   /////在实际模型中的单元编号
        public int N1, N2, N3, N4;
        public float Area;
        public bool Valid;               /////判断单元是否有效的参数，true为有效
        public Element(int ID, int N1, int N2, int N3, int N4, float Area)
        {
            this.ID = ID;
            this.N1 = N1;
            this.N2 = N2;
            this.N3 = N3;
            this.N4 = N4;
            this.Area = Area;
            Valid = true;
            Kind = 4;
        }
        public Element(int ID, int N1, int N2, int N3, float Area)
        {
            this.ID = ID;
            this.N1 = N1;
            this.N2 = N2;
            this.N3 = N3;
            this.Area = Area;
            N4 = 0;
            Valid = true;
            Kind = 3;
        }
        public Element(int ID, int N1, int N2, float Area)
        {
            this.ID = ID;
            this.N1 = N1;
            this.N2 = N2;
            this.Area = Area;
            N3 = 0;
            N4 = 0;
            Valid = true;
            Kind = 2;
        }
    }

}
