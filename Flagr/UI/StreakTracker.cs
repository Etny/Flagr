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

        private int maxStreak = 4;

        private Image[] odometers;
        private float scale = .4f;

        Pen pointerPen;
        Point centerPoint;
        Point endPoint = Point.Empty;

        private float[] targets = { 80, 105, 150, 205, 280};
        private float rotation = 80;
        private float rotationSpeed = 120f;

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
            float target = targets[CurrentStreak];

            if(rotation != target)
            {
                if (rotation > target)
                    rotation -= (rotationSpeed * deltaTime.Seconds);
                else
                    rotation += (rotationSpeed * deltaTime.Seconds);
            }

            //float radius = (float)odometers[0].Width / 2;

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
