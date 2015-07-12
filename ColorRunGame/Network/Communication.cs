using ColorRunGame.GameEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColorRunGame.Network
{
    class Communication
    {
        private Socket sock;
        private bool connected;
        private ASCIIEncoding enc = new ASCIIEncoding();

        private Thread networkThread;
        public Communication()
        {
            networkThread = new Thread(NetworkAction);
        }

        public bool Connect()
        {
            if (Program.Username == null)
                throw new UnauthorizedAccessException();
            try
            {
                this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.sock.Connect("localhost", 13001);

                this.Send("1", Program.Username);

                byte[] buff = new byte[2];
                int bRead = 0;
                if (!Receive(buff, ref bRead))
                {
                    throw new CommunicationException();
                }
                else
                {
                    if (buff.SequenceEqual(new byte[] { 3, 14 }))
                    {
                        throw new ServerFullException();
                    }
                    if (buff.SequenceEqual(new byte[] { 1, 1 })) // successful
                    {
                        connected = true;
                    }
                }
            }
            catch (SocketException)
            {
                connected = false;
            }

            return connected;
        }

        public void StartListener()
        {
            networkThread.IsBackground = true;
            networkThread.Start();
        }

        private void NetworkAction()
        {
            byte[] buff = new byte[2048];
            int bytes = 0;
            while (this.sock.Connected)
            {
                if (!Receive(buff, ref bytes)) { break; }

                Packet packet;
                string[] packetArray = enc.GetString(buff, 0, bytes).Split('\n');

                foreach (var pstr in packetArray)
                {
                    using (packet = new Packet(pstr))
                    {
                        HandlePacket(packet);
                    }
                }
            }
            //if (this.ConnectionLost != null && Kicked == false)
            //    this.ConnectionLost();
        }

        private void HandlePacket(Packet p)
        {
            Debug.WriteLine("Received packet: " + p.ToString(), "IN");
            p.Seek(1);
            if (p.Header == "")
            {
                return;
            }
            if (p.Header == "INIT")
            {
                int uid = p.ReadInt();
                string username = p.ReadString();
                int x = p.ReadInt(), y = p.ReadInt();
                int mass = p.ReadInt();
                
            }
            if (p.Header == "2") // set map area
            {
                int wid = p.ReadInt(), hei = p.ReadInt();
                if (Program.GameManager.Screen == null)
                {
                    Program.GameManager.Screen = new UI.Screen(new Size(wid, hei), Program.GForm.PanelSize);
                }
                else
                {
                    Program.GameManager.Screen.SetPlatformArea(wid, hei);
                }
                Program.GameManager.OnMapAreaModified();
            }
            if (p.Header == "A") // add something
            {
                int x, y;
                int id;
                string subhead = p.ReadString();
                switch (subhead)
                {
                    case "P":
                        string username = p.ReadString();
                        id = p.ReadInt();
                        int cells = p.ReadInt();
                        byte[] color = new[] { p.ReadByte(), p.ReadByte(), p.ReadByte() };

                        Color c = Color.FromArgb(color[0], color[1], color[2]);

                        Player player = new Player(id, username, c, cells);
                        if (!Program.GameManager.Players.ContainsKey(id))
                        {
                            Program.GameManager.Players.Add(id, player);
                        }
                        else
                        {
                            Program.GameManager.Players[id] = player;
                        }
                        Program.GameManager.OnPlayerJoinedGame(player);
                        break;

                    case "p":
                        int ownerID = p.ReadInt();
                        int cellID = p.ReadInt();
                        int mass = p.ReadInt();
                        x = p.ReadInt();
                        y = p.ReadInt();
                        lock (Program.GameManager.Players)
                        {
                            if (Program.GameManager.Players.ContainsKey(ownerID))
                            {
                                lock (Program.GameManager.PlayerCells)
                                {
                                    Player owner = Program.GameManager.Players[ownerID];
                                    if (!Program.GameManager.PlayerCells.ContainsKey(cellID))
                                    {
                                        owner.AddCell(new PlayerCell(mass, x, y, cellID, owner));
                                    }
                                    else
                                    {
                                        var cell = Program.GameManager.PlayerCells[cellID];
                                        cell.SetMass(mass);
                                        cell.SetXY(x, y);
                                        if (ownerID != cell.Owner.PID) // did this cell get eaten?
                                        {
                                            cell.Owner.RemoveCell(cell);
                                            owner.AddCell(cell);
                                        }
                                    }
                                }
                            }
                        }

                        break;

                    case "f":
                        x = p.ReadInt();
                        y = p.ReadInt();
                        id = p.ReadInt();

                        FoodCell food = new FoodCell(id, x, y);
                        food.SetMass(p.ReadInt());
                        lock (Program.GameManager.OtherCells)
                        {
                            if (Program.GameManager.OtherCells.ContainsKey(id))
                            {
                                Program.GameManager.OtherCells[id] = food;
                            }
                            else Program.GameManager.OtherCells.Add(id, food);
                        }

                        break;

                    case "v":
                        throw new NotImplementedException();
                    default:
                        break;
                }
            }
            if (p.Header == "R") // remove
            {
                string subhead = p.ReadString();
                switch (subhead)
                {
                    case "F": // remove eaten food
                        int count = p.ReadInt();
                        lock (Program.GameManager.OtherCells)
                        {
                            while (--count >= 0)
                            {
                                Program.GameManager.OtherCells.Remove(p.ReadInt());
                            }
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        

        

        public void Send(object[] par)
        {
            this.sock.Send(enc.GetBytes(string.Join("|", par)));
        }

        public void Send(string msg, params object[] obj)
        {
            this.sock.Send(enc.GetBytes(string.Format(msg, obj)));
        }

        private bool Receive(byte[] buffer, ref Int32 bytesRead)
        {
            try
            {
                bytesRead = this.sock.Receive(buffer);
                return bytesRead != 0;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}
