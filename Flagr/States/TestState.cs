using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.States
{
    class TestState : State
    {

        float x = 0;

        Stopwatch watch;
        public TestState()
        {
            watch = new Stopwatch();
        }

        public override void Update(DeltaTime deltaTime)
        {

            Form1.BufferGraphics.FillRectangle(Brushes.White, 0, 0, Program.Width, Program.Height);
            Form1.BufferGraphics.FillRectangle(Brushes.Red, (int) x, 0, 100, 100);

            x += 100 * deltaTime.Seconds;

            base.Update(deltaTime);
        }
    }
}
