using ColorRunServer.Entities;
using ColorRunServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColorRunServer.GameEngine
{
    class User
    {
        public User(Client networkPart)
        {
            this.NetworkHandler = networkPart;

        }

        public Thread Thread { get; set; }
        public List<int> Cells;
        public int Score;
        public Client NetworkHandler;

        public void HandleUser()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Cells.Add(new PlayerCell());
        }
    }
}
