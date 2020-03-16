using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Flagr.UI
{
    class FancyWater : UIElement
    {
        public PointF[] WaterPoints;

        private int line, height, surfacePoints;
        private Brush waterBrush;

        private float cutoff = 1.2f;
        private float k = .025f; //Spring Constant
        private float maxHeight = 50;
        private float dampening = .065f;
        private float spread = .01f;
        private float[] pointVel;
        private float[] deltaHeight;

        public int pointDistance = 100;

        public FancyWater(int waterline, int waterHeight, int surfacePoints, Color waterColor)
        {
            line = waterline;
            height = waterHeight;
            this.surfacePoints = surfacePoints;
           // dampening /= (surfacePoints - 1) / 20;

            waterBrush = new LinearGradientBrush(
                new Point(Program.Width / 2, line), 
                new Point(Program.Width / 2, line + height),
                waterColor,
                Color.FromArgb(0, waterColor.R, waterColor.G, waterColor.B));

            pointVel = new float[surfacePoints];
            deltaHeight = new float[surfacePoints];

            PopulatePoints();
        }

        //Coulnd't get this to work with deltaTime. Since it's just a misc visual effect I don't think it's worth the time to fix it.
        private void UpdatePoint(int pointIndex, DeltaTime deltaTime)
        {
            float mod = deltaTime.Seconds / .01f;
            if (mod > 10) mod = 10; 
            
            float dif = WaterPoints[pointIndex].Y - line;
            float acc = -k * dif - (dampening * pointVel[pointIndex]) * mod;

            deltaHeight[pointIndex] += pointVel[pointIndex];
            pointVel[pointIndex] += acc;

            if (pointIndex > 0) pointVel[pointIndex - 1] += mod * spread * (WaterPoints[pointIndex].Y - WaterPoints[pointIndex - 1].Y);
            if (pointIndex < surfacePoints - 1) pointVel[pointIndex + 1] += mod * spread * (WaterPoints[pointIndex].Y - WaterPoints[pointIndex + 1].Y);
        }

        public void MovePoint(int index, float delta)
        {
            if (index >= deltaHeight.Length)
                return;

            deltaHeight[index] += delta;
        }

        public int ClosestPoint(int X)
        {
            if (X >= Program.Width)
                return surfacePoints - 1;

            if (X <= 0)
                return 0;

            X += pointDistance / 2;
            return (X - (X % pointDistance)) / pointDistance;
        }

        public void Update(DeltaTime deltaTime)
        {
            for (int i = 0; i < surfacePoints; i++)
                UpdatePoint(i, deltaTime);

            for (int i = 0; i < surfacePoints; i++)
            {
                if (Math.Abs(WaterPoints[i].Y + deltaHeight[i]) - line < maxHeight)

                    WaterPoints[i].Y += deltaHeight[i];
                else
                {
                    WaterPoints[i].Y = (Math.Sign(deltaHeight[i]) * maxHeight) + line;
                    //pointVel[i] = 0;
                }
                
                deltaHeight[i] = 0;
            }
        }

        private void PopulatePoints()
        {
            WaterPoints = new PointF[surfacePoints + 2];

            pointDistance = (int)Math.Ceiling((double)Program.Width / (surfacePoints - 1));

            for (int i = 0; i < surfacePoints; i++)
            {
                WaterPoints[i] = new Point(i * pointDistance, line);
                pointVel[i] = 0;
                deltaHeight[i] = 0;
            }

            WaterPoints[WaterPoints.Length - 2] = new Point((surfacePoints - 1) * pointDistance, line + height);
            WaterPoints[WaterPoints.Length - 1] = new Point(0, line + height);


        }

        public override void Draw(Graphics g)
        {
            g.FillPolygon(Brushes.DeepSkyBlue, WaterPoints);
        }

        private class WaterPoint
        {
            public float Pos, Vel;
            public int Target;

            readonly float k = .05f;


            public void Update()
            {
                float dif = Pos - Target;
                float acc = -k * dif;

                Pos += Vel;
                Vel += acc;
            }

        }
    }
}
