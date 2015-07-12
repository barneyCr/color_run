using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorRunGame.UI
{
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();
            this.Load += GameForm_Load;
        }

        void GameForm_Load(object sender, EventArgs e)
        {
            if (Program.NetworkManager.Connect())
            {
                MessageBox.Show("Connected");
            }
            else
                MessageBox.Show("Failed to connect");
        }
         
        public Size PanelSize { get { return this.gamePanel1.Size; } }
    }
}
