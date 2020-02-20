using Flagr.Flags;
using Flagr.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Flagr.States
{
    class QuizState : State
    {

        //UI elements
        private FlagContainer[] flags;
        private QuizButton[] buttons;

        //Question info
        public int QuestionCount { get; set; } = 13;
        private TextLabel QuestionCounter;
        private int currentQuestion = 0;

        //Scoring
        private ResultsState results;
        private StreakTracker tracker;

        //Queue info
        private readonly int queueSize = 4;
        private readonly int currentFlagIndex = 2;

        //Flag placement
        private readonly int currentFlagX = Program.Width / 2;
        private readonly int currentFlagY = Program.Height / 3;
        private readonly int flagSpacing = Program.Width / 2;
        private readonly BezierCurve bezier;

        //Button placement
        private readonly int buttonWidth = 300;
        private readonly int buttonHeight = 60;
        private readonly int buttonSpacing = 26;
        private readonly int buttonTop = Program.Height / 2 + 100;

        //Keys mapped to answer buttons
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

        //Correct answer randomization
        private Random rng;
        private QuizButton Correct;

        //Freezetime between questions
        private float freezeTime = 0;
        private float maxFreezeTime = .7f;

        Stopwatch timer;
        private int ticks = 0;

        public QuizState() : base()
        {
            //Set UI elements
            flags = new FlagContainer[queueSize];
            buttons = new QuizButton[4];

            //Set question counter
            QuestionCounter = new TextLabel()
            {
                Font = new Font("Arial", 30, FontStyle.Bold),
                DrawMode = UI.DrawMode.Centered,
                Location = new Point(currentFlagX, 40)
            };

            //Set scoring
            results = new ResultsState();
            tracker = new StreakTracker()
            {
                Location = new Point(150, Program.Height - 150)
            };

            //Set curve for flag movement
            bezier = new BezierCurve(new PointF[] { new PointF(1, -.4f), new PointF(0.8f, 1) }, maxFreezeTime);

            //Set answer selection
            rng = new Random();

            //Add and correctly position buttons
            int[] buttonX = { Program.Width / 2 - (buttonWidth + buttonSpacing / 2), Program.Width / 2 + buttonSpacing / 2 };
            int[] buttonY = { buttonTop, buttonTop + buttonHeight + buttonSpacing };

            for (int i = 0; i < 4; i++)
            {
                buttons[i] = new QuizButton(buttonX[i % 2], buttonY[i / 2], buttonWidth, buttonHeight, i + 1);
                buttons[i].OnSelect += ButtonPressed;
            }

            //Fill the flag queue
            for (int i = 0; i < 5; i++)
                AddRandomFlag();

            //Select random answers
            NextFlag();

            //Remove the flag at end of queue
            flags[queueSize - 1] = null;

            timer = new Stopwatch();
            timer.Start();
        }

        /// <summary>
        /// Get the flag that is the subject of the current question.
        /// </summary>
        /// <returns>The current flag</returns>
        private Flag GetCurrentFlag()
        {
            return flags[currentFlagIndex].Flag;
        }

        /// <summary>
        /// Sets the onscreen location of FlagContainers.
        /// </summary>
        private void SetFlagLocations()
        {
            float slideOffset = freezeTime <= 0 ? 0 : bezier.GetValue(maxFreezeTime - freezeTime) * (float)flagSpacing;

            for (int i = 0; i < flags.Length; i++)
            {
                FlagContainer c = flags[i];

                if (c == null) continue;

                int offset = currentFlagIndex - i;

                c.SetLocation(currentFlagX + (offset * flagSpacing) - (int)slideOffset, currentFlagY);
            }
        }


        /// <summary>
        /// Moves all FlagContainers forwards in the queue. The one at the end is moved to the beginning and is given the new flag.
        /// </summary>
        /// <param name="flag">Flag to add. Can be null.</param>
        private void Enqueue(Flag flag)
        {
            FlagContainer temp = flags[flags.Length - 1];

            for (int i = flags.Length - 1; i > 0; i--)
                flags[i] = flags[i - 1];

            if (temp == null)
                temp = new FlagContainer();
            else
                temp.Flag.UnloadImage();

            if (flag == null)
                temp = null;
            else
            {
                temp.Flag = flag;
                flag.LoadImage();
            }

            flags[0] = temp;
        }

        /// <summary>
        /// Check if any of the FlagContainers have the flag set as their current flag.
        /// </summary>
        /// <param name="f">The flag to check for.</param>
        /// <returns>Wether f is currently in the flag queue.</returns>
        private bool IsCurrentlyInQueue(Flag f)
        {
            foreach (FlagContainer c in flags)
                if (c != null && c.Flag == f) return true;

            return false;
        }

        /// <summary>
        /// Enqueues a random flag. Will enqueue null instead if adding a flag would cause the total flags added to exceed the question total.
        /// </summary>
        private void AddRandomFlag()
        {
            if (currentQuestion >= QuestionCount - (queueSize - 2))
            {
                Enqueue(null);
                return;
            }

            Flag f;

            do
            {
                f = Program.Flags.GetRandomFlag();
            } while (IsCurrentlyInQueue(f));

            f.LoadImage();
            Enqueue(f);
        }

        /// <summary>
        /// Fired when one of the quiz buttons is pressed. Check whether correct answer was selected, and set freezetime to allow for flag sliding animation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPressed(object sender, EventArgs e)
        {
            QuizButton button = (QuizButton)sender;

            foreach (QuizButton b in buttons)
                b.CorrectAnswer = (b == Correct);

            tracker.UpdateStreak(true);

            if (!button.CorrectAnswer)           
                Correct.Highlight();
            else
                results.Score += 100;

            freezeTime = maxFreezeTime;

            foreach (QuizButton b in buttons)
            {
                b.Selectable = false;
                b.Hoverable = false;
            }

            if (currentQuestion >= QuestionCount)
                EndQuiz();
        }

        protected override void KeyPressed(KeyEventArgs e, bool repeating)
        {
            if (!repeating && buttonKeys.Contains(e.KeyCode))
                buttons[buttonKeys.IndexOf(e.KeyCode) % 4].Select();
        }

        /// <summary>
        /// Removes the flag at the end of the flag queue, adds a new flag and randomly selects new answers. 
        /// Move on to results screen if all questions have been answered
        /// </summary>
        private void NextFlag()
        {
            if (Correct != null)
                AddRandomFlag();

            SetFlagLocations();

            currentQuestion++;
            if (currentQuestion > QuestionCount)
                return;
            
            QuestionCounter.Text = currentQuestion + "/" + QuestionCount;

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

        private void EndQuiz()
        {
            results.FinalizeScore();
            Program.SetCurrentState(new TransitionState(this, results, 1f, .5f, .5f));
        }

        public override void Update(DeltaTime deltaTime)
        {
            /*
            ticks++;
            if(timer.ElapsedMilliseconds >= 1000)
            {
                Console.WriteLine("{0} tps", ticks);
                ticks = 0;
                timer.Restart();
            }
            */

            if (freezeTime > 0)
            {
                SetFlagLocations();

                freezeTime -= deltaTime.Seconds;

                if (freezeTime <= 0)
                {
                    foreach (QuizButton b in buttons)
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

            tracker.Update(deltaTime);

            Draw();
        }

        private void Draw()
        {
            graphics.FillRectangle(Brushes.White, 0, 0, Program.Width, Program.Height);

            QuestionCounter.Draw(graphics);

            for (int i = 0; i < flags.Length; i++)
                if (flags[i] != null)
                    flags[i].Draw(graphics);

            foreach (QuizButton b in buttons)
                b.Draw(graphics);

            tracker.Draw(graphics);
        }

    }



}