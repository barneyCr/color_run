using ColorRunGame.Network;
using ColorRunGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorRunGame
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            com = new Communication();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            Application.Run(GForm);
        }

        public static void RunNewGameForm(string username, ColorRunGame.UI.Form1 close)
        {
            Program.Username = username;
            GForm = new GameForm();
            close.Close();
        }

        #region Global variables and backing fields
        static GameForm _form2;
        public static GameForm GForm
        {
            get { return _form2; }
            private set { Program._form2 = value; }
        }

        static string _username;
        public static string Username
        {
            get { return _username; }
            private set { Program._username = value; }
        }

        static Communication com;
        public static Communication NetworkManager { get { return com; } }


        static GameManager man;
        public static GameManager GameManager { get { return man; } }
        #endregion

    }
}
