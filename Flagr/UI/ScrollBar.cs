using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.UI
{
    class ScrollBar : UIElement
    {
        public int BarWidth = 20;
        public int HandleHeight = 50;

        public Color BarColor = Color.FromArgb(100, 20, 20, 20);
        public Color HandleColor = Color.FromArgb(255, 50, 50, 50);

        public float ScrollPosition
        {
            get
            {
                return scrollPos;
            }

            set
            {
                scrollPos = value;
                barY = (int)(scrollPos * (Program.Height - HandleHeight));
            }
        }

        public float FadeDelay = .8f;
        public float FadeTime = .2f;

        private float currentFadeDelay = 0f;
        private float currentFade = 0f;
        private int currentBarWidth = 0;

        private float scrollPos = 0f;
        private int barY = 0;

        public void Popup()
        {
            currentFadeDelay = FadeDelay;
            currentFade = FadeTime;
            currentBarWidth = BarWidth;
        }

        public override void Update(DeltaTime deltaTime)
        {
            if (currentFade <= 0) return;

            if (currentFadeDelay > 0)
                currentFadeDelay -= deltaTime.Seconds;
            else
                currentFade -= deltaTime.Seconds;

            currentBarWidth = (int)((currentFade / FadeTime) * BarWidth);
        }

        public override void Draw(Graphics g)
        {
            if (currentFade <= 0) return;

            g.FillRectangle(new SolidBrush(BarColor), Program.Width - currentBarWidth, 0, currentBarWidth, Program.Height);
            g.FillRectangle(new SolidBrush(HandleColor), Program.Width - currentBarWidth, barY, Program.Width, HandleHeight);
        }
    }
}
