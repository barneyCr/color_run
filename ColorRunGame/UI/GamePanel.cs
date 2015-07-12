using ColorRunGame.GameEntities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorRunGame.UI
{
    class GamePanel : Panel
    {
        public GamePanel()
            : base(
                )
        {
            this.DoubleBuffered = true;
            this.Paint += GamePanel_Paint;
        }
        


        //dynamic screen;
        //dynamic player;
        void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            Screen screen = Program.GameManager.Screen;
            var center = screen.CoordinateToScreen(Program.GameManager.Player.FindCentralPointOfCells());
            screen.SetXY(center.X, center.Y);
            screen.SetSight(new SizeF(700f, 437.5f));
             
            //screen.setSight(player.cells.Sum((Func<Cell,int>)(c=>c.Mass)).toRadius()*25);
           


            throw new NotImplementedException();
        }

        private GameForm parent;

        public static GamePanel CreateNewInstance(GameForm parent, int mpx, int mpy, int viewdiag)
        {
            GamePanel ret = new GamePanel();
            ret.parent = parent;

            return ret;
        }

        public void FormFullScreen()
        {
            int wid = parent.Width, hi = parent.Height;
            this.DoResize(wid * 0.95f, hi * 0.95f);
        }

        private void DoResize(float p1, float p2)
        {
            this.Size = new Size((int)p1, (int)p2);
            this.Location = new Point(
                (this.parent.Width - this.Width) / 2,
                (this.parent.Height - this.Height) / 2
                );
            this.Resize += GamePanel_Resize;
        }

        void GamePanel_Resize(object sender, EventArgs e)
        {
            // TODO
        }



        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GamePanel));
            this.SuspendLayout();
            // 
            // GamePanel
            // 
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ResumeLayout(false);

        }
    }
}
