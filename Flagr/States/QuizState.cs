using Flagr.Flags;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flagr.UI;

namespace Flagr.States
{
    class QuizState : State
    {
        private Queue<Flag> flagQueue;
        private Button[] buttons;

        private readonly int buttonWidth = 300;
        private readonly int buttonHeight = 60;
        private readonly int buttonSpacing = 26;
        private readonly int buttonTop = Program.Height / 2 + 100;

        private Random rng;

       

        public QuizState() : base() 
        {
            flagQueue = new Queue<Flag>();
            buttons = new Button[4];
            rng = new Random();

            int[] buttonX = { Program.Width / 2 - (buttonWidth + buttonSpacing/2), Program.Width / 2 + buttonSpacing / 2 };
            int[] buttonY = {buttonTop, buttonTop + buttonHeight + buttonSpacing};

            for (int i = 0; i < 3; i++)
                AddRandomFlag();

            for(int i = 0; i < 4; i++)
                buttons[i] = new QuizButton(buttonX[i%2], buttonY[i/2], buttonWidth, buttonHeight, i+1);
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

            graphics.DrawImage(currentFlag.Image, Program.Width / 2 - currentFlag.Image.Width / 2, Program.Height / 3 - currentFlag.Image.Height / 2);

            foreach (Button b in buttons)
                b.Update(deltaTime);
                    
            foreach (Button b in buttons)
                b.Draw(graphics);
            
            base.Update(deltaTime);
        }

    }
}
