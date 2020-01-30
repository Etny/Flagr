using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.Flags
{
    class Flag
    {
        public Bitmap Image { get; protected set; }
        public Bitmap RawImage { get; protected set; }
        public String Country { get; protected set; }

        public Flag(Bitmap Image, String Country)
        {
            this.Image = Image;
        //  this.RawImage = Image;
            this.Country = Country;
        }

        public Flag(Bitmap Image, String Country, int MaxWidth, int MaxHeight) : this(Image, Country)
        {
            float scale = 1;

            if (Image.Width > MaxWidth)
                scale = (float)((float)MaxWidth / (float)Image.Width);

            if ((int)(Image.Height * scale) > MaxHeight)
                scale = (float)((float)MaxHeight / (float)(Image.Height * scale));

            if (scale == 1) return;

            int scaledWidth = (int)(Image.Width * scale);
            int scaledHeight = (int)(Image.Height * scale);

            //Console.WriteLine(scale);

            Bitmap temp = new Bitmap(scaledWidth, scaledHeight);
            Graphics tempGraphics = Graphics.FromImage(temp);
                
            tempGraphics.DrawImage(Image, 0, 0, scaledWidth, scaledHeight);
            tempGraphics.Dispose();

            this.Image = temp;
        }


    }
}
