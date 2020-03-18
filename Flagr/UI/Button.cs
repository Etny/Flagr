using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.UI
{
    class Button : UIElement
    {

        public SelectMode SelectMode { get; set; } = SelectMode.OnMouseDown;
        public bool Selectable { get; set; } = true;
        public bool Hoverable { get; set; } = true;
        public int RimSize { get; set; } = 3;
        public TextLabel Label { get; protected set; } = new TextLabel();
        public bool Hovered { get; protected set; } = false;

        public event EventHandler OnSelect;

        protected bool lastDown = false;

        protected int textMargin = 5;

        public float hoverBuildup = 0;
        public float hoverBuildupMax = 100;
        public float hoverBuildupIncrease = 600;
        public float hoverBuildupDecrease = 650;

        public float clickBuildup = 0;
        public float clickBuildUpMax = 200;
        public float clickBuildupDecrease = 280;

        public Button(Point Location, Size Size) 
        {
            this.Location = Location;
            this.Size = Size;

            Label.Font = new Font("Arial", 30, FontStyle.Regular);
        }

        public Button(int X, int Y, int Width, int Height) : this(new Point(X, Y), new Size(Width, Height)) { }

        public Button(int X, int Y, Size Size) : this(new Point(X, Y), Size) { }

        public Button(Point Location, int Width, int Height) : this(Location, new Size(Width, Height)) { }

        public Button(Point point) : this(Point.Empty, 100, 50) { }

        protected override void SetOrigin()
        {
            base.SetOrigin();
            Label.Location = new Point(origin.X + Size.Width / 2, origin.Y + Size.Height / 2);
            Label.Bounds = new Size(Size.Width - (textMargin * 2), Size.Height - (textMargin * 2));
        }

        public virtual void Select()
        {
            if (!Selectable) return;

            if (OnSelect != null) OnSelect.Invoke(this, null);

            hoverBuildup = 0;
            clickBuildup = clickBuildUpMax;
        }
        
        public override void Update(DeltaTime deltaTime)
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
                if (mouseDown != lastDown && Selectable)
                {
                    if ((SelectMode == SelectMode.OnMouseUp && mouseDown == false) 
                    || (SelectMode == SelectMode.OnMouseDown && mouseDown == true))
                        Select();
                }

                if (Hoverable && hoverBuildup < hoverBuildupMax && clickBuildup == 0)
                    hoverBuildup += hoverBuildupIncrease * deltaTime.Seconds;
            }

            else if(hoverBuildup > 0)
            {
                hoverBuildup -= hoverBuildupDecrease * deltaTime.Seconds;
                if (hoverBuildup < 0) hoverBuildup = 0;
            }

            lastDown = mouseDown;
        }

        public override void Draw(Graphics g)
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
