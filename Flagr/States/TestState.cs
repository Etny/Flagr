using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flagr.Flags;

namespace Flagr.States
{
    class TestState : State
    {

        int x = 2000;
        int index = 0;
        Flag flag;

        Stopwatch watch;
        public TestState() : base()
        {
            watch = new Stopwatch();
            flag = Program.Flags.GetFlags()[index];
        }

        public override void Update(DeltaTime deltaTime)
        {
            graphics.FillRectangle(Brushes.White, 0, 0, Program.Width, Program.Height);
            graphics.DrawImageUnscaled(flag.Image, 0, 0);
            graphics.DrawString(flag.Country, Form1.DefaultFont, Brushes.Black, 400, 400);

            x -= deltaTime.Milliseconds;

            if(x <= 0)
            {
                x = 1000;
                //index = new Random().Next(0, Program.Flags.getFlags().Count-1);
                index++;
                flag = Program.Flags.GetFlags()[index];
            }

            Console.WriteLine(deltaTime.Milliseconds + " " + x);

            base.Update(deltaTime);
        }
    }
}
