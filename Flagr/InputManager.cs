using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flagr
{
    class InputManager
    {
        private Form form;

        public Point MouseLocation { get; protected set; } = Point.Empty;
        public bool MouseDown { get; protected set; } = false;

        private List<Keys> pressed = new List<Keys>();

        public delegate void KeyDownEvent(KeyEventArgs e, bool repeat);

        public event KeyDownEvent OnKeyDown; 

        public InputManager(Form form)
        {
            this.form = form;

            form.MouseMove += Form_MouseMove;
            form.MouseDown += Form_MouseDown;
            form.MouseUp += Form_MouseUp;
            form.KeyDown += Form_KeyDown;
            form.KeyUp += Form_KeyUp;
        }

        public bool IsPressed(Keys key)
        {
            return pressed.Contains(key);
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            pressed.Remove(e.KeyCode);
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (OnKeyDown != null)
                OnKeyDown.Invoke(e, pressed.Contains(e.KeyCode));

            if (!pressed.Contains(e.KeyCode))
                pressed.Add(e.KeyCode);
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            MouseDown = false;
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDown = true;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            MouseLocation = e.Location;
        }
    }
}
