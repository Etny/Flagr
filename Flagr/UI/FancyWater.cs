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

        private float cutoff = 0f;
        private float k = .02f; //Spring Constant
        private float dampening = .05f;
        private float spread = .01f;
        private float[] pointVel;
        private float[] deltaHeight;

        public FancyWater(int waterline, int waterHeight, int surfacePoints, Color waterColor)
        {
            line = waterline;
            height = waterHeight;
            this.surfacePoints = surfacePoints;

            waterBrush = new LinearGradientBrush(
                new Point(Program.Width / 2, line), 
                new Point(Program.Width / 2, line + height),
                waterColor,
                Color.FromArgb(0, waterColor.R, waterColor.G, waterColor.B));

            pointVel = new float[surfacePoints];
            deltaHeight = new float[surfacePoints];

            PopulatePoints();
        }

        //Coulnd't get this to work with deltaTime. Since it's just a misc visual effect I don't think it's worth the time to fix it
        private void UpdatePoint(int pointIndex, DeltaTime deltaTime)
        {
            if (pointIndex == 5) Console.WriteLine(line - WaterPoints[pointIndex].Y + " " + pointVel[pointIndex] + " " + deltaTime.Seconds);

            float dif = WaterPoints[pointIndex].Y - line;
            float acc = -k * dif - (dampening * pointVel[pointIndex]);

            deltaHeight[pointIndex] += pointVel[pointIndex];
            pointVel[pointIndex] += acc;

            if (pointIndex > 0) pointVel[pointIndex - 1] += spread * (WaterPoints[pointIndex].Y - WaterPoints[pointIndex - 1].Y);
            if (pointIndex < surfacePoints - 1) pointVel[pointIndex + 1] += spread * (WaterPoints[pointIndex].Y - WaterPoints[pointIndex + 1].Y);
        }

        public void MovePoint(int index, int delta)
        {
            deltaHeight[index] += delta;
        }

        public void Update(DeltaTime deltaTime)
        {
            for (int i = 0; i < surfacePoints; i++)
                UpdatePoint(i, deltaTime);

            for (int i = 0; i < surfacePoints; i++)
            {
                WaterPoints[i].Y += deltaHeight[i];
                deltaHeight[i] = 0;
            }
        }

        private void PopulatePoints()
        {
            WaterPoints = new PointF[surfacePoints + 2];

            int pointDistance = Program.Width / (surfacePoints - 1);

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
