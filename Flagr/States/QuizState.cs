using Flagr.Flags;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flagr.UI;
using System.Windows.Forms;
using Flagr;

namespace Flagr.States
{
    class QuizState : State
    {
        private FlagContainer[] flags;
        private QuizButton[] buttons;

        private readonly int queueSize = 4;
        private readonly int currentFlagIndex = 2;

        private readonly int currentFlagX = Program.Width / 2;
        private readonly int currentFlagY = Program.Height / 3;
        private readonly int flagSpacing = Program.Width / 2;
        private readonly Curve flagMoveCurve;

        private readonly int buttonWidth = 300;
        private readonly int buttonHeight = 60;
        private readonly int buttonSpacing = 26;
        private readonly int buttonTop = Program.Height / 2 + 100;

        private readonly List<Keys> buttonKeys = new List<Keys>()
        {
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.NumPad1,
            Keys.NumPad2,
            Keys.NumPad3,
            Keys.NumPad4
        };

        private Random rng;
        private QuizButton Correct;

        private float freezeTime = 0;
        private float maxFreezeTime = 1;

        public QuizState() : base()
        {
            flags = new FlagContainer[queueSize];
            buttons = new QuizButton[4];
            rng = new Random();
            flagMoveCurve = new Curve()
            {
                Range = maxFreezeTime
            };


            int[] buttonX = { Program.Width / 2 - (buttonWidth + buttonSpacing / 2), Program.Width / 2 + buttonSpacing / 2 };
            int[] buttonY = { buttonTop, buttonTop + buttonHeight + buttonSpacing };

            for (int i = 0; i < 5; i++)
                AddRandomFlag();

            for (int i = 0; i < 4; i++)
            {
                buttons[i] = new QuizButton(buttonX[i % 2], buttonY[i / 2], buttonWidth, buttonHeight, i + 1);
                buttons[i].OnSelect += ButtonPressed;
            }

            NextFlag();
        }

        private Flag GetCurrentFlag()
        {
            return flags[currentFlagIndex].Flag;
        }

        private void SetFlagLocations()
        {
            float slideOffset = freezeTime <= 0 ? 0 : flagMoveCurve.GetValue(maxFreezeTime-freezeTime) * (float)flagSpacing;

            for(int i = 0; i < flags.Length; i++)
            {
                FlagContainer c = flags[i];

                if (c == null) continue;

                int offset = currentFlagIndex - i;

                c.SetLocation(currentFlagX + (offset * flagSpacing) - (int)slideOffset, currentFlagY);
            }
        }

        private void Enqueue(Flag f)
        {
            FlagContainer temp = flags[flags.Length - 1];

            for(int i=flags.Length-1; i>0; i--)           
                flags[i] = flags[i - 1];

            if (temp == null)
                temp = new FlagContainer();
            else
                temp.Flag.UnloadImage();

            temp.Flag = f;

            flags[0] = temp;

            f.LoadImage();
        }

        private bool IsCurrentlyInQueue(Flag f)
        {
            foreach (FlagContainer c in flags)
                if (c != null && c.Flag == f) return true;

            return false;
        }

        private void AddRandomFlag()
        {
            Flag f;

            do
            {
                f = Program.Flags.GetRandomFlag();
            } while (IsCurrentlyInQueue(f));

            f.LoadImage();
            Enqueue(f);
        }

        private void ButtonPressed(object sender, EventArgs e)
        {
            QuizButton button = (QuizButton)sender;

            foreach (QuizButton b in buttons)
                b.CorrectAnswer = (b == Correct);

            if (!button.CorrectAnswer)
                Correct.Highlight();

            freezeTime = maxFreezeTime;

            foreach (QuizButton b in buttons)
            {
                b.Selectable = false;
                b.Hoverable = false;
            }
        }

        protected override void KeyPressed(KeyEventArgs e, bool repeating)
        {
            if (!repeating && buttonKeys.Contains(e.KeyCode))
                buttons[buttonKeys.IndexOf(e.KeyCode) % 4].Select();
        }

        private void NextFlag()
        {
            if (Correct != null)
            {
             //   flagQueue.Dequeue().UnloadImage();
                AddRandomFlag();
            }

            SetFlagLocations();

            Correct = buttons[rng.Next(0, 4)];

            List<Flag> choosenAnswers = new List<Flag>();

            choosenAnswers.Add(GetCurrentFlag());

            foreach (QuizButton b in buttons)
            {
                if (Correct == b)
                {
                    b.Label.Text = GetCurrentFlag().Country;
                }
                else
                {
                    Flag falseFlag;

                    do
                    {
                        falseFlag = Program.Flags.GetRandomFlag();
                    } while (choosenAnswers.Contains(falseFlag));

                    choosenAnswers.Add(falseFlag);
                    b.Label.Text = falseFlag.Country;
                }
            }
        }

        public override void Update(DeltaTime deltaTime)
        {
            if (freezeTime > 0)
            {
                SetFlagLocations();

                freezeTime -= deltaTime.Seconds;

                if(freezeTime <= 0)
                {
                    foreach(QuizButton b in buttons)
                    {
                        b.Selectable = true;
                        b.Hoverable = true;
                    }

                    freezeTime = 0;

                    NextFlag();
                }
            }

            foreach (QuizButton b in buttons)
                b.Update(deltaTime);

            Draw();
        }

        private void Draw()
        {
            graphics.FillRectangle(Brushes.White, 0, 0, Program.Width, Program.Height);

            for (int i = 0; i < flags.Length; i++)
                if (flags[i] != null)
                    flags[i].Draw(graphics);

            foreach (QuizButton b in buttons)
                b.Draw(graphics);
        }

    }
 
        

}