using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.UI
{
    abstract class UIElement
    {
        public virtual Point Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
                SetOrigin();
            }
        }

        public virtual Size Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
                SetOrigin();
            }
        }

        public virtual DrawMode DrawMode
        {
            get
            {
                return drawMode;
            }

            set
            {
                drawMode = value;
                SetOrigin();
            }
        }

        protected Point origin;

        private Point location = Point.Empty;
        private Size size = Size.Empty;
        private DrawMode drawMode = DrawMode.TopLeft;

        public abstract void Draw(Graphics g);
        public virtual void Update(DeltaTime deltaTime) { }

        protected virtual void SetOrigin()
        {
            int originX = location.X, originY = location.Y;

            if(drawMode == DrawMode.Centered)
            {
                originX -= size.Width / 2;
                originY -= size.Height / 2;
            }

            origin = new Point(originX, originY);
        }
    }
}
