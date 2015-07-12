using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ColorRunServer.Entities
{
    /// <summary>
    /// Base class for all interactive objects in the game
    /// </summary>
    abstract class Cell
    {
        protected int x, y, mass;

        /// <summary>
        /// Mass
        /// </summary>
        public int Mass
        {
            get { return this.mass; }
            set { mass = value; }
        }
        /// <summary>
        /// Center of the cell
        /// </summary>
        public int X { get { return this.x; } }
        public int Y { get { return this.y; } }
        /// <summary>
        /// Game units/sec
        /// </summary>
        public abstract double Speed { get; }
        public int CID { get; protected set; }
        public double Radius { get { return 4 + Math.Sqrt(this.Mass) * 6; } }

        public Point Location
        {
            get { return new Point(x, y); }
            set { this.x = value.X; this.y = value.Y; }
        }

        public double GetDistanceBtwnEdges(Cell other)
        {
            return this.GetDistanceBtwnCenters(other) - this.Radius - other.Radius;
        }

        public double GetDistanceBtwnCenters(Cell other)
        {
            return GetDistanceToPoint(other.Location);
        }

        public double GetDistanceToPoint(Point pt)
        {
            return Math.Sqrt((pt.X - this.X) * (pt.X - this.X) + (pt.Y - this.Y)*(pt.Y - this.Y));
        }

        internal bool Eaten;
    }
}
