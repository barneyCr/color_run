using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorRunGame.GameEntities.Helpers
{
    public class Vector
    {
        public Vector()
        {
        }

        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public double X;
        public double Y;

        public double DistanceTo(Vector otherdude)
        {
            return Math.Sqrt(((otherdude.X - X) * (otherdude.X - X)) + ((otherdude.Y - Y) * (otherdude.Y - Y)));
        }
    }
}
