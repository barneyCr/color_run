using ColorRunServer.GameEngine;
using ColorRunServer.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorRunServer
{
    partial class Program
    {
        public static readonly string[] ReservedNames = new[] { "admin", "system", "server", "TODEA" };
        public static readonly string SERVER_INI_PATH = Environment.CurrentDirectory + @"\server.ini";

        const ConsoleColor DefaultColor = ConsoleColor.DarkGray;
        const int VARIOUS_JOB_TIMER_TICK = 7500;
        static Server server;
        static bool WriteInLogFile;
        static bool firstEditOfSettings = true;

        static StreamWriter writer = new StreamWriter("logs.txt", true);

        public static WorldManager WorldManager;

        public static void Write(string msg, string header = "", ConsoleColor color = DefaultColor)
        {
            string msg_ = (!string.IsNullOrWhiteSpace(header) ? (string.Concat(DateTime.Now.ToLongTimeString(), " >>> [", header, "]  ", msg)) : (string.Concat(">>>", "   ", msg)));
            var _col = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg_);
            Console.ForegroundColor = _col;

            if (WriteInLogFile)
            {
                writer.WriteLine(msg_);
                writer.Flush();
            }
        }
        public static void Write(LogMessageType type, string msg, params object[] obj)
        {
            string head = GetEnum(type);
            msg = string.Format(msg, obj);
            switch (type)
            {
                case LogMessageType.Config:
                    Write(msg, head, ConsoleColor.Red);
                    break;

                case LogMessageType.Network:
                    Write(msg, head, ConsoleColor.Green);
                    break;

                case LogMessageType.Chat:
                    Write(msg, head, ConsoleColor.DarkCyan);
                    break;

                case LogMessageType.Auth:
                    Write(msg, head);
                    break;

                case LogMessageType.UserEvent:
                    Write(msg, head, ConsoleColor.DarkYellow);
                    break;

                case LogMessageType.Packet:
                    Write(msg, head);
                    break;


                default:
                    Write(msg);
                    break;
            }
        }
        
        
        static void Main(string[] args)
        {
            Program.Settings = LoadSettings();
            InitializeConsole();

            Program.server = new Server(13001, 125, GameModes.FYS, "");
            Program.WorldManager = new WorldManager(server);
            Program.server.listenThread.Start();


        }

        static void InitializeConsole()
        {
            Console.Title = "Chat Server";

            ConsoleColor bckCol = Console.BackgroundColor = Parse<ConsoleColor>(Settings["consoleBackColor"]);
            Console.ForegroundColor = bckCol == ConsoleColor.Black ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;

            Console.SetWindowSize(105, 25);
            Console.SetBufferSize(105, 1500);

            Console.Clear();
            Program.Write(LogMessageType.Config, "Settings loaded");
        }


        static string GetEnum<T>(T _enum) where T : struct
        {
            return Enum.GetName(typeof(T), _enum);
        }

        static T Parse<T>(string from) where T : struct // == where T : Enum
        {
            T auth;
            if (Enum.TryParse<T>(from, true, out auth))
                return auth;
            else return default(T);
        }
    }
}
