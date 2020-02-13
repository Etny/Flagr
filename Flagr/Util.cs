using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr
{
    class Util
    {
        public static Image ResizeImage(Image img, int width, int height)
        {
            Bitmap newImg = new Bitmap(width, height);
            Graphics gfx = Graphics.FromImage(newImg);

            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

            gfx.DrawImage(img, 0, 0, width, height);

            gfx.Dispose();

            return newImg;
        }

    }
}
