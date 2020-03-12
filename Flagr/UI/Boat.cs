using Flagr.Flags;
using Flagr.States;
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
        public float X = (float)Program.Width + 10;

        public Color IndicatorColor = Color.Gray;
        public TextLabel IndicatorLabel;
        private int indicatorSize = 40;

        protected Point containerOffset;
        protected Point numberOffset;
        protected int disturbPoint;
        protected Image image;

        public Boat(XmlNode node, float scale)
        {
            Container = new FlagContainer();
            Container.DrawMode = DrawMode.TopLeft;

            containerOffset = new Point((int)(int.Parse(node["flagX"].InnerText) * scale), (int)(int.Parse(node["flagY"].InnerText) * scale));
            numberOffset = new Point((int)(int.Parse(node["numX"].InnerText) * scale), (int)(int.Parse(node["numY"].InnerText) * scale));

            disturbPoint = (int)(int.Parse(node["disturbX"].InnerText) * scale);


            Speed = float.Parse(node["speed"].InnerText);
            Weight = (int)(int.Parse(node["watersink"].InnerText) * scale);
            FlagHeight = (int)(int.Parse(node["flagHeight"].InnerText) * scale);

            image = (Image)Properties.Resources.ResourceManager.GetObject(node["image"].InnerText);

            if (scale != 1)
                image = Util.ResizeImage(image, (int)(image.Width * scale), (int)(image.Height * scale));

            Size = image.Size;

            this.DrawMode = DrawMode.TopLeft;
        }

        public Boat()
        {
            Container = new FlagContainer();
            Container.DrawMode = DrawMode.TopLeft;
            this.DrawMode = DrawMode.TopLeft;


            IndicatorLabel = new TextLabel()
            {
                Font = new Font("Arial", 20, FontStyle.Bold),
                Color = Color.White,
                DrawMode = DrawMode.Centered
            };
        }
    

  

        public void SetFlag(Flag flag)
        {
            if (Container.Flag != null) Container.Flag.UnloadImage();
            Container.Flag = flag.getDifferentSize(Program.Flags.MaxWidth, FlagHeight);
            Container.Flag.LoadImage();
        }

        public Boat Clone()
        {
            Boat b = new Boat()
            {
                containerOffset = this.containerOffset,
                numberOffset = this.numberOffset,
                disturbPoint = this.disturbPoint,
                image = this.image,
                Speed = this.Speed,
                Weight = this.Weight,
                FlagHeight = this.FlagHeight,
                Size = this.Size
            };
            b.SetLocation((int)X, SpeedState.WaterLine - Size.Height + Weight);

            return b;
        }

        public void Update(DeltaTime deltaTime, FancyWater water, float baseSpeed)
        {
            X -= (Speed * baseSpeed) * deltaTime.Seconds;
            origin.X = (int)X;

            if(X > 0) water.MovePoint(water.ClosestPoint(origin.X + disturbPoint), -(Speed * 85) * deltaTime.Seconds);

        }

        public override void Draw(Graphics g)
        {
            Container.SetLocation(origin.X + containerOffset.X, origin.Y + containerOffset.Y);
            Container.Draw(g);

            g.DrawImage(image, origin.X, origin.Y, Size.Width, Size.Height);

            g.FillEllipse(new SolidBrush(IndicatorColor), origin.X + numberOffset.X - (indicatorSize / 2),
                                                          origin.Y + numberOffset.Y - (indicatorSize / 2),
                                                          indicatorSize, indicatorSize);

            IndicatorLabel.SetLocation(origin.X + numberOffset.X, origin.Y + numberOffset.Y);
            IndicatorLabel.Draw(g);
        }
    }
}
