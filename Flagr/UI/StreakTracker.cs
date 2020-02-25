using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.UI
{
    class StreakTracker : UIElement
    {
        
        public int CurrentStreak { get; protected set; }
        public double CurrentMultiplier { get { return multipliers[CurrentStreak]; } }

        private int maxStreak = 4;

        private Image[] odometers;
        private float scale = .4f;
        Pen pointerPen;
        Point centerPoint;
        Point endPoint = Point.Empty;

        private int[] targets = { 80, 105, 160, 220, 280 };
        private int[] wiggle = { 0, 0, 0, 12, 20 };
        private int[] speeds = { 240, 240, 360, 480, 600 };
        private double[] multipliers = { 1d, 1.2d, 1.4d, 1.7d, 2d };
        private float rotation = 80;

        private Random rng;

        private int radius = 75;

        public StreakTracker()
        {
            odometers = new Image[maxStreak+1];

            for(int i = 0; i <= maxStreak; i++)
                odometers[i] = (Image)Properties.Resources.ResourceManager.GetObject("Odo"+i);

            Size = new Size((int)(scale * odometers[0].Width), (int)(scale * odometers[0].Height));

            for (int i = 0; i < odometers.Length; i++)   
                odometers[i] = Util.ResizeImage(odometers[i], Size.Width, Size.Height);

            pointerPen = new Pen(Brushes.Black, 8);
            pointerPen.SetLineCap(System.Drawing.Drawing2D.LineCap.Round, System.Drawing.Drawing2D.LineCap.Round, System.Drawing.Drawing2D.DashCap.Round);
            centerPoint = new Point(origin.X + odometers[0].Width / 2, origin.Y + odometers[0].Height / 2 + 3);

            rng = new Random();

            DrawMode = DrawMode.Centered;
        }

        protected override void SetOrigin()
        {
            base.SetOrigin();
            centerPoint = new Point(origin.X + odometers[0].Width / 2, origin.Y + odometers[0].Height / 2 + 3);
        }


        public void UpdateStreak(bool correct)
        {
            if (correct)
            {
                if (CurrentStreak < maxStreak) CurrentStreak++;
            }
            else
                CurrentStreak = 0;
        }

        public override void Update(DeltaTime deltaTime)
        {
            base.Update(deltaTime);

            UpdatePointer(deltaTime);
        }

        private void UpdatePointer(DeltaTime deltaTime)
        {
            float target = targets[CurrentStreak] + (rng.Next(0, wiggle[CurrentStreak]) - wiggle[CurrentStreak]/2);
            float rotationSpeed = speeds[CurrentStreak];

            if(rotation != target)
            {
                float dif = target - rotation;
                float move = (rotationSpeed * deltaTime.Seconds);

                if (Util.AbsoluteFloat(dif) < Util.AbsoluteFloat(move)) move = Util.AbsoluteFloat(dif);

                if (dif < 0)
                    move *= -1;

                rotation += move;
            }

            endPoint.X = centerPoint.X + (int)(radius * Math.Cos(Util.DegreeToRadian(rotation+90)));
            endPoint.Y = centerPoint.Y + (int)(radius * Math.Sin(Util.DegreeToRadian(rotation+90)));
        }


        public override void Draw(Graphics g)
        {
            g.DrawImage(odometers[CurrentStreak], origin);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawLine(pointerPen, centerPoint, endPoint);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        }
    }
}
