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
        public AnswerBlock CurrentBlock;
        public bool CorrectlyAnswered = false;
        public bool Answered = false;
        public float X = (float)Program.Width + 10;
        public bool OutOfBounds = false;

        public Color IndicatorColor = Color.Gray;
        public TextLabel IndicatorLabel;
        private int indicatorSize = 40;

        protected Point containerOffset;
        protected Point numberOffset;
        protected int disturbPoint;
        protected Image image;
        protected Image sinkingImage;
        protected bool createdImage = false;
        protected int trueWidth;

        protected float SpeedMod = 1f;
        protected float currentSpeedOffTime = 0f;
        protected bool speedingOff = false;

        protected static float maxSpeedMod = 6f;
        protected static float speedOffTime = 1.2f;
        protected static BezierCurve speedOffCurve = new BezierCurve(new PointF[] { new PointF(.9f, -.15f), new PointF(1f, .67f) }, speedOffTime);

        protected bool sinking = false;
        protected float sinkY = 0;
        protected float sinkingRot = 0;
        protected float maxSinkingRot = 45;
        protected float sinkingRotSpeed = 90;

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

            trueWidth = image.Width;
            if (Container.Flag.ImageSize.Width > (trueWidth - containerOffset.X))
                trueWidth += Container.Flag.ImageSize.Width - (trueWidth - containerOffset.X);
        }

        public Boat Clone()
        {
            Boat b = new Boat()
            {
                containerOffset = this.containerOffset,
                numberOffset = this.numberOffset,
                disturbPoint = this.disturbPoint,
                image = (Image)this.image.Clone(),
                Speed = this.Speed,
                Weight = this.Weight,
                FlagHeight = this.FlagHeight,
                Size = this.Size
            };
            b.SetLocation((int)X, SpeedState.WaterLine - Size.Height + Weight);

            return b;
        }
        
        public void CreateImage()
        {
            Image temp = new Bitmap(trueWidth, image.Height);

            Graphics g = Graphics.FromImage(temp);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;


            g.DrawImage(image, 0, 0);

            Container.SetLocation(containerOffset.X, containerOffset.Y);
            Container.Draw(g);

            g.FillEllipse(new SolidBrush(IndicatorColor), numberOffset.X - (indicatorSize / 2),
                                                              numberOffset.Y - (indicatorSize / 2),
                                                              indicatorSize, indicatorSize);
            IndicatorLabel.SetLocation(numberOffset.X, numberOffset.Y);
            IndicatorLabel.Draw(g);

            g.Dispose();
            image.Dispose();

            image = temp;
            Size = image.Size;
            createdImage = true;
        }

        public void SpeedOff()
        {
            speedingOff = true;
        }

        public void Sink()
        {
            sinking = true;
            sinkingImage = Util.RotateImage(image, 0, false); ;
        }

        public void Update(DeltaTime deltaTime, FancyWater water, float baseSpeed)
        {
            if (sinking)
            {
                if(sinkingRot < maxSinkingRot)
                {
                    sinkingRot += sinkingRotSpeed * deltaTime.Seconds;
                    sinkingImage.Dispose();
                    sinkingImage = Util.RotateImage(image, sinkingRot, false);
                }

                sinkY += Weight * 12 * deltaTime.Seconds;

                if ((origin.Y + sinkY) + (image.Height / 2) - (sinkingImage.Height / 2) >= SpeedState.WaterLine)
                    OutOfBounds = true;

                return;
            }

            if (speedingOff)
            {
                if (currentSpeedOffTime < speedOffTime)
                {
                    currentSpeedOffTime += deltaTime.Seconds;
                    if (currentSpeedOffTime > speedOffTime) currentSpeedOffTime = speedOffTime;

                    SpeedMod = 1 + ((maxSpeedMod - 1) * speedOffCurve.GetValue(currentSpeedOffTime));
                }
            }

            X -= ((SpeedMod * Speed) * baseSpeed) * deltaTime.Seconds;
            origin.X = (int)X;

            int disturbX = origin.X + disturbPoint;

            if(disturbX > 0)
            {
                float waterDelta = (Speed * SpeedMod * 50) * deltaTime.Seconds;
                int closest = water.ClosestPoint(disturbX);

                water.MovePoint(closest, -waterDelta);
                water.MovePoint(closest + 1, waterDelta);
            }

            if (!OutOfBounds && X <= -trueWidth)
                OutOfBounds = true;
        }

        public override void Draw(Graphics g)
        {
            if (createdImage)
            {
                if (!sinking)
                {
                    g.DrawImage(image, origin.X, origin.Y, Size.Width, Size.Height);
                }
                else
                {
                    g.DrawImage(sinkingImage, origin.X + (image.Width / 2)- (sinkingImage.Width / 2), (origin.Y + sinkY) + (image.Height / 2) - (sinkingImage.Height / 2), sinkingImage.Width, sinkingImage.Height);
                }
            }
            else
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
}
