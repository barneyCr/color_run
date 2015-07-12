using ColorRunServer.Entities;
using ColorRunServer.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColorRunServer.GameEngine
{
    class WorldManager
    {
        private Network.Server Server;
        private Thread workThread;
        public WorldManager(Network.Server server)
        {

            this.Server = server;
            this.Users = new Dictionary<int, User>();
            this.eatenFood = new List<int>(100);

            Tuple<int, int> mapSize = Program.Settings["mapSize"];
            WorldValues.MapWidth = mapSize.Item1;
            WorldValues.MapHeight = mapSize.Item2;
            WorldValues.StartingMass = Program.Settings["pcellStartMass"];
            WorldValues.FoodMass = Program.Settings["foodMass"];
        }

        public class WorldValues
        {
            public static int MapWidth { get; set; }
            public static int MapHeight { get; set; }
            public static int StartingMass { get; set; }
            public static int FoodMass { get; set; }
        }

        public Dictionary<int, User> Users { get; private set; }
        public Dictionary<int, PlayerCell> PlayerCells { get; private set; }
        public Dictionary<int, FoodCell> FoodCells { get; private set; }


        /// <summary>
        /// Creates a user's first cell
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public PlayerCell GenerateNewPlayerCell(User owner)
        {
            int ok = 0;
            int x, y, id;
            do
            {
                ok = 0;
                x = Helper.NextInt32(300, WorldValues.MapWidth - 300);
                y = Helper.NextInt32(300, WorldValues.MapHeight - 300);
                id = Helper.NextInt32(0, 150 * 1000);

                foreach (var pair in this.PlayerCells)
                {
                    if (pair.Key != id)
                    {
                        // id is free
                        ok++;
                    }
                    var otherCell = pair.Value;
                    if (otherCell.GetDistanceToPoint(new Point(x, y)) > 125)
                    {
                        ok++;
                    }
                }
            } while (ok != 2);
            return new PlayerCell(x, y, WorldValues.StartingMass, id, owner);
        }

        // TODO split


        private void tick()
        {
            lock (this.PlayerCells)
            {
                foreach (var playerPairData in this.PlayerCells)
                {
                    PlayerCell pCell = playerPairData.Value;
                    lock (this.FoodCells)
                    {
                        foreach (var foodData in this.FoodCells)
                        {
                            var food = foodData.Value;
                            // we consider the size of food constant (10)
                            if (pCell.GetDistanceBtwnCenters(food) < 10 / 2 && !food.Eaten)
                            {
                                pCell.Mass += food.Mass;
                                food.Eaten = true;
                            }
                        }
                    }

                }
            }
        }

        private List<int> eatenFood;
        private void DisposeOfEatenFoodCells()
        {
            StringBuilder str = new StringBuilder("R|F|", 600);
            lock (this.FoodCells)
            {
                str.Append(eatenFood.Count);
                foreach (var id in eatenFood)
                {
                    FoodCells.Remove(id);
                    str.Append('|').Append(id);
                }
            }
            string s = str.ToString();
            foreach (var item in this.Users)
            {
                item.Value.NetworkHandler.Send(s);
            }
        }
    }
}
