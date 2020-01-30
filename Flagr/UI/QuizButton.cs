using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flagr.UI
{
    class QuizButton : Button
    {

        public int NumberBoxWidth { get; set; } = 40;
        public int Number {

            get
            {
                return num;
            }

            set
            {
                num = value;
                numString = num + "";
            }
        }

        private int num;
        private string numString;

        public Font NumberFont { get; set; } = new Font("Arial", 30, FontStyle.Bold); 


        public QuizButton(int X, int Y, int Width, int Height, int Number) : base(X, Y, Width, Height)
        {
            this.Number = Number;
        }

        public QuizButton(int X, int Y, int Width, int Height) : this(X, Y, Width, Height, 1) { }
        

        public override void Draw(Graphics g)
        {
            base.Draw(g);

            g.FillRectangle(Brushes.Black, origin.X, origin.Y, NumberBoxWidth, Size.Height);

            int numWidth = (int)g.MeasureString(numString, NumberFont).Width;
            int numHeight = TextRenderer.MeasureText(numString, NumberFont).Height;

            g.DrawString(numString, NumberFont, Brushes.White, origin.X + NumberBoxWidth / 2 - numWidth / 2, origin.Y + Size.Height / 2 - numHeight / 2);

        }
    }
}
