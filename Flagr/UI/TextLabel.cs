using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flagr.UI
{
    class TextLabel : UIElement
    {

        public Color Color { get; set; } = Color.Black;
        public Size Bounds { get; set; } = Size.Empty;


        public TextLabel()
        {
            this.DrawMode = DrawMode.Centered;
        }

        public String Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
                textFont = defaultFont;
                SetSize();
            }
        }

        public Font Font
        {
            get
            {
                return textFont;
            }

            set
            {
                textFont = value;
                defaultFont = value;
                SetSize();
            }
        }

        private Font textFont = Program.AppForm.Font;
        private Font defaultFont = Program.AppForm.Font;
        private String text = "";
        protected bool originSet = false;

        protected void SetSize()
        {
            originSet = false;

            if (Bounds.IsEmpty)
                return;

            for (int i = 0; i < 9999; i++)
            {
                Size textSize = Form1.BufferGraphics.MeasureString(Text, Font).ToSize();

                if (textSize.Width <= Bounds.Width && textSize.Height <= Bounds.Height)
                    break;

                textFont = new Font(Font.FontFamily, Font.SizeInPoints - 1, Font.Style);
            }
        }

        public override void Draw(Graphics g)
        {
            if (!originSet) { 
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

            g.DrawString(Text, Font, new SolidBrush(Color), origin.X, origin.Y);
            //g.FillRectangle(Brushes.Orange, Location.X, Location.Y, 100, 100);
        }

        public TextLabel Clone()
        {
            return new TextLabel()
            {
                Text = this.text,
                Color = this.Color,
                Location = this.Location,
                DrawMode = this.DrawMode,
                Bounds = this.Bounds,
                Font = this.Font
            };
        }
    }
}
