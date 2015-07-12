using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWF = System.Windows.Forms;

namespace ColorRunGame.UI
{
    static class FormHelpers
    {
        public static void GoFullscreen(Form f, bool fullscreen)
        {
            if (fullscreen)
            {
                f.WindowState = FormWindowState.Normal;
                f.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                f.Bounds = SWF.Screen.PrimaryScreen.Bounds;
                
            }
            else
            {
                f.WindowState = FormWindowState.Maximized;
                f.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        }
    }
}