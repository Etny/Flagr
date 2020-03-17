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

        public static Bitmap RotateImage(Image img, float rotationAngle, bool fancySizing)
        {

            if (rotationAngle < 0) rotationAngle = 360 + rotationAngle;

            int rotWidth = 0;
            int rotHeight = 0;

            if (fancySizing)
            {
                if (rotationAngle % 180 == 0)
                {
                    rotWidth = img.Width;
                    rotHeight = img.Height;
                }
                else if ((rotationAngle - 90) % 180 == 0)
                {
                    rotWidth = img.Height;
                    rotHeight = img.Width;
                }
                else
                {
                    bool widthFacingUp = rotationAngle < 90 || (rotationAngle > 180 && rotationAngle - 180 < 90);
                    int facingUp = widthFacingUp ? img.Width : img.Height;
                    int facingSide = widthFacingUp ? img.Height : img.Width;

                    rotWidth = (int)((facingUp * Math.Cos(DegreeToRadian(rotationAngle % 90))) + (facingSide * Math.Cos(DegreeToRadian(90 - (rotationAngle % 90)))));
                    rotHeight = (int)((facingSide * Math.Cos(DegreeToRadian(rotationAngle % 90))) + (facingUp * Math.Cos(DegreeToRadian(90 - (rotationAngle % 90)))));
                }

                if (rotWidth % 2 != img.Width % 2)
                    rotWidth++;

                if (rotHeight % 2 != img.Height % 2)
                    rotHeight++;
            }
            else
            {
                int maxDimension = (int)Math.Sqrt((img.Width * img.Width) + (img.Height * img.Height));
                rotWidth = maxDimension;
                rotHeight = maxDimension;
            }

            Bitmap bmp = new Bitmap(rotWidth, rotHeight);
            Graphics gfx = Graphics.FromImage(bmp);

            //      gfx.FillRectangle(Brushes.Black, 0, 0, rotWidth, rotHeight);

            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

           // gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
          //  gfx.SmoothingMode = SmoothingMode.None;

            gfx.DrawImage(img, new Point((rotWidth / 2) - (img.Width / 2), (rotHeight / 2) - (img.Height / 2)));

            gfx.Dispose();

            return bmp;
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
