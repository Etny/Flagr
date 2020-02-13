using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flagr.States
{
    abstract class State
    {
        protected Graphics graphics;

        public bool IsCurrentState { get; private set; }

        public State()
        {
            this.graphics = Form1.BufferGraphics;
            Program.Input.OnKeyDown += KeyDownCheck;
        }

        public virtual void Update(DeltaTime deltaTime) { }

        public virtual void SetCurrentState(bool current)
        {
            this.IsCurrentState = current;
        }

        private void KeyDownCheck(KeyEventArgs e, bool repeating)
        {
            if(IsCurrentState) KeyPressed(e, repeating);
        }

        protected virtual void KeyPressed(KeyEventArgs e, bool repeat) { }

    }
}
