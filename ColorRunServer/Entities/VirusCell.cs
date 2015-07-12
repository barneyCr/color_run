using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorRunServer.Entities
{
    class VirusCell :Cell
    {
        public VirusCell(int x, int y, int id)
        {
            this.x = x;
            this.y = y;
            this.CID = id;
        }

        /// <summary>
        /// Returns 0
        /// Viruses are motionless
        /// </summary>
        public override double Speed
        {
            get { return 0; }
        }
    }
}
