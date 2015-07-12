using ColorRunGame.GameEntities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorRunGame.GameEntities
{
    sealed class PlayerCell : Cell
    {
        public PlayerCell(int mass, int x, int y, int cellID, Player owner)
        {
            SetMass(mass);
            SetXY(x, y);
            this.obj = MovableObject.Create(Speed, x, y);
            this._owner = owner;
            this.CID = cellID;
        }

        private Player _owner;

        public Player Owner { get { return _owner; } }

        public override double Speed
        {
            get
            {
                // Old formula: 5 + (20 * (1 - (this.mass/(70+this.mass))));
                // Based on 50ms ticks. If updateMoveEngine interval changes, change 50 to new value
                // (should possibly have a config value for this?)
                return 30 * Math.Pow(this._mass, -1.0 / 4.5) * 50 / 40;
            }
        }

        private readonly MovableObject obj;


    }
}
