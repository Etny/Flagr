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

        public int CurrentStreak { get; set; }

        private int maxStreak = 5;

        private Image odometer;
        private float scale = .4f;

        public StreakTracker()
        {
            odometer = Properties.Resources.Odometer;

            Size = new Size((int)(scale * odometer.Width), (int)(scale * odometer.Height));

            odometer = Util.ResizeImage(odometer, Size.Width, Size.Height);

            DrawMode = DrawMode.Centered;
        }

        public void UpdateStreak(bool correct)
        {
            if (correct)
                if (CurrentStreak < maxStreak) CurrentStreak++;
            else
                CurrentStreak = 0;
        }


        public override void Draw(Graphics g)
        {
            g.DrawImage(odometer, origin);
        }
    }
}
