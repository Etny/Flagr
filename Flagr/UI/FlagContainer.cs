using Flagr.Flags;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.UI
{
    class FlagContainer : UIElement
    {
        public bool DrawPlaceholder = false;

        public Flag Flag
        {
            get
            {
                return flag;
            }

            set
            {
                flag = value;
                Size = value.ImageSize;
            }
        }

        private Flag flag;

        public FlagContainer()
        {
            DrawMode = DrawMode.Centered;
        }

        public override void Draw(Graphics g)
        {
            if (flag == null)
                return;

            if (flag.IsImageLoaded)
            {
                //var temp = g.InterpolationMode;
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                
                g.DrawImage(flag.Image, origin);

                //g.InterpolationMode = temp;
            }
            else if (DrawPlaceholder)
                g.FillRectangle(Brushes.Gray, origin.X, origin.Y, flag.ImageSize.Width, flag.ImageSize.Height);
        }
    }
}
