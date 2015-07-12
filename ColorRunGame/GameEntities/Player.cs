using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ColorRunGame.GameEntities
{
    class Player
    {
        public Player(int id, string username, Color col, int cells = 4)
        {
            this.Cells = new List<PlayerCell>(cells);
            this.PID = id;
            this.Username = username;
        }

        public void AddCell(PlayerCell c)
        {
            lock (this.Cells)
            {
                this.Cells.Add(c);
                Program.GameManager.PlayerCells.Add(c.CID, c);
            }
        }

        public void RemoveCell(PlayerCell c)
        {
            lock (this.Cells)
            {
                this.Cells.Remove(c);
                Program.GameManager.PlayerCells.Remove(c.CID);
            }
        }

        public Point FindCentralPointOfCells()
        {
            int x = this.Cells.Sum(c => c.X) / this.Cells.Count;
            int y = this.Cells.Sum(c => c.Y) / this.Cells.Count;
            return new Point(x, y);
        }

        public List<PlayerCell> Cells { get; set; }
        public int PID { get; set; }
        public string Username { get; set; }
    }
}
