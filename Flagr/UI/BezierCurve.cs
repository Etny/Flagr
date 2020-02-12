using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.UI
{
    class BezierCurve : Curve
    {

        private PointF[] points;

        public BezierCurve(PointF[] points, float Range)
        {
            this.points = new PointF[points.Length + 2];

            this.points[0] = new PointF(0f, 0f);
            this.points[this.points.Length - 1] = new PointF(1f, 1f);

            for (int i = 0; i < points.Length; i++)
                this.points[i + 1] = points[i];

            this.Range = Range;
        }

        public BezierCurve(PointF[] points) : this(points, 1) { }


        public override float GetValue(float x)
        {
            if (x < 0)
                return 0;

            if (x == 0)
                return this.points[0].Y;

            float t = x / Range;

            int currentSize = this.points.Length - 1;

            PointF[] points = this.points;
            PointF[] interPoints = new PointF[currentSize];

            while (currentSize >= 1)
            {
                for (int i = 0; i < currentSize; i++)            
                    interPoints[i] = Intermediate(points[i], points[i + 1], t);
                
                currentSize--;

                points = interPoints;
                interPoints = new PointF[currentSize];
                
            }

            return points[0].Y;
        }


        private PointF Intermediate(PointF p1, PointF p2, float f)
        {
            float x = p1.X + (f * (p2.X - p1.X));
            float y = p1.Y + (f * (p2.Y - p1.Y));

            return new PointF(x, y);
        }

    }
}
