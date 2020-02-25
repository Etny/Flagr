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
        public static float DegreeToRadian(float angle)
        {
            return (float)(Math.PI * angle / 180.0);
        }

        public static float RadianToDegree(float angle)
        {
            return (float)(angle * (180.0 / Math.PI));
        }

        public static float AbsoluteFloat(float f)
        {
            return f < 0 ? -f : f;
        }
    }
}
