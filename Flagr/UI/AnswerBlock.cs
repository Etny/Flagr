using Flagr.Flags;
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

        public QuizButton[] Buttons;
        public int ButtonSpacing = 15;
        public Size ButtonSize = new Size(Program.Width / 4, 50);

        private int index;
        private Random rng = new Random();

        public AnswerBlock(Point Location, Color Color, Size Size, String Letter, int index)
        {
            DrawMode = DrawMode.TopLeft;
            this.Size = Size;
            this.Location = Location;

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
        }

        public void PupolateButtons()
        {
            for (int i = 0; i < 3; i++)
                Buttons[i] = new QuizButton(origin.X + (Size.Width / 2),
                                            origin.Y + (Size.Height / 5) * 2 + i * (ButtonSpacing + ButtonSize.Height),
                                            ButtonSize.Width,
                                            ButtonSize.Height,
                                            (index * 3) + i + 1)
                { DrawMode = DrawMode.Centered };
        }

        public void NewQuestion(Boat boat)
        {
            QuizButton Correct = Buttons[rng.Next(0, 3)];

            List<Flag> choosenAnswers = new List<Flag>();
            Flag CorrectFlag = Program.Flags.GetRandomFlag();

            choosenAnswers.Add(CorrectFlag);

            foreach (QuizButton b in Buttons)
            {
                if (Correct == b)
                {
                    b.Label.Text = CorrectFlag.Country;
                    b.CorrectAnswer = true;
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

        public void Update(DeltaTime deltaTime)
        {
            foreach (QuizButton b in Buttons)
                b.Update(deltaTime);
        }

        public override void Draw(Graphics g)
        {
            if (!Enabled)
            {
               // g.FillRectangle(Brushes.Gray, origin.X, origin.Y, Size.Width, Size.Height);
                return;
            }


            g.FillRectangle(new SolidBrush(Color), origin.X, origin.Y, Size.Width, Size.Height);

            LetterLabel.Draw(g);

            foreach (QuizButton b in Buttons)
                b.Draw(g);
        }
    }
}
