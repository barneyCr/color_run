using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorRunGame.GameEntities
{
    abstract class Cell// : IMassObject
    {
        public Cell()
        {
            this._mass = 1;
            x = y = 0;
        }

        public Cell(int id, int x, int y) : this()
        {
            this.CID = id;
            this.x = x;
            this.y = y;
        }

        public int CID { get; set; }

        public virtual double Speed
        {
            get;
        }

        protected int x, y;
        public virtual int X { get { return x; } protected set { this.x = value; } }
        public virtual int Y { get { return y; } protected set { this.y = value; } }
        public virtual void SetXY(int x, int y) { this.X = x; this.Y = y; }

        protected int _mass;
        public virtual int Mass
        {
            get { return _mass; }
            protected set { _mass = value; }
        }
        public virtual void SetMass(int mass) { Mass = mass; }

        /// <summary>
        /// Calculates the radius based on mass
        /// </summary>
        public double Radius
        {
            get
            {
                return 4 + Math.Sqrt(this._mass) * 6;
            }
        }

        public double GetDistanceBtwnEdges(Cell other)
        {
            return this.GetDistanceBtwnCenters(other) - this.Radius - other.Radius;
        }

        public double GetDistanceBtwnCenters(Cell other)
        {
            return Math.Sqrt(Math.Pow(other.x - this.x, 2) + Math.Pow(other.y - this.y, 2));
        }
    }
}