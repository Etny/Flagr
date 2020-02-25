using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.UI
{
    class FadeLabel : TextLabel
    {
        public int FadeDeltaX { get; set; } = 0;
        public int FadeDeltaY { get; set; } = 0;
        public float FadeTime { get; set; } = 1f;

        private float fade = 0;
        private bool setup = false;

        private Color textColor;
        private int deltaX, deltaY;

        public void StartFade()
        {
            fade = FadeTime;
        }

        public override void Update(DeltaTime deltaTime)
        {
            bool setData = fade != 0 || !setup;

            if(fade > 0)
            {
                fade -= deltaTime.Seconds;
                if (fade < 0) fade = 0;
            }

            if (setData)
            {
                float ratio = fade / FadeTime;

                int alpha = (int)(ratio * 255);
                textColor = Color.FromArgb(alpha, Color.R, Color.G, Color.B);

                deltaX = (int)(FadeDeltaX * (1-ratio));
                deltaY = (int)(FadeDeltaY * (1-ratio));
            }

            setup = true;
        }

        public override void Draw(Graphics g)
        {
            if (!originSet)
            {
                int drawX = Location.X, drawY = Location.Y;

                if (DrawMode == DrawMode.Centered)
                {
                    int numWidth = (int)g.MeasureString(Text, Font).Width;
                    int numHeight = (int)g.MeasureString(Text, Font).Height;

                    drawX -= numWidth / 2;
                    drawY -= numHeight / 2;
                }

                origin = new Point(drawX, drawY);
                originSet = true;
            }

            g.DrawString(Text, Font, new SolidBrush(textColor), origin.X + deltaX, origin.Y + deltaY);
        }
    }
}
