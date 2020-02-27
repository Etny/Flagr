using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Flagr.Flags
{
    class Flag
    {
        public Bitmap Image { get; protected set; }
        public Size ImageSize { get; protected set; }
        public String Country { get; protected set; }
        public String ImageName { get; protected set; }
        public float Scale { get; protected set; }
        public bool IsImageLoaded { get; protected set; }

        public Flag(XmlNode node, int MaxWidth, int MaxHeight)
        {
            Country = node["country"].InnerText;
            ImageName = node["image"].InnerText;
            Scale = float.Parse(node["scale"].InnerText);

            int baseWidth = int.Parse(node["width"].InnerText);
            int baseHeight = int.Parse(node["height"].InnerText);

            SetImageSize(baseWidth, baseHeight, MaxWidth, MaxHeight);
        }

        public Flag(Bitmap Image, String Country, String ImageName, int MaxWidth, int MaxHeight)
        {
            this.Country = Country;
            this.ImageName = ImageName;

            this.Scale = 1;

            SetImageSize(Image.Width, Image.Height, MaxWidth, MaxHeight);
        }

        private void SetImageSize(int baseWidth, int baseHeight, int MaxWidth, int MaxHeight)
        {
            if (baseWidth > MaxWidth)
                Scale = (float)((float)MaxWidth / (float)baseWidth);

            if ((int)(baseHeight * Scale) > MaxHeight)
                Scale = (float)((float)MaxHeight / (float)(baseHeight * Scale));

            this.ImageSize = new Size((int)(baseWidth * Scale), (int)(baseHeight * Scale));
        }

        public void LoadImage()
        {
            Bitmap img = (Bitmap)Properties.Resources.ResourceManager.GetObject(ImageName);

            int ScaledWidth = (int)(img.Width * Scale);
            int ScaledHeight = (int)(img.Height * Scale);

            Image = new Bitmap(ScaledWidth, ScaledHeight);
            Graphics tempGraphics = Graphics.FromImage(Image);

            tempGraphics.DrawImage(img, 0, 0, ScaledWidth, ScaledHeight);
            tempGraphics.Dispose();

            img.Dispose();

            IsImageLoaded = true;
        }

        public void UnloadImage()
        {
            IsImageLoaded = false;
            if(Image != null) Image.Dispose();
        }

    }
}
