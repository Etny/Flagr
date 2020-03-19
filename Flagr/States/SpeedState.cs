using Flagr.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace Flagr.States
{
    class SpeedState : State
    {

        private List<Boat> boatTemplates = new List<Boat>();
        private List<Boat> boats = new List<Boat>();
        private List<Boat> deadBoats = new List<Boat>();
        private readonly float boatSpeed = 250f;
        private readonly float boatScale = .75f;
        private readonly int maxBoats = 3;
        private readonly float maxBoatSpawnDelay = 1.7f;
        private float spawnCountdown = 2f;

        private Random rng = new Random();

        public readonly static int WaterLine = Program.Height / 5 * 2;
        private readonly int waterHeight = 150;
        private readonly int waterPoints = 51;
        private readonly Color waterColor = Color.DeepSkyBlue;
        private FancyWater water;

        private int score = 0;
        private readonly int scorePerBoat = 100;
        private TextLabel scoreLabel;
        private FadeLabel scoreAddLabel;

        private AnswerBlock[] blocks;

        public long Time = 30 * 1000;
        private TextLabel timeLabel;

        private SpeedResultsState results;
        private bool ending = false;

        //Keys mapped to answer buttons
        private readonly List<Keys> buttonKeys = new List<Keys>()
        {
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.D5,
            Keys.D6,
            Keys.D7,
            Keys.D8,
            Keys.D9,
            Keys.NumPad1,
            Keys.NumPad2,
            Keys.NumPad3,
            Keys.NumPad4,
            Keys.NumPad5,
            Keys.NumPad6,
            Keys.NumPad7,
            Keys.NumPad8,
            Keys.NumPad9
        };

        public SpeedState()
        {
            water = new FancyWater(WaterLine, waterHeight, waterPoints, waterColor);
            results = new SpeedResultsState();

            scoreLabel = new TextLabel()
            {
                Font = new Font("Arial", 30, FontStyle.Bold),
                DrawMode = UI.DrawMode.Centered,
                Location = new Point(Program.Width - 120, 60),
                Text = "0"
            };

            scoreAddLabel = new FadeLabel()
            {
                Font = new Font("Arial", 20, FontStyle.Bold),
                DrawMode = UI.DrawMode.Centered,
                Location = new Point(Program.Width - 120, 60),
                Text = "0",
                FadeTime = .8f,
                FadeDeltaY = -60
            };

            timeLabel = new TextLabel()
            {
                Font = new Font("Arial", 30, FontStyle.Bold),
                DrawMode = UI.DrawMode.Centered,
                Location = new Point(Program.Width / 2, 60)
            };

            SetTimeLabel();

            Size blockSize = new Size(Program.Width / 3 + 1, Program.Height - (WaterLine + waterHeight));
            blocks = new AnswerBlock[3];
            blocks[0] = new AnswerBlock(new Point(0, WaterLine + waterHeight), Color.Red, blockSize, "A", 0, this);
            blocks[1] = new AnswerBlock(new Point(blockSize.Width, WaterLine + waterHeight), Color.Green, blockSize, "B", 1, this);
            blocks[2] = new AnswerBlock(new Point(blockSize.Width * 2, WaterLine + waterHeight), Color.Blue, blockSize, "C", 2, this);

            ParseBoatData();

            AddBoat();
        }

        private void ParseBoatData()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.boatData);

            XmlNodeList boatList = doc.DocumentElement.ChildNodes;

            for(int i = 0; i < boatList.Count; i++)
                boatTemplates.Add(new Boat(boatList.Item(i), boatScale));
        }

        private void AddBoat()
        {
            Boat boat = boatTemplates[rng.Next(0, boatTemplates.Count)].Clone();

            foreach(AnswerBlock block in blocks)
            {
                if (block.Enabled)
                    continue;

                block.NewQuestion(boat);
                break;
            }

            boat.CreateImage();

            boats.Add(boat);


            if (boats.Count < maxBoats) spawnCountdown = maxBoatSpawnDelay;
        }

        public override void Scroll(MouseEventArgs e)
        {
            float delta = Math.Sign(e.Delta) * 3f;
            int closest = water.ClosestPoint(Program.Input.MouseLocation.X);

            water.MovePoint(closest - 1, -.5f * delta);
            water.MovePoint(closest, delta);
            water.MovePoint(closest + 1, -.5f * delta);
        }
        protected override void KeyPressed(KeyEventArgs e, bool repeating)
        {
            if (e.KeyCode == Keys.Escape)
                Program.SetCurrentState(new TransitionState(this, Program.MainMenu, .4f, .1f, .4f));

            if (repeating || !buttonKeys.Contains(e.KeyCode))
                return;

            int buttonsPerBlock = blocks[0].Buttons.Length;
            int index = buttonKeys.IndexOf(e.KeyCode) % (blocks.Length * buttonsPerBlock);
            AnswerBlock block = blocks[index / blocks.Length];


            if(block.Enabled) block.Buttons[index % buttonsPerBlock].Select(); 
        }

        private void SetTimeLabel()
        {
            int seconds = (int)((Time % (60 * 1000)) / 1000);
            int minutes = (int)((Time - seconds) / (60 * 1000));

            timeLabel.Text = minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
        }

        private void EndQuiz()
        {   ending = true;
            results.Score = score;
            results.FinalizeScore();
            Program.SetCurrentState(new TransitionState(this, results, 1f, .5f, .5f));
        }

        public void ScoreBoat(Boat b)
        {
            results.Questions++;

            //Points for getting a boat correct
            float pointsF = b.Speed * scorePerBoat;

            //Turn score negative 
            if (!b.CorrectlyAnswered)
            {
                if (b.Answered)
                    pointsF *= -.8f; // 80% penalty for guessing wrong
                else
                {
                    results.Missed++;
                    pointsF *= -.5f; // 50% penalty for letting a boat slip by
                }
            }
            else
            {
                results.CorrectAnswers++;
            }

            //Convert points to int
            int points = (int)pointsF;

            //Round to nearest 5 (for symmetries' sake)
            if(points % 5 != 0)
            {
                int dif = points % 5;
                if (dif <= 2) points -= dif;
                else points += 5 - dif;
            }

            //Make sure we don't let the score drop below 0
            if (points < 0 && score + points < 0)
                points = -1 * score;

            //Add (or subtract) new points
            score += points;
            scoreLabel.Text = "" + score;

            //Don't activate the fading label if we don't gain or lose any points
            if (points == 0)
                return;

            //Otherwise make the score fade label do its thing
            if(points > 0)
            {
                scoreAddLabel.Color = Color.Green;
                scoreAddLabel.Text = "+"+points;
            }
            else
            {
                scoreAddLabel.Color = Color.Red;
                scoreAddLabel.Text = "-" + (-1*points);
            }

            scoreAddLabel.StartFade();
        }

        public override void Update(DeltaTime deltaTime)
        {
            if(IsCurrentState && spawnCountdown > 0)
            {
                spawnCountdown -= deltaTime.Seconds;

                if (spawnCountdown <= 0 && boats.Count < maxBoats)
                    AddBoat();      
            }

            water.Update(deltaTime);
            scoreAddLabel.Update(deltaTime);

            
            if(IsCurrentState) Time -= deltaTime.Milliseconds;
            if(Time <= 0)
            {
                Time = 0;
                if(!ending) EndQuiz();
            }
            SetTimeLabel();

            foreach (Boat boat in boats)
            {
                boat.Update(deltaTime, water, boatSpeed);
                if (boat.OutOfBounds) deadBoats.Add(boat);
            }

            foreach(Boat dead in deadBoats)
            {
                if(dead.CurrentBlock != null) dead.CurrentBlock.Disable();
                if(!dead.Answered) ScoreBoat(dead);
                boats.Remove(dead);
                if(spawnCountdown <= 0) spawnCountdown = maxBoatSpawnDelay;
            }

            deadBoats.Clear();

            foreach (AnswerBlock block in blocks)
                block.Update(deltaTime);

            Draw();
        }

        private void Draw()
        {
            graphics.FillRectangle(Program.BackgroundBrush, 0, 0, Program.Width, Program.Height);

            foreach (Boat boat in boats)
                boat.Draw(graphics);

            foreach (AnswerBlock block in blocks)
                block.Draw(graphics);

            water.Draw(graphics);

            scoreLabel.Draw(graphics);
            scoreAddLabel.Draw(graphics);
            timeLabel.Draw(graphics);
        }

    }
}
