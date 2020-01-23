using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.States
{
    class StartupState : State
    {
        private bool loading = false;

        public StartupState() : base() {}

        public override void Update(DeltaTime deltaTime)
        {
            if (!loading)
            {
                loading = true;

                graphics.FillRectangle(Brushes.Black, 0, 0, Program.Width, Program.Height);
                graphics.DrawString("Loading...", Form1.DefaultFont, Brushes.White, 100, 100);

                base.Update(deltaTime);

                Program.Flags.Populate();
            }
            else
            {
                Program.CurrentState = new TestState();
            }
        }
    }
}
