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
        public Size ImageSize { get; protected set; }
        public String Country { get; protected set; }

        public bool IsImageLoaded { get; protected set; }


        private float scale;
        private string imgName;

        public Flag(Bitmap Image, String Country, String ImageName, int MaxWidth, int MaxHeight)
        {
            this.Country = Country;
            this.imgName = ImageName;


             this.scale = 1;

            if (Image.Width > MaxWidth)
                scale = (float)((float)MaxWidth / (float)Image.Width);

            if ((int)(Image.Height * scale) > MaxHeight)
                scale = (float)((float)MaxHeight / (float)(Image.Height * scale));

            this.ImageSize = new Size((int)(Image.Width * scale), (int)(Image.Height * scale));

            Image.Dispose();
        }

        public void LoadImage()
        {
            Bitmap img = (Bitmap)Properties.Resources.ResourceManager.GetObject(imgName);

            int scaledWidth = (int)(img.Width * scale);
            int scaledHeight = (int)(img.Height * scale);

            Image = new Bitmap(scaledWidth, scaledHeight);
            Graphics tempGraphics = Graphics.FromImage(Image);

            tempGraphics.DrawImage(img, 0, 0, scaledWidth, scaledHeight);
            tempGraphics.Dispose();

            img.Dispose();

            IsImageLoaded = true;
        }

        public void UnloadImage()
        {
            IsImageLoaded = false;
            Image.Dispose();
        }

    }
}
