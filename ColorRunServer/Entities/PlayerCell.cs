using ColorRunServer.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorRunServer.Entities
{
    class PlayerCell : Cell
    {
        public PlayerCell(int x, int y, int mass, int id, User owner)
        {
            this.x = x;
            this.y = y;
            this.Mass = mass;
            this.CID = id;
            this.Owner = owner;
        }

        public User Owner { get; set; }

        /// <summary>
        /// 37,5 * (mass ^ (-1/4.5))
        /// </summary>
        public override double Speed
        {
            get
            {
                // Old formula: 5 + (20 * (1 - (this.mass/(70+this.mass))));
                // Based on 50ms ticks. If updateMoveEngine interval changes, change 50 to new value
                // (should possibly have a config value for this?)
                return 30 * Math.Pow(this.Mass, -1.0 / 4.5) * 50 / 40;
            }
        }
    }
}
