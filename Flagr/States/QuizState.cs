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
        private Queue<Flag> flagQueue;
        private QuizButton[] buttons;

        private readonly int flagX = Program.Width / 2;
        private readonly int flagY = Program.Height / 3;

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
        private FlagFrame frame;
        private bool frameSet = false;


        public QuizState() : base() 
        {
            flagQueue = new Queue<Flag>();
            buttons = new QuizButton[4];
            rng = new Random();
            

            int[] buttonX = { Program.Width / 2 - (buttonWidth + buttonSpacing/2), Program.Width / 2 + buttonSpacing / 2 };
            int[] buttonY = {buttonTop, buttonTop + buttonHeight + buttonSpacing};

            for (int i = 0; i < 5; i++)
                AddRandomFlag();

            for (int i = 0; i < 4; i++)
            {
                buttons[i] = new QuizButton(buttonX[i % 2], buttonY[i / 2], buttonWidth, buttonHeight, i + 1);
                buttons[i].OnSelect += ButtonPressed;
            }

            frame = new FlagFrame()
            {
                RimSize = 20,
                LerpTime = .3f
            };

            SetFrame(flagQueue.Peek());
            frame.ReachTargets();
           

            NextFlag();
        }

        private void AddRandomFlag()
        {
            Flag f;

            do
            {
                f = Program.Flags.GetRandomFlag();
            } while (flagQueue.Contains(f));

            f.LoadImage();
            flagQueue.Enqueue(f);
        }

        private void ButtonPressed(object sender, EventArgs e)
        {
            QuizButton button = (QuizButton)sender;

            foreach (QuizButton b in buttons)
                b.CorrectAnswer = (b == Correct);

            if (!button.CorrectAnswer)
                Correct.Highlight();

            NextFlag();
        }

        protected override void KeyPressed(KeyEventArgs e, bool repeating)
        {
            if (!repeating && buttonKeys.Contains(e.KeyCode))
                buttons[buttonKeys.IndexOf(e.KeyCode)%4].Select();
        }

        private void NextFlag()
        {
            frameSet = false;
            

            if (Correct != null)
            {
                flagQueue.Dequeue().UnloadImage();
                AddRandomFlag();
            }
            
            SetFrame(flagQueue.Peek());

            Correct = buttons[rng.Next(0, 4)];

            List<Flag> choosenAnswers = new List<Flag>();

            choosenAnswers.Add(flagQueue.Peek());

            foreach(QuizButton b in buttons)
            {
                if (Correct == b)
                {
                    b.Label.Text = flagQueue.Peek().Country;
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

        private void SetFrame(Flag currentFlag)
        {
            if (currentFlag.IsImageLoaded)
            {
                frame.SetTargets(currentFlag.ImageSize.Width, currentFlag.ImageSize.Height);
                frameSet = true;
            }
        }

        public override void Update(DeltaTime deltaTime)
        {
            Flag currentFlag = flagQueue.Peek();

            if (!frameSet) SetFrame(currentFlag);
            frame.Update(deltaTime);

            foreach (QuizButton b in buttons)
                b.Update(deltaTime);

            graphics.FillRectangle(Brushes.White, 0, 0, Program.Width, Program.Height);

        //    graphics.FillRectangle(Brushes.Orange, flagX - frame.Width / 2, flagY - frame.Height / 2, frame.Width, frame.Height);

            if (currentFlag.IsImageLoaded)
                graphics.DrawImage(currentFlag.Image, flagX - currentFlag.Image.Width / 2, flagY - currentFlag.Image.Height / 2);


            foreach (QuizButton b in buttons)
                b.Draw(graphics);
            
            base.Update(deltaTime);
        }

    }
}


internal class FlagFrame
{

    public int RimSize { get; set; } = 0;
    public float Width { get; set; } = 0;
    public float Height { get; set; } = 0;
    public int TargetWidth { get; set; } = 0;
    public int TargetHeight { get; set; } = 0;

    public float LerpTime { get; set; } = 1;

    private float deltaWidth, deltaHeight;

    public void Update(DeltaTime deltaTime)
    {
        Width += MoveTowards(deltaTime, Width, deltaWidth, TargetWidth);
        Height += MoveTowards(deltaTime, Height, deltaHeight, TargetHeight);
    }

    public void SetTargets(int TargetWidth, int TargetHeight)
    {
        this.TargetWidth = TargetWidth + (RimSize * 2);
        this.TargetHeight = TargetHeight + (RimSize * 2);

        this.deltaWidth = ((float)this.TargetWidth - Width) * (1/LerpTime); 
        this.deltaHeight = ((float)this.TargetHeight - Height) * (1/LerpTime);
    }

    public void ReachTargets()
    {
        Width = (int)TargetWidth;
        Height = (int)TargetHeight;
    }

    private float MoveTowards(DeltaTime deltaTime, float current, float delta, int target)
    {
        if ((int)current == target) return 0;

        float lerp = delta * deltaTime.Seconds;
        float dif = (float)target - current;

        if (Math.Abs(lerp) > Math.Abs(dif)) lerp = dif;

        return lerp;
    }

 
        

}