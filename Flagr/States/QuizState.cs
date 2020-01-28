using Flagr.Flags;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.States
{
    class QuizState : State
    {
        private Queue<Flag> flagQueue;
        private Random rng;

        public QuizState() : base() 
        {
            flagQueue = new Queue<Flag>();
            rng = new Random();

            for (int i = 0; i < 3; i++) AddRandomFlag();
        }

        private void AddRandomFlag()
        {
            Flag f;

            do
            {
                f = Program.Flags.GetRandomFlag();
            } while (flagQueue.Contains(f));

            flagQueue.Enqueue(f);
        }

        public override void Update(DeltaTime deltaTime)
        {
            Flag currentFlag = flagQueue.Peek();

            graphics.FillRectangle(Brushes.White, 0, 0, Program.Width, Program.Height);

            graphics.DrawImage(currentFlag.Image, Program.Width / 2 - currentFlag.Image.Width / 2, Program.Height / 2 - currentFlag.Image.Height / 2);

            base.Update(deltaTime);
        }

    }
}
