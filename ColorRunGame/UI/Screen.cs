using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorRunGame.UI
{
    class Screen
    {
        public const double WIDTH_HEIGHT_RATIO = 1.6;
        public const double INVERSED_RATIO = 1 / 1.6; // (=0,625)

        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 800;
        //public double INIT_GU_PIXEL_RATIO = 8.75;
        //public int MapScreenRatio = 16; // default: 16
        //public int MapScreenAreaRatio = 256; // 16*16
        public const double CELL_RADIUS_SCREEN_WIDTH_RATIO = 1 / 16;

        public Screen(Size gameTotalArea, Size screenSize)
        {
            this._platformSize = gameTotalArea;
            this._physicalScreenSize = new Size(screenSize.Width, screenSize.Height);
        }

        public void SetPlatformArea(int wid, int hei)
        {
            this._platformSize = new Size(wid, hei);
        }

        /// <summary>
        /// Top-left corner of screen
        /// </summary>
        public int X, Y;
        public void SetXY(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets the center of the screen in game coordinates
        /// </summary>
        public Point ScrCenter
        {
            get
            {
                var sgt = this.GetSight();
                return new Point(this.X + sgt.Width / 2, this.Y + sgt.Height / 2); 
            }
        }

        public Point PhysicalCenter
        {
            get { return new Point(this._physicalScreenSize.Width / 2, this._physicalScreenSize.Height / 2); }
        }

        public Size GetSight()
        {
            double totalZoom = _zoom * _artificialZoom;
            return new Size((int)(_physicalScreenSize.Width * totalZoom), (int)(_physicalScreenSize.Height * totalZoom));
        }

        public void SetSight(SizeF sight)
        {
            if (sight.Width / sight.Height == Screen.WIDTH_HEIGHT_RATIO)
            {
                this._zoom = 1 / (sight.Width / this._physicalScreenSize.Width);
                // how many times the sight is bigger than what the screen can show?

                // inversed because: if we show 1580 pixels on a 1280 wide screen, ratio will be something like ~1,23475
                // which is 123,475%
                // but actually we are zooming out so we should have 81%

                // inversion can also be done by just switching the places of the two divided numbers but i prefer it this way because it's more clear
            }
            else
                throw new ResolutionException();
        }

        /// <summary>
        /// This is linked to "MapByX, ByY"
        /// We use this to set the sight
        /// </summary>
        private double _zoom;
        /// <summary>
        /// Artificial zooming done by the player
        /// </summary>
        private double _artificialZoom=1;
        public void ZoomIn(int percentagePoints)
        {
            _artificialZoom += percentagePoints / 100;
        }
        public void ZoomOut(int percentagePoints)
        {
            _artificialZoom -= percentagePoints / 100;
        }
        public void ResetZoom() { _artificialZoom = 1; }
        public double ArtificialZoom { get { return _artificialZoom; } }

        public Point CoordinateToScreen(Point mapCoord)
        {
            // for example: cell at 9800-4350
	        // screen at 10000-4000
	
            //cell will be drawn at 9800-10000 = -200   X
            //                      4350-4000  = +350   Y

            double _totalZoom = _zoom * _artificialZoom;
            // 0,81 = rendering
            // 140% = zoom by player
            // => result = 0,81*1,41 = 1,1421 (114%)

            return new Point(
                (int)((mapCoord.X - this.X)*_totalZoom),
                (int)((mapCoord.Y - this.Y)*_totalZoom)
            );
        }

        private Size _platformSize;
        private Size _physicalScreenSize;

        public int PhysicalWidth
        {
            get { return this._physicalScreenSize.Width; }
            set { this._physicalScreenSize.Width = value; }
        }

        public int PhysicalHeight
        {
            get { return this._physicalScreenSize.Height; }
            set { this._physicalScreenSize.Height = value; }
        }

        public bool CircleAppearsInScreen(
            Point centerOfObj, Point centerOfScreen,
            double radiusOfObject,
            out double? xDraw, out double? yDraw)
        {
			throw new NotImplementedException();
		
            int topX, topY;
            topX = centerOfScreen.X - PhysicalWidth / 2;
            topY = centerOfScreen.Y - PhysicalHeight / 2;

            Rectangle currentScreenCapture = new Rectangle(topX, topY, this.PhysicalWidth, this.PhysicalHeight);
            if (currentScreenCapture.Contains(centerOfObj))
            {
                xDraw = centerOfObj.X;
                yDraw = centerOfObj.Y;
                return true;
            }
            else // center of object is outside screen bounds
                // checking if we have to draw anything at all
            {
                if (centerOfObj.X < currentScreenCapture.Left)
                {
                    if (centerOfObj.X + radiusOfObject <= currentScreenCapture.Left)
                    {
                        xDraw = yDraw = null;
                        return false;
                    }
                    else
                    {

                    }
                }
            }
            xDraw = yDraw = -1;
            return false;
        }
    }
}
