using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorRunServer.Entities
{
    class FoodCell : Cell
    {
        public FoodCell()
        {
            this.Mass = 1;
            // todo set this according to settings file
        }

        /// <summary>
        /// Returns 0
        /// Food cells are motionless
        /// </summary>
        public override int Speed
        {
            get { return 0; }
        }
    }
}
