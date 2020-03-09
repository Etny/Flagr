using Flagr.Flags;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Flagr.UI
{
    class Boat : UIElement
    {
        public FlagContainer Container;
        public float Speed;
        public int Weight;
        public int FlagHeight;

        private Point containerOffset;
        private Point numberOffset;
        private Image image;

        public Boat(XmlNode node, float scale)
        {
            Container = new FlagContainer();
            Container.DrawMode = DrawMode.TopLeft;

            containerOffset = new Point((int)(int.Parse(node["flagX"].InnerText) * scale), (int)(int.Parse(node["flagY"].InnerText) * scale));
            numberOffset = new Point((int)(int.Parse(node["numX"].InnerText) * scale), (int)(int.Parse(node["numY"].InnerText) * scale));

            Speed = float.Parse(node["speed"].InnerText);
            Weight = (int)(int.Parse(node["watersink"].InnerText) * scale);
            FlagHeight = (int)(int.Parse(node["flagHeight"].InnerText) * scale);

            image = (Image)Properties.Resources.ResourceManager.GetObject(node["image"].InnerText);

            if (scale != 1)
                image = Util.ResizeImage(image, (int)(image.Width * scale), (int)(image.Height * scale));

            Size = image.Size;

            DrawMode = DrawMode.TopLeft;
        }

        public void SetFlag(Flag flag)
        {
            Container.Flag = flag.getDifferentSize(Program.Flags.MaxWidth, FlagHeight);
        }

        public override void Draw(Graphics g)
        {
            Container.SetLocation(Location.X + containerOffset.X, Location.Y + containerOffset.Y);

            Container.Draw(g);
            g.DrawImage(image, origin.X, origin.Y, Size.Width, Size.Height);
        }
    }
}
