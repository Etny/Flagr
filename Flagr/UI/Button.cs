using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.UI
{
    class Button
    {

        public Size Size { get; set; }
        public Point Location { get; set; }
        public SelectMode SelectMode { get; set; } = SelectMode.OnMouseDown;
        public DrawMode DrawMode
        {
            get
            {
                return drawMode;
            }

            set
            {
                drawMode = value;
                SetTopLeft();
            }
        } 

        public int RimSize { get; set; } = 3;
        public TextLabel Label { get; protected set; } = new TextLabel();

        public bool Hovered { get; protected set; } = false;

        public event EventHandler OnSelect;

        private DrawMode drawMode = DrawMode.TopLeft;
        protected Point origin;
        protected bool lastDown = false;

        protected float hoverBuildup = 0;
        protected float hoverBuildupMax = 100;
        protected float hoverBuildupIncrease = 380;
        protected float hoverBuildupDecrease = 650;

        protected float clickBuildup = 0;
        protected float clickBuildUpMax = 200;
        protected float clickBuildupDecrease = 480;

        public Button(Point Location, Size Size) 
        {
            this.Location = Location;
            this.Size = Size;

            SetTopLeft();
        }

        public Button(int X, int Y, int Width, int Height) : this(new Point(X, Y), new Size(Width, Height)) { }

        public Button(int X, int Y, Size Size) : this(new Point(X, Y), Size) { }

        public Button(Point Location, int Width, int Height) : this(Location, new Size(Width, Height)) { }

        public Button() : this(Point.Empty, 100, 50) { }

        protected virtual void SetTopLeft()
        {
            int topX = Location.X, topY = Location.Y;

            if (DrawMode == DrawMode.Centered)
            {
                topX = Location.X - Size.Width / 2;
                topY= Location.Y - Size.Height / 2;
            }

            origin = new Point(topX, topY);
            Label.Location = new Point(topX + Size.Width / 2, topY + Size.Height / 2);
            Label.Text = "Text";
        }

        public virtual void Select()
        {
            if (OnSelect != null) OnSelect.Invoke(this, null);

            hoverBuildup = 0;
            clickBuildup = clickBuildUpMax;
        }
        
        public virtual void Update(DeltaTime deltaTime)
        {
            Point mouse = Program.Input.MouseLocation;
            bool mouseDown = Program.Input.MouseDown;

            Hovered = (mouse.X >= origin.X && mouse.X <= origin.X + Size.Width)
                   && (mouse.Y >= origin.Y && mouse.Y <= origin.Y + Size.Height);

            if(clickBuildup > 0)
            {
                clickBuildup -= clickBuildupDecrease * deltaTime.Seconds;
                if (clickBuildup < 0) clickBuildup = 0;
            }

            if (Hovered)
            {
                if (mouseDown != lastDown)
                {
                    if ((SelectMode == SelectMode.OnMouseUp && mouseDown == false) 
                    || (SelectMode == SelectMode.OnMouseDown && mouseDown == true))
                        Select();
                }

                if (hoverBuildup < hoverBuildupMax && clickBuildup == 0)
                    hoverBuildup += hoverBuildupIncrease * deltaTime.Seconds;
            }

            else if(hoverBuildup > 0)
            {
                hoverBuildup -= hoverBuildupDecrease * deltaTime.Seconds;
                if (hoverBuildup < 0) hoverBuildup = 0;
            }

            lastDown = mouseDown;
        }

        public virtual void Draw(Graphics g)
        {
            Color fillColor = Color.FromArgb(255 - (int)hoverBuildup, 255 - (int)clickBuildup, 255);

            g.FillRectangle(Brushes.Black, origin.X, origin.Y, Size.Width, Size.Height);
            g.FillRectangle(new SolidBrush(fillColor), origin.X + RimSize, origin.Y + RimSize, Size.Width - (RimSize * 2), Size.Height - (RimSize * 2));

            Label.Draw(g);
        }

    }

    public enum SelectMode
    {
        OnMouseDown, OnMouseUp
    }

    public enum DrawMode
    {
        Centered, TopLeft
    }
}
