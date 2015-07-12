using ColorRunServer.GameEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColorRunServer.Network
{
    class Server
    {
        public GameModes GameMode { get; set; }
        public string Password { private get; set; }

        readonly TcpListener _listener;
        readonly IPAddress ip = IPAddress.Any;
        readonly int port, maxConnections;
        readonly bool logPackets, acceptServers;
        public readonly Thread listenThread;
        public static readonly ASCIIEncoding Encoding = new ASCIIEncoding();

        

        public Dictionary<int, Client> Connections { get; set; }
        public ICollection<string> Blacklist { get; set; }

        public bool Listen { get; set; }

        public Server(int port, int maxConnections, GameModes auth, string password)
        {
            this.maxConnections = maxConnections;
            this.port = port;
            this._listener = new TcpListener(ip, port);
            this.Connections = new Dictionary<int, Client>(maxConnections);
            this.listenThread = new Thread(StartListening);
            this.logPackets = Program.Settings["logPackets"];
            this.acceptServers = Program.Settings["acceptServerSockets"];

            this.Blacklist = new List<string>();

            Password = password;
            GameMode = auth;
        }

        private void StartListening()
        {
            try
            {
                _listener.Start();
                Listen = true;
                //Program.Write(LogMessageType.Network, "Started listening on port " + this.port);
            }
            catch { 
                //Program.Write("There may be another server listening on this port, we can't start our TCP listener", "Network", ConsoleColor.Magenta); 
            }

            while (Listen)
            {
                try
                {
                    Socket socket = _listener.AcceptSocket();
                    if (Connections.Count + 1 > maxConnections)
                    {
                        // todo
                        //socket.Send(ServerIsFullPacket);
                        socket.Close();
                        Thread.Sleep(500);
                        continue;
                    }
                    if (this.Blacklist.Contains(socket.RemoteEndPoint.ToString().Split(':')[0]))
                    {
                        //Program.Write(LogMessageType.Auth, "Rejected blacklisted IP: {0}", socket.RemoteEndPoint.ToString());
                        //socket.Send(BlacklistedPacket);
                        socket.Close();
                        continue;
                    }

                    var watch = Stopwatch.StartNew();
                    //todo
                    Program.Write(LogMessageType.Network, "Incoming connection");
                    ConnectionFlags flag = ConnectionFlags.OtherError;

                    Client client = OnClientConnected(socket, ref flag);
                    if (client != null && flag == ConnectionFlags.Accepted)
                    {
                        OnSuccessfulClientConnect(client);
                    }
                    else if ((flag == ConnectionFlags.WrongPassword
                        // && AuthMethod == ChatServer.AuthMethod.Full
                        ) || flag == ConnectionFlags.BadFirstPacket)// || flag == ConnectionFlags.BadInviteCode)
                    {
                        try
                        {
                            socket.Send(new byte[] { 3, 14 });
                            socket.Shutdown(SocketShutdown.Both);
                        }
                        finally
                        {
                            socket.Close();
                            socket = null;
                            Program.Write(LogMessageType.Auth, "A client failed to connect");
                        }
                    }
                    else if (flag == ConnectionFlags.SocketError)
                    {
                        Program.Write("Socket error on connection", "Auth", ConsoleColor.Red);
                    }
                    else if (flag == ConnectionFlags.OtherError)
                    {

                    }
                    watch.Stop();
                    Program.Write("Handled new connection in " + watch.Elapsed.TotalSeconds + " seconds", "Trace");
                }
                catch (SocketException e)
                {
                    Program.Write("Exception code: " + e.ErrorCode, "Socket Error", ConsoleColor.Red);
                    break;
                }
            }
        }

        private void OnSuccessfulClientConnect(Client client)
        {
            client.Socket.Send(new byte[] { 1, 1 });
            User tracker = new User(client);
            tracker.Thread = new Thread(tracker.HandleUser);
            tracker.Thread.IsBackground = true;
            tracker.Thread.Start();

        }
        
        static string CreatePacket(params object[] o) {
            return string.Join("|", o);
        }

        private Client OnClientConnected(Socket newGuy, ref ConnectionFlags flag)
        {
            #region hide
            //switch (Server.AuthMethod)
            //{
            //    case AuthMethod.UsernameOnly:
            //        newGuy.Send(NameRequiredPacket);
            //        break;
            //    case AuthMethod.Full:
            //        newGuy.Send(FullAuthRequiredPacket);
            //        break;
            //    case AuthMethod.InviteCode:
            //        newGuy.Send(InviteCodeRequiredPacket);
            //        break;
            //    default:
            //        throw new NotImplementedException();
            //} 
            #endregion

            byte[] buffer = new byte[32];
            int bytesRead = 0;

            if (!Receive(newGuy, buffer, ref bytesRead))
                return OnConnectionError(ref flag);

            using (Packet loginPacket = new Packet(Encoding.GetString(buffer)))
            {
                if (loginPacket.ReadString() == "1")
                {
                    string username = loginPacket.ReadString();
                    flag = ConnectionFlags.Accepted;
                    return new Client(username, newGuy);
                    #region hide
                //if (buffer[0] == 0x45)// && AuthMethod == ChatServer.AuthMethod.UsernameOnly) // UsernameOnly
                //{
                //    string username = Helper.XorText(enc.GetString(buffer, 1, bytesRead - 1), buffer[0]);
                //    while (this.Connections.ValuesWhere(c => c.Username == username).Any() || Program.ReservedNames.Contains(username.ToLower()))
                //        username += (char)Helper.Randomizer.Next((int)'a', (int)'z');

                //    flag = ConnectionFlags.OK;
                //    return new Client(username, newGuy);
                //}
                //else if (buffer[0] == 0x55 && AuthMethod == ChatServer.AuthMethod.Full)
                //{
                //    string[] data = GetLoginPacketParams(buffer, bytesRead);
                //    if (Server.Password == data[1])
                //    {
                //        flag = ConnectionFlags.OK;
                //        return new Client(data[0], newGuy);
                //    }
                //    else
                //    {
                //        flag = ConnectionFlags.BadPassword;
                //        return null;
                //    }
                //}
                //else if (buffer[0] == 0x65 && AuthMethod == ChatServer.AuthMethod.InviteCode)
                //{
                //    string[] data = GetLoginPacketParams(buffer, bytesRead);
                //    //data[1] = Helper.XorText(data[1], data[1].Length);
                //    lock (Program.InviteCodes)
                //    {
                //        if (Program.InviteCodes.Contains(data[1]))
                //        {
                //            Program.InviteCodes.Remove(data[1]); // remove the invite code from the list
                //            Program.Write(LogMessageType.Auth, "{0} used invite code {1}", data[0], data[1]);
                //            flag = ConnectionFlags.OK;
                //            return new Client(data[0], newGuy);
                //        }
                //        else
                //        {
                //            flag = ConnectionFlags.BadInviteCode;
                //            return null;
                //        }
                //    }
                //}
#endregion
                }
                else
                {
                    flag = ConnectionFlags.BadFirstPacket;
                    return null;
                }
            }
            /*
            if (buffer[0] < 128)
            {
                string username = Encoding.GetString(buffer, 1, bytesRead - 1);
                if (username.Length > 18)
                {
                    username = new string(username.ToCharArray(), 0, 18);
                }
                flag = ConnectionFlags.Accepted;
                return new Client(username, newGuy);
            }
            else
            {
                flag = ConnectionFlags.BadFirstPacket;
                return null;
            }
             */ 
        }

        private Client OnConnectionError(ref ConnectionFlags flag)
        {
            flag = ConnectionFlags.SocketError;
            return null;
        }

        private bool Receive(Socket socket, byte[] buffer, ref Int32 bytesRead)
        {
            try
            {
                bytesRead = socket.Receive(buffer, SocketFlags.None);
                // todo maybe increase buffer here dynamically
                if (bytesRead == 0) throw new SocketException();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                try { socket.Close(); }
                catch (SocketException) { }
                return false;
            }
        }
    }
}