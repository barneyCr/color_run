using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ColorRunServer.Network
{
    /// <summary>
    /// Encapsulates network traffic and communication with the client
    /// </summary>
    public class Client
    {
        public Client(string username, Socket socket)
        {
            this.Username = username;
            this.Socket = socket;
        }
        public void Send(string message, params object[] p)
        {
            this.Send(string.Format(message, p));
        }
        public void Send(string message)
        {
            this.Socket.Send(message);
            //Console.WriteLine("Packet to" + this.Username +": "+ message);
            //Program.Write("Sending message of type " + Server.GetHeaderType(message.Split('|')[0]) + " to " + this.Username + "[" + this.UserID + "]",
            //    "PacketLogs", ConsoleColor.Blue);
        }


        public Socket Socket { get; set; }
        public string Username { get; set; }
        public int UserID { get; set; }

        public string GetEndpoint()
        {
            return this.Socket.RemoteEndPoint.ToString().Split(':')[0];
        }
    }
}
