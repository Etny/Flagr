using Flagr.Flags;
using Flagr.States;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.UI
{
    class AnswerBlock : UIElement
    {
        public String Letter
        {
            set
            {
                LetterLabel.Text = value;
            }

            get
            {
                return LetterLabel.Text;
            }
        }

        public Color Color;
        public TextLabel LetterLabel;

        public bool Enabled = false;
        private float currentFade = 0f;
        private float maxFade = .4f;

        public QuizButton[] Buttons;
        public int ButtonSpacing = 15;
        public Size ButtonSize = new Size(Program.Width / 4, 50);
        public QuizButton Correct;

        private Boat currentBoat = null;

        private int index;
        private Random rng = new Random();

        private SpeedState state;

        public AnswerBlock(Point Location, Color Color, Size Size, String Letter, int index, SpeedState State)
        {
            DrawMode = DrawMode.TopLeft;
            this.Size = Size;
            this.Location = Location;
            this.state = State;

            LetterLabel = new TextLabel()
            {
                Location = new Point(origin.X + (Size.Width / 2), origin.Y + (Size.Height / 6)),
                Color = Color.White,
                Font = new Font("Arial", 30, FontStyle.Bold)
            };

            this.Color = Color;
            this.Letter = Letter;
            this.index = index;
            
            Buttons = new QuizButton[3];

            PupolateButtons();

            Disable();
        }

        public void PupolateButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                Buttons[i] = new QuizButton(origin.X + (Size.Width / 2),
                                            origin.Y + (Size.Height / 5) * 2 + i * (ButtonSpacing + ButtonSize.Height),
                                            ButtonSize.Width,
                                            ButtonSize.Height,
                                            (index * 3) + i + 1)
                                            { DrawMode = DrawMode.Centered };

                Buttons[i].OnSelect += OnButtonPress;
            }
        }

        public void NewQuestion(Boat boat)
        {
            Correct = Buttons[rng.Next(0, 3)];

            currentBoat = boat;
            boat.CurrentBlock = this;

            List<Flag> choosenAnswers = new List<Flag>();
            Flag CorrectFlag = Program.Flags.GetRandomFlag();

            choosenAnswers.Add(CorrectFlag);

            foreach (QuizButton b in Buttons)
            {
                b.CorrectAnswer = false;
                b.Hoverable = true;
                b.Selectable = true;
                b.clickBuildupDecrease = 100;

                if (Correct == b)
                {
                    b.Label.Text = CorrectFlag.Country;
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
                    b.CorrectAnswer = false;
                }
            }

            boat.SetFlag(CorrectFlag);
            boat.IndicatorColor = Color;
            boat.IndicatorLabel.Text = Letter;

            Enabled = true;
        }

        private void OnButtonPress(Object sender, EventArgs e)
        {
            QuizButton pressed = (QuizButton)sender;

            Correct.CorrectAnswer = true;

            currentBoat.Answered = true;

            if (!pressed.CorrectAnswer)
            {
                Correct.Highlight();
                currentBoat.Sink();
            }
            else
            {
                currentBoat.CorrectlyAnswered = true;
                currentBoat.SpeedOff();
            }

            state.ScoreBoat(currentBoat);

            foreach (QuizButton b in Buttons)
            {
                b.Hoverable = false;
                b.Selectable = false;
            }
        }

        public void Disable()
        {
            if (!Enabled)
                return;

            Enabled = false;
            
            foreach(QuizButton b in Buttons)
            {
                b.Hoverable = false;
                b.Selectable = false;
                b.clickBuildupDecrease = 280;
            }
        }

        public override void Update(DeltaTime deltaTime)
        {
            if(Enabled && currentFade < maxFade)
            {
                currentFade += deltaTime.Seconds;
                if (currentFade > maxFade) currentFade = maxFade;
            }
            else if(!Enabled && currentFade > 0)
            {
                currentFade -= deltaTime.Seconds;
                if (currentFade < 0) currentFade = 0;
            }

            foreach (QuizButton b in Buttons)
                b.Update(deltaTime);
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color), origin.X, origin.Y, Size.Width, Size.Height);

            LetterLabel.Draw(g);

            if (currentFade != 0)
            {
                foreach (QuizButton b in Buttons)
                    b.Draw(g);
            }

            if(currentFade < maxFade)
            {
                int alpha = 255 - (int)((currentFade/maxFade)*(float)255);

                g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, Color.R, Color.G, Color.B)), origin.X, Buttons[0].Location.Y - (ButtonSize.Height / 2), Size.Width, Buttons.Length * ButtonSize.Height + (Buttons.Length-1) * ButtonSpacing);
            }
        }
    }
}
