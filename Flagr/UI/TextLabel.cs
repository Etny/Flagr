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

        public Color Color = Color.Black;
        public Color BackdropColor = Color.White;

        public int BackdropSpacing = 10;
        public Size Bounds = Size.Empty;

        public bool DrawBackdrop = false;


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

        private Size backdropSize = Size.Empty;
        private Point backDropLocation = Point.Empty;

        protected void SetSize()
        {
            originSet = false;

            if (Bounds.IsEmpty)
                return;

            Size textSize = Form1.BufferGraphics.MeasureString(Text, Font).ToSize();

            while (textFont.SizeInPoints > 1)
            {
                if (textSize.Width <= Bounds.Width && textSize.Height <= Bounds.Height)
                    break;

                textFont = new Font(Font.FontFamily, Font.SizeInPoints - 1, Font.Style);

                textSize = Form1.BufferGraphics.MeasureString(Text, Font).ToSize();
            }

            backdropSize.Width = textSize.Width + (BackdropSpacing * 2);
            backdropSize.Height = textSize.Height + (BackdropSpacing * 2);
        }

        protected override void SetOrigin()
        {
            originSet = false;
        }

        public Size GetTextSize()
        {
            return Form1.BufferGraphics.MeasureString(Text, Font).ToSize();
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
                    
                backDropLocation.X = drawX - BackdropSpacing;
                backDropLocation.Y = drawY - BackdropSpacing;

                origin = new Point(drawX, drawY);
                originSet = true;
            }

            if (DrawBackdrop)
                g.FillRectangle(new SolidBrush(BackdropColor), backDropLocation.X, backDropLocation.Y, backdropSize.Width, backdropSize.Height);

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
