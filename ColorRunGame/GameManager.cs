using ColorRunGame.GameEntities;
using ColorRunGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorRunGame
{
    class GameManager
    {
        /// <summary>
        /// x
        /// </summary>
        public int MapWidth = 15000;
        /// <summary>
        /// y
        /// </summary>
        public int MapHeight = 10500;

        public event EventHandler MapAreaModified;
        public void OnMapAreaModified()
        {
            if (MapAreaModified != null)
            {
                MapAreaModified(null, EventArgs.Empty);
            }
        }

        public Player Player { get; set; }

        public Dictionary<int, Player> Players = new Dictionary<int, Player>(100);
        public Dictionary<int, PlayerCell> PlayerCells = new Dictionary<int, PlayerCell>(250);
        public Dictionary<int, Cell> OtherCells = new Dictionary<int, Cell>(500);

        public Screen Screen { get; set; }

        public event EventHandler<Player> PlayerJoinedGame;
        public void OnPlayerJoinedGame(Player player)
        {
            if (PlayerJoinedGame != null)
            {
                PlayerJoinedGame(null, player);
            }
        }
    }
}
