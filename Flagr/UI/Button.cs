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

        public SelectMode SelectMode { get; set; } = SelectMode.OnMouseUp;
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

        public bool Hovered { get; protected set; } = false;

        public event EventHandler OnSelect;

        private DrawMode drawMode = DrawMode.origin;
        protected Point origin;
        protected bool lastDown = false;

        protected float hoverBuildUp = 0;
        protected float hoverBuildUpMax = 100;

        protected float clickBuildup = 0;
        protected float clickBuildUpMax = 200;

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

        private void SetTopLeft()
        {
            int topX = Location.X, topY = Location.Y;

            if (DrawMode == DrawMode.Centered)
            {
                topX = Location.X - Size.Width / 2;
                topY= Location.Y - Size.Height / 2;
            }

            origin = new Point(topX, topY);
        }

        public virtual void Select()
        {
            hoverBuildUp = 0;
            clickBuildup = clickBuildUpMax;

            if(OnSelect != null) OnSelect.Invoke(this, null);
        }
        
        public virtual void Update(DeltaTime deltaTime)
        {
            Point mouse = Program.Input.MouseLocation;
            bool mouseDown = Program.Input.MouseDown;

            Hovered = (mouse.X >= origin.X && mouse.X <= origin.X + Size.Width)
                   && (mouse.Y >= origin.Y && mouse.Y <= origin.Y + Size.Height);

            if(clickBuildup > 0)
            {
                clickBuildup -= 5;
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

                if (hoverBuildUp < hoverBuildUpMax && clickBuildup == 0)
                    hoverBuildUp += 360 * deltaTime.Seconds;
            }

            else if(hoverBuildUp > 0)
            {
                hoverBuildUp -= 480 * deltaTime.Seconds;
                if (hoverBuildUp < 0) hoverBuildUp = 0;
            }

            lastDown = mouseDown;
        }

        public virtual void Draw(Graphics g)
        {
            Color fillColor = Color.FromArgb(255 - (int)hoverBuildUp, 255 - (int)clickBuildup, 255);

            g.FillRectangle(Brushes.Black, origin.X, origin.Y, Size.Width, Size.Height);
            g.FillRectangle(new SolidBrush(fillColor), origin.X + RimSize, origin.Y + RimSize, Size.Width - (RimSize * 2), Size.Height - (RimSize * 2));
        }

    }

    public enum SelectMode
    {
        OnMouseDown, OnMouseUp
    }

    public enum DrawMode
    {
        Centered, origin
    }
}
