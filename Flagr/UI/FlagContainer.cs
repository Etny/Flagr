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
            if(flag != null && flag.IsImageLoaded)
                g.DrawImage(flag.Image, origin.X, origin.Y);
        }
    }
}
