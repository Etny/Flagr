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

        public bool CorrectAnswer
        {
            get
            {
                return correct;
            }

            set
            {
                correct = value;
                clickBuildup = 0;
            }
        }
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
        private bool correct = false;
        

        public Font NumberFont { get; set; } = new Font("Arial", 30, FontStyle.Bold); 


        public QuizButton(int X, int Y, int Width, int Height, int Number) : base(X, Y, Width, Height)
        {
            this.Number = Number;
            this.Label.Font = new Font("Arial", 30, FontStyle.Regular);
        }

        public QuizButton(int X, int Y, int Width, int Height) : this(X, Y, Width, Height, 1) { }


        public void Highlight()
        {
            hoverBuildup = 0;
            clickBuildup = clickBuildUpMax;
        }

        protected override void SetOrigin()
        {
            base.SetOrigin();

            this.Label.Location = new Point((origin.X + NumberBoxWidth) + ((Size.Width - NumberBoxWidth) / 2), origin.Y + Size.Height / 2);
            this.Label.Bounds = new Size(Size.Width - NumberBoxWidth - (textMargin * 2), Size.Height - (textMargin * 2));
        }

        public override void Draw(Graphics g)
        {
            Color fillColor;
            
            if(CorrectAnswer)
                fillColor = Color.FromArgb(255 - (int)hoverBuildup - (int)clickBuildup, 255, 255 - (int)clickBuildup);
            else
                fillColor = Color.FromArgb(255 - (int)hoverBuildup, 255 - (int)clickBuildup, 255 - (int)clickBuildup);

            g.FillRectangle(Brushes.Black, origin.X, origin.Y, Size.Width, Size.Height);
            g.FillRectangle(new SolidBrush(fillColor), origin.X + RimSize, origin.Y + RimSize, Size.Width - (RimSize * 2), Size.Height - (RimSize * 2));

            g.FillRectangle(Brushes.Black, origin.X, origin.Y, NumberBoxWidth, Size.Height);

            int numWidth = (int)g.MeasureString(numString, NumberFont).Width;
            int numHeight = TextRenderer.MeasureText(numString, NumberFont).Height;

            Color numColor;

            if (CorrectAnswer)
                numColor = Color.FromArgb(255 - (int)clickBuildup, 255, 255 - (int)clickBuildup);
            else
                numColor = Color.FromArgb(255, 255 - (int)clickBuildup, 255 - (int)clickBuildup);

            g.DrawString(numString, NumberFont, new SolidBrush(numColor), origin.X + NumberBoxWidth / 2 - numWidth / 2, origin.Y + Size.Height / 2 - numHeight / 2);

            Label.Draw(g);
        }
    }
}
